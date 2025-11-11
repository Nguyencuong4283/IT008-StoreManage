using Microsoft.Data.Sqlite;
using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace Store.Services
{
    public static class SanPhanService 
    {
        private static readonly string dbPath = Path.Combine(AppContext.BaseDirectory, "store.db");

        public static void Initialize()
        {
            Console.WriteLine($"Database Path: {dbPath}");

            string dbDirectory = Path.GetDirectoryName(dbPath)!;
            if (!Directory.Exists(dbDirectory))
                Directory.CreateDirectory(dbDirectory);

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SanPham (
                    MaSP TEXT PRIMARY KEY,
                    TenSP TEXT NOT NULL,
                    GiaSP REAL NOT NULL,
                    SoLuongSP INTEGER NOT NULL,
                    HinhAnhDuongDan TEXT NOT NULL,
                    KichThuocSP TEXT NOT NULL,
                    LoaiSP TEXT NOT NULL,
                    MoTaSP TEXT
                );";
                cmd.ExecuteNonQuery();
            }
        }
        //CRUD
        //Create
        public static void InsertSanPham(SanPham sp)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                string newMaSP = GenerateNewMaSP();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO SanPham 
                (MaSP, TenSP, GiaSP, SoLuongSP, HinhAnhDuongDan, KichThuocSP, LoaiSP, MoTaSP)
                VALUES ($MaSP, $TenSP, $GiaSP, $SoLuongSP, $HinhAnhDuongDan, $KichThuocSP, $LoaiSP, $MoTaSP)";
                cmd.Parameters.AddWithValue("$MaSP", newMaSP);
                cmd.Parameters.AddWithValue("$TenSP", sp.TenSP);
                cmd.Parameters.AddWithValue("$GiaSP", (double)sp.GiaSP);
                cmd.Parameters.AddWithValue("$SoLuongSP", sp.SoLuongSP);
                cmd.Parameters.AddWithValue("$HinhAnhDuongDan", sp.HinhAnhDuongDan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$KichThuocSP", sp.KichThuocSP);
                cmd.Parameters.AddWithValue("$LoaiSP", sp.LoaiSP);
                cmd.Parameters.AddWithValue("$MoTaSP", sp.MoTaSP ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        //Read one
        public static SanPham GetOneSanPham(string MaSP)
        {
            var sanPham = new SanPham();
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaSP, TenSP, GiaSP, SoLuongSP, LoaiSP, KichThuocSP, MoTaSP, HinhAnhDuongDan FROM SanPham";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sp = new SanPham
                        {
                            MaSP = reader.GetString(0),
                            TenSP = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            GiaSP = reader.IsDBNull(2) ? 1000 : reader.GetDecimal(2),
                            SoLuongSP = reader.IsDBNull(3) ? 1 : reader.GetInt32(3),
                            LoaiSP = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            KichThuocSP = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            MoTaSP = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        };

                        if (!reader.IsDBNull(7))
                        {
                            try
                            {
                                string imagePath = reader.GetString(7);
                                if (File.Exists(imagePath))
                                    sp.HinhAnhSP = new Bitmap(imagePath);
                            }
                            catch { }
                        }
                        if (MaSP == sp.MaSP)
                        {
                            return sp;
                        }
                    }
                    return null;
                }
            }
        }
        //Đếm số lượng sản phẩm
        public static int CountSanPham()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM SanPham";
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
        //Read All
        public static List<SanPham> GetAllSanPham()
        {
            var sanPhams = new List<SanPham>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaSP, TenSP, GiaSP, SoLuongSP, LoaiSP, KichThuocSP, MoTaSP, HinhAnhDuongDan FROM SanPham";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sp = new SanPham
                        {
                            MaSP = reader.GetString(0),
                            TenSP = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            GiaSP = reader.IsDBNull(2) ? 1000 : reader.GetDecimal(2),
                            SoLuongSP = reader.IsDBNull(3) ? 1 : reader.GetInt32(3),
                            LoaiSP = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            KichThuocSP = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            MoTaSP = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        };

                        if (!reader.IsDBNull(7))
                        {
                            try
                            {
                                string imagePath = reader.GetString(7);
                                if (File.Exists(imagePath))
                                    sp.HinhAnhSP = new Bitmap(imagePath);
                            }
                            catch { }
                        }

                        sanPhams.Add(sp);
                    }
                }
            }

            return sanPhams;
        }
        //Update
        public static void UpdateSanPham(SanPham sp)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                UPDATE SanPham 
                SET TenSP = $TenSP,
                    GiaSP = $GiaSP,
                    SoLuongSP = $SoLuongSP,
                    HinhAnhDuongDan = $HinhAnhDuongDan,
                    KichThuocSP = $KichThuocSP,
                    LoaiSP = $LoaiSP,
                    MoTaSP = $MoTaSP
                WHERE MaSP = $MaSP";
                cmd.Parameters.AddWithValue("$MaSP", sp.MaSP);
                cmd.Parameters.AddWithValue("$TenSP", sp.TenSP);
                cmd.Parameters.AddWithValue("$GiaSP", (double)sp.GiaSP);
                cmd.Parameters.AddWithValue("$SoLuongSP", sp.SoLuongSP);
                cmd.Parameters.AddWithValue("$HinhAnhDuongDan", sp.HinhAnhDuongDan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$KichThuocSP", sp.KichThuocSP);
                cmd.Parameters.AddWithValue("$LoaiSP", sp.LoaiSP);
                cmd.Parameters.AddWithValue("$MoTaSP", sp.MoTaSP ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // Delete
        public static void DeleteSanPham(string maSP)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM SanPham WHERE MaSP = $MaSP";
                cmd.Parameters.AddWithValue("$MaSP", maSP);
                cmd.ExecuteNonQuery();
            }
        }
        //Tạo MaSP
        public static string GenerateNewMaSP()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaSP FROM SanPham ORDER BY MaSP DESC LIMIT 1";
                var result = cmd.ExecuteScalar()?.ToString();

                if (string.IsNullOrEmpty(result))
                    return "SP001";

                int number = int.Parse(result.Substring(2));
                return $"SP{(number + 1):D3}";
            }
        }
    }
}
