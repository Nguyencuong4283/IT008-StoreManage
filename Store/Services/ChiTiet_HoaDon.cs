using Microsoft.Data.Sqlite;
using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Store.Services
{
    public static class ChiTiet_HoaDonService
    {
        private static readonly string dbPath = Path.Combine(AppContext.BaseDirectory, "store.db");

        // ------------------ INIT TABLE ------------------
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
                CREATE TABLE IF NOT EXISTS ChiTiet_HoaDon (
                    MaHD TEXT NOT NULL,
                    MaSP TEXT NOT NULL,
                    SoLuong INTEGER NOT NULL,
                    DonGia REAL NOT NULL,
                    KhuyenMai INTEGER DEFAULT 0,
                    ThanhTien REAL NOT NULL,
                    PRIMARY KEY (MaHD, MaSP),
                    FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
                    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
                );";
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ CREATE ------------------
        public static void InsertChiTiet_HoaDon(ChiTiet_HoaDon ct)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO ChiTiet_HoaDon 
                (MaHD, MaSP, SoLuong, DonGia, KhuyenMai, ThanhTien)
                VALUES ($MaHD, $MaSP, $SoLuong, $DonGia, $KhuyenMai, $ThanhTien);";

                cmd.Parameters.AddWithValue("$MaHD", ct.MaHD);
                cmd.Parameters.AddWithValue("$MaSP", ct.MaSP);
                cmd.Parameters.AddWithValue("$SoLuong", ct.SoLuong);
                cmd.Parameters.AddWithValue("$DonGia", (double)ct.DonGia);
                cmd.Parameters.AddWithValue("$KhuyenMai", ct.KhuyenMai);
                cmd.Parameters.AddWithValue("$ThanhTien", (double)ct.ThanhTien);

                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ READ ALL ------------------
        public static List<ChiTiet_HoaDon> GetAllChiTiet_HoaDon()
        {
            var chiTiets = new List<ChiTiet_HoaDon>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaHD, MaSP, SoLuong, DonGia, KhuyenMai, ThanhTien
                FROM ChiTiet_HoaDon;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ct = new ChiTiet_HoaDon
                        {
                            MaHD = reader.GetString(0),
                            MaSP = reader.GetString(1),
                            SoLuong = reader.GetInt32(2),
                            DonGia = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            KhuyenMai = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            ThanhTien = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5)
                        };
                        chiTiets.Add(ct);
                    }
                }
            }

            return chiTiets;
        }

        // ------------------ READ BY MaHD ------------------
        public static List<ChiTiet_HoaDon> GetChiTiet_HoaDonByMaHD(string maHD)
        {
            var chiTiets = new List<ChiTiet_HoaDon>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaHD, MaSP, SoLuong, DonGia, KhuyenMai, ThanhTien
                FROM ChiTiet_HoaDon
                WHERE MaHD = $MaHD;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ct = new ChiTiet_HoaDon
                        {
                            MaHD = reader.GetString(0),
                            MaSP = reader.GetString(1),
                            SoLuong = reader.GetInt32(2),
                            DonGia = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            KhuyenMai = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            ThanhTien = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5)
                        };
                        chiTiets.Add(ct);
                    }
                }
            }

            return chiTiets;
        }

        // ------------------ READ ONE ------------------
        public static ChiTiet_HoaDon? GetChiTiet_HoaDonByKey(string maHD, string maSP)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaHD, MaSP, SoLuong, DonGia, KhuyenMai, ThanhTien
                FROM ChiTiet_HoaDon 
                WHERE MaHD = $MaHD AND MaSP = $MaSP;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                cmd.Parameters.AddWithValue("$MaSP", maSP);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ChiTiet_HoaDon
                        {
                            MaHD = reader.GetString(0),
                            MaSP = reader.GetString(1),
                            SoLuong = reader.GetInt32(2),
                            DonGia = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            KhuyenMai = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                            ThanhTien = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5)
                        };
                    }
                }
            }

            return null;
        }

        // ------------------ UPDATE ------------------
        public static void UpdateChiTiet_HoaDon(ChiTiet_HoaDon ct)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                UPDATE ChiTiet_HoaDon SET
                    SoLuong = $SoLuong,
                    DonGia = $DonGia,
                    KhuyenMai = $KhuyenMai,
                    ThanhTien = $ThanhTien
                WHERE MaHD = $MaHD AND MaSP = $MaSP;";

                cmd.Parameters.AddWithValue("$SoLuong", ct.SoLuong);
                cmd.Parameters.AddWithValue("$DonGia", (double)ct.DonGia);
                cmd.Parameters.AddWithValue("$KhuyenMai", ct.KhuyenMai);
                cmd.Parameters.AddWithValue("$ThanhTien", (double)ct.ThanhTien);
                cmd.Parameters.AddWithValue("$MaHD", ct.MaHD);
                cmd.Parameters.AddWithValue("$MaSP", ct.MaSP);

                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ DELETE ------------------
        public static void DeleteChiTiet_HoaDon(string maHD, string maSP)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM ChiTiet_HoaDon WHERE MaHD = $MaHD AND MaSP = $MaSP;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                cmd.Parameters.AddWithValue("$MaSP", maSP);
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ DELETE BY MaHD ------------------
        public static void DeleteChiTiet_HoaDonByMaHD(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM ChiTiet_HoaDon WHERE MaHD = $MaHD;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ TÍNH TỔNG TIỀN ------------------
        public static decimal GetTongTienByMaHD(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT SUM(ThanhTien) FROM ChiTiet_HoaDon WHERE MaHD = $MaHD;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                
                var result = cmd.ExecuteScalar();
                var total = result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
                
                System.Diagnostics.Debug.WriteLine($"[Service] GetTongTienByMaHD('{maHD}') = {total}");
                return total;
            }
        }


        // ------------------ INSERT NHIỀU CHI TIẾT ------------------
        public static void InsertMultipleChiTiet_HoaDon(List<ChiTiet_HoaDon> chiTiets)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var ct in chiTiets)
                        {
                            var cmd = connection.CreateCommand();
                            cmd.CommandText = @"
                            INSERT INTO ChiTiet_HoaDon 
                            (MaHD, MaSP, SoLuong, DonGia, KhuyenMai, ThanhTien)
                            VALUES ($MaHD, $MaSP, $SoLuong, $DonGia, $KhuyenMai, $ThanhTien);";

                            cmd.Parameters.AddWithValue("$MaHD", ct.MaHD);
                            cmd.Parameters.AddWithValue("$MaSP", ct.MaSP);
                            cmd.Parameters.AddWithValue("$SoLuong", ct.SoLuong);
                            cmd.Parameters.AddWithValue("$DonGia", (double)ct.DonGia);
                            cmd.Parameters.AddWithValue("$KhuyenMai", ct.KhuyenMai);
                            cmd.Parameters.AddWithValue("$ThanhTien", (double)ct.ThanhTien);

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // ------------------ ĐẾM SỐ LƯỢNG CHI TIẾT ------------------
        public static int GetCountByMaHD(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM ChiTiet_HoaDon WHERE MaHD = $MaHD;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        // ------------------ KIỂM TRA TỒN TẠI ------------------
        public static bool Exists(string maHD, string maSP)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM ChiTiet_HoaDon WHERE MaHD = $MaHD AND MaSP = $MaSP;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                cmd.Parameters.AddWithValue("$MaSP", maSP);
                
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }

        // ------------------ TÍNH TỔNG GIẢM GIÁ ------------------
       

        // ------------------ TÍNH TỔNG GIẢM GIÁ ------------------
        public static decimal GetTongGiamGiaByMaHD(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT SUM(DonGia * SoLuong * KhuyenMai / 100) FROM ChiTiet_HoaDon WHERE MaHD = $MaHD;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
            }
        }

        // ------------------ TÍNH TỔNG TRỊ GIÁ (TRƯỚC GIẢM) ------------------
        public static decimal GetTongTriGiaByMaHD(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT SUM(DonGia * SoLuong) FROM ChiTiet_HoaDon WHERE MaHD = $MaHD;";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
            }
        }
    }
}
