using Microsoft.Data.Sqlite;
using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace Store.Services
{
    public static class KhachHangService
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
                CREATE TABLE IF NOT EXISTS KhachHang (
                    MaKH TEXT PRIMARY KEY,
                    TenKH TEXT NOT NULL CHECK(TenKH <> ''),
                    SDT TEXT NOT NULL CHECK(SDT <> ''),
                    GioiTinh TEXT NOT NULL,
                    DiaChi TEXT CHECK(DiaChi <> ''),
                    Hang TEXT,
                    GhiChu TEXT,
                    TongMua REAL
                );";
                cmd.ExecuteNonQuery();
            }
        }

        // ----------------- CREATE -----------------
        public static void InsertKhachHang(KhachHang kh)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                string newMaKH = GenerateNewMaKH();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO KhachHang 
                (MaKH, TenKH, SDT, GioiTinh, DiaChi, Hang, GhiChu, TongMua)
                VALUES ($MaKH, $TenKH, $SDT, $GioiTinh, $DiaChi, $Hang, $GhiChu, $TongMua)";
                cmd.Parameters.AddWithValue("$MaKH", newMaKH);
                cmd.Parameters.AddWithValue("$TenKH", kh.TenKH);
                cmd.Parameters.AddWithValue("$SDT", kh.SDT);
                cmd.Parameters.AddWithValue("$GioiTinh", kh.GioiTinh);
                cmd.Parameters.AddWithValue("$DiaChi", kh.DiaChi);
                cmd.Parameters.AddWithValue("$Hang", kh.Hang);
                cmd.Parameters.AddWithValue("$GhiChu", kh.GhiChu);
                cmd.Parameters.AddWithValue("$TongMua", (double)kh.TongMua);
                cmd.ExecuteNonQuery();
            }
        }

        // ----------------- READ -----------------
        public static List<KhachHang> GetAllKhachHang()
        {
            var khachHangs = new List<KhachHang>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaKH, TenKH, SDT, GioiTinh, DiaChi, Hang, GhiChu, TongMua FROM KhachHang";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var kh = new KhachHang
                        {
                            MaKH = reader.GetString(0),
                            TenKH = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            SDT = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            GioiTinh = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            DiaChi = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Hang = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            GhiChu = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            TongMua = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                        };
                        khachHangs.Add(kh);
                    }
                }
            }

            return khachHangs;
        }
        //Đếm khách hàng
        public static int CountKhachHang()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM KhachHang";
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
        // ----------------- UPDATE -----------------
        public static void UpdateKhachHang(KhachHang kh)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                UPDATE KhachHang
                SET TenKH = $TenKH,
                    SDT = $SDT,
                    GioiTinh = $GioiTinh,
                    DiaChi = $DiaChi,
                    Hang = $Hang,
                    GhiChu = $GhiChu,
                    TongMua = $TongMua
                WHERE MaKH = $MaKH";

                cmd.Parameters.AddWithValue("$MaKH", kh.MaKH);
                cmd.Parameters.AddWithValue("$TenKH", kh.TenKH);
                cmd.Parameters.AddWithValue("$SDT", kh.SDT);
                cmd.Parameters.AddWithValue("$GioiTinh", kh.GioiTinh);
                cmd.Parameters.AddWithValue("$DiaChi", kh.DiaChi);
                cmd.Parameters.AddWithValue("$Hang", kh.Hang);
                cmd.Parameters.AddWithValue("$GhiChu", kh.GhiChu);
                cmd.Parameters.AddWithValue("$TongMua", (double)kh.TongMua);

                cmd.ExecuteNonQuery();
            }
        }

        // ----------------- DELETE -----------------
        public static void DeleteKhachHang(string maKH)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM KhachHang WHERE MaKH = $MaKH";
                cmd.Parameters.AddWithValue("$MaKH", maKH);
                cmd.ExecuteNonQuery();
            }
        }

        // ----------------- Generate ID -----------------
        public static string GenerateNewMaKH()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaKH FROM KhachHang ORDER BY MaKH DESC LIMIT 1";
                var result = cmd.ExecuteScalar()?.ToString();

                if (string.IsNullOrEmpty(result))
                    return "KH001";

                int number = int.Parse(result.Substring(2));
                return $"KH{(number + 1):D3}";
            }
        }
    }
}
