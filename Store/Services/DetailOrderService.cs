using Microsoft.Data.Sqlite;
using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Store.Services
{
    public static class DetailOrderService
    {
        private static readonly string dbPath = Path.Combine(AppContext.BaseDirectory, "store.db");

        // ------------------ INIT TABLE ------------------ //
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

        // ------------------ CREATE ------------------ //
        public static void InsertOderDetail(ChiTiet_HoaDon ct)
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

        // ------------------ READ ALL ------------------ //
        public static List<ChiTiet_HoaDon> GetAllOrderDetail()
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

        public static List<ChiTiet_HoaDon> GetOrderDetail(string maHD)
        {
            var chiTiets = new List<ChiTiet_HoaDon>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
            SELECT 
                ct.MaHD, ct.MaSP, ct.SoLuong, ct.DonGia, ct.KhuyenMai, ct.ThanhTien,
                sp.TenSP, sp.KichThuocSP, sp.GiaSP, sp.SoLuongSP, sp.LoaiSP, sp.MoTaSP, sp.HinhAnhDuongDan
            FROM ChiTiet_HoaDon ct
            LEFT JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE ct.MaHD = $MaHD;
        ";
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
                            ThanhTien = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                            SanPham = reader.IsDBNull(6) ? null : new SanPham
                            {
                                MaSP = reader.GetString(1),
                                TenSP = reader.GetString(6),
                                KichThuocSP = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                GiaSP = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                                SoLuongSP = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                LoaiSP = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                MoTaSP = reader.IsDBNull(11) ? "" : reader.GetString(11),
                                HinhAnhDuongDan = reader.IsDBNull(12) ? "" : reader.GetString(12)
                            }
                        };
                        chiTiets.Add(ct);
                    }
                }
            }

            return chiTiets;
        }
        // ------------------ READ BY MaHD ------------------ //
        public static List<ChiTiet_HoaDon> GetOrderDetailByCustomerID(string maHD)
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

        // ------------------ READ ONE ------------------ //
        public static ChiTiet_HoaDon? GetOrderDetailByKey(string maHD, string maSP)
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

        // ------------------ UPDATE ------------------ //
        public static void UpdateOrderDetail(ChiTiet_HoaDon ct)
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

        // ------------------ DELETE ------------------ //
        public static void DeleteOrderDetail(string maHD, string maSP)
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

        // ------------------ DELETE BY MaHD ------------------ //
        public static void DeleteOrderDetailByOrderID(string maHD)
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

        // ------------------ TOTAL INCOME ------------------ //
        public static decimal GetTongTienByOrderID(string maHD)
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


        // ------------------ INSERT DETAIL ------------------ //
        public static void InsertMultipleOrderDetail(List<ChiTiet_HoaDon> chiTiets)
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

        // ------------------ COUNT DETAIL------------------ //
        public static int GetCountByOrderID(string maHD)
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

        // ------------------ CHECK EXISTENCE ------------------ //
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
        
        // ------------------ CALCULATE DISCOUNT ------------------ //
        public static decimal GetTotalDiscountByOrderID(string maHD)
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

        // ------------------ TÍNH TỔNG TRỊ GIÁ (TRƯỚC GIẢM) ------------------ //
        public static decimal GetTotalValueByOrderID(string maHD)
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
