using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Store.Models;

namespace Store.Services;

public class ImportService
{
    private static readonly string dbPath = Path.Combine(AppContext.BaseDirectory, "store.db");

    // ----------------- Khởi tạo Database và bảng ----------------- //
    public static void Initialize()
    {
        Console.WriteLine($"Database Path: {dbPath}");

        var dbDirectory = Path.GetDirectoryName(dbPath)!;
        if (!Directory.Exists(dbDirectory)) Directory.CreateDirectory(dbDirectory);

        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            -- ===== Bảng Import =====
            CREATE TABLE IF NOT EXISTS Import (
            MaNhapKho TEXT PRIMARY KEY,
            NgayNhap TEXT NOT NULL,
            NhaCungCap TEXT NOT NULL,
            MaUser INTEGER,
            TongTien REAL,
            GhiChu TEXT,
            IsDelete INTEGER DEFAULT 0
            );

            -- ===== Bảng Chi tiết nhập kho =====
            CREATE TABLE IF NOT EXISTS ChiTiet_NhapKho (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            MaNK TEXT NOT NULL,
            MaSP TEXT NOT NULL,
            SoLuong INTEGER,
            DonGia REAL,
            ThanhTien REAL
            );

            -- ===== Bảng Sản phẩm =====
            CREATE TABLE IF NOT EXISTS SanPham (
            MaSP TEXT PRIMARY KEY,
            TenSP TEXT,
            GiaSP REAL,
            SoLuongSP INTEGER,
            KichThuocSP TEXT,
            LoaiSP TEXT,
            IsDelete INTEGER DEFAULT 0,
            MoTaSP TEXT,
            HinhAnhDuongDan TEXT
            );
            ";
            cmd.ExecuteNonQuery();
        }
    }

    // ----------------- Tạo Nhập kho ----------------- //
    public static void InsertNhapKho(
    Import nk,
    List<ChiTiet_NhapKho> chiTietList
    )
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        using var tran = connection.BeginTransaction();

        var maNK = GenerateNewMaNK();

        var cmdImport = connection.CreateCommand();
        cmdImport.CommandText = @"
        INSERT INTO Import
        (MaNhapKho, NgayNhap, NhaCungCap, MaUser, TongTien, GhiChu, IsDelete)
        VALUES ($MaNK, $NgayNhap, $NCC, $MaUser, $TongTien, $GhiChu, 0)";
        cmdImport.Parameters.AddWithValue("$MaNK", maNK);
        cmdImport.Parameters.AddWithValue("$NgayNhap", nk.NgayNhap.ToString("yyyy-MM-dd"));
        cmdImport.Parameters.AddWithValue("$NCC", nk.NhaCungCap);
        cmdImport.Parameters.AddWithValue("$MaUser", nk.MaUser);
        cmdImport.Parameters.AddWithValue("$TongTien", nk.TongTien);
        cmdImport.Parameters.AddWithValue("$GhiChu", nk.GhiChu ?? "");
        cmdImport.ExecuteNonQuery();

        foreach (var ct in chiTietList)
        {
            ct.MaNK = maNK;
            ct.ThanhTien = ct.SoLuong * ct.DonGia;

            var cmdCT = connection.CreateCommand();
            cmdCT.CommandText = @"
            INSERT INTO ChiTiet_NhapKho
            (MaNK, MaSP, SoLuong, DonGia, ThanhTien)
            VALUES ($MaNK, $MaSP, $SL, $DG, $TT)";
            cmdCT.Parameters.AddWithValue("$MaNK", ct.MaNK);
            cmdCT.Parameters.AddWithValue("$MaSP", ct.MaSP);
            cmdCT.Parameters.AddWithValue("$SL", ct.SoLuong);
            cmdCT.Parameters.AddWithValue("$DG", ct.DonGia);
            cmdCT.Parameters.AddWithValue("$TT", ct.ThanhTien);
            cmdCT.ExecuteNonQuery();

            var cmdUpdate = connection.CreateCommand();
            cmdUpdate.CommandText = @"
            UPDATE SanPham
            SET SoLuongSP = SoLuongSP + $SL
            WHERE MaSP = $MaSP";
            cmdUpdate.Parameters.AddWithValue("$SL", ct.SoLuong);
            cmdUpdate.Parameters.AddWithValue("$MaSP", ct.MaSP);
            cmdUpdate.ExecuteNonQuery();
        }

        tran.Commit();
    }


    // ----------------- Lấy dữ liệu ----------------- //
    public static List<Import> GetAllImport()
    {
        var nhapKhoList = new List<Import>();
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT MaNhapKho, NgayNhap, NhaCungCap, TongTien, GhiChu 
                FROM Import
                WHERE IsDelete = 0
                ORDER BY NgayNhap DESC;";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var nk = new Import
                    {
                        MaNK = reader.GetString(0),
                        NgayNhap = DateOnly.Parse(reader.GetString(1)),
                        NhaCungCap = reader.GetString(2),
                        TongTien = reader.GetDecimal(3),
                        GhiChu = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                    };
                    if (nk.IsDelete == 0) nhapKhoList.Add(nk);
                }
            }
        }

        return nhapKhoList;
    }

    // ----------------- Xóa Nhập kho ----------------- //
    public static void DeleteImport(string maNK)
    {
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                UPDATE Import
                SET IsDelete = 1
                WHERE MaNhapKho = $MaNhapKho";

            cmd.Parameters.AddWithValue("$MaNhapKho", maNK);
            cmd.ExecuteNonQuery();
        }
    }

    // ----------------- Cập nhật Nhập kho ----------------- //
    public static void UpdateImport(Import nk)
    {
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                UPDATE Import
                SET NgayNhap = $NgayNhap,
                    NhaCungCap = $NhaCungCap,
                    TongTien = $TongTien,
                    GhiChu = $GhiChu
                WHERE MaNhapKho = $MaNhapKho";

            cmd.Parameters.AddWithValue("$NgayNhap", nk.NgayNhap.ToString("dd/MM/yyyy"));
            cmd.Parameters.AddWithValue("$NhaCungCap", nk.NhaCungCap);
            cmd.Parameters.AddWithValue("$TongTien", nk.TongTien);
            cmd.Parameters.AddWithValue("$GhiChu", nk.GhiChu ?? string.Empty);
            cmd.Parameters.AddWithValue("$MaNhapKho", nk.MaNK);

            cmd.ExecuteNonQuery();
        }
    }

    // ----------------- Tạo Mã Nhập kho mới ----------------- //
    private static string GenerateNewImportID()
    {
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT MaNhapKho FROM Import
                ORDER BY MaNhapKho DESC
                LIMIT 1";

            var result = cmd.ExecuteScalar();
            if (string.IsNullOrEmpty(result?.ToString()))
            {
                return "NK001";
            }
            else
            {
                var lastMaNK = result.ToString()!;
                var lastNumber = int.Parse(lastMaNK.Substring(2));
                return $"NK{(lastNumber + 1).ToString("D3")}";
            }
        }
    }
}