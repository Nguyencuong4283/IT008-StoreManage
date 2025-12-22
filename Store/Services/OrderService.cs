using Microsoft.Data.Sqlite;
using Store.Models;
using Store.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Store.Services
{
    public static class OrderService
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
                CREATE TABLE IF NOT EXISTS HoaDon (
                    MaHD TEXT PRIMARY KEY,
                    NgayLapHD TEXT NOT NULL,
                    TongTienHD REAL NOT NULL,
                    GiamGiaHD REAL DEFAULT 0,
                    MaKH TEXT NOT NULL,
                    MaUser TEXT NOT NULL,
                    SoHD INTEGER NOT NULL,
                    TrangThaiHD TEXT NOT NULL,
                    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
                    FOREIGN KEY (MaUser) REFERENCES Users(MaNV)
                );";
                cmd.ExecuteNonQuery();
            }
        }


        // ------------------ CREATE ------------------ //
        public static void InsertOrder(HoaDon hd)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
            INSERT INTO HoaDon 
            (MaHD,NgayLapHD, TongTienHD, GiamGiaHD, MaKH, MaUser, SoHD, TrangThaiHD)
            VALUES ($MaHD, $NgayLapHD, $TongTienHD, $GiamGiaHD, $MaKH, $MaUser, $SoHD, $TrangThaiHD);
        ";
                cmd.Parameters.AddWithValue("$MaHD", hd.MaHD);
                cmd.Parameters.AddWithValue("$NgayLapHD", hd.NgayLapHD.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("$TongTienHD", (double)hd.TongTienHD);
                cmd.Parameters.AddWithValue("$GiamGiaHD", (double)hd.GiamGiaHD);
                cmd.Parameters.AddWithValue("$MaKH", hd.MaKH);
                cmd.Parameters.AddWithValue("$MaUser", hd.MaUser);
                cmd.Parameters.AddWithValue("$SoHD", hd.SoHD);
                cmd.Parameters.AddWithValue("$TrangThaiHD", hd.TrangThaiHD);

                cmd.ExecuteNonQuery();
                
                // Gửi message thông báo đã thêm hóa đơn
                WeakReferenceMessenger.Default.Send(new HoaDonChangedMessage("Insert"));
            }
        }

        //So   HD tự động tăng
        public static int GetNextOrderNumber()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT IFNULL(MAX(SoHD), 0) FROM HoaDon;";
                int maxSoHD = Convert.ToInt32(cmd.ExecuteScalar());
                return maxSoHD + 1;
            }
        }


        // ------------------ READ ALL ------------------ //
        public static List<HoaDon> GetAllOrder()
        {
            var hoaDons = new List<HoaDon>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT 
                    h.MaHD, h.NgayLapHD, h.TongTienHD, h.GiamGiaHD, 
                    h.MaKH, h.MaUser, h.SoHD, h.TrangThaiHD,
                    k.TenKH, k.SDT, k.DiaChi, k.GioiTinh, k.Hang, k.GhiChu, k.TongMua,
                    u.HoTen as TenUser
                FROM HoaDon h
                LEFT JOIN KhachHang k ON h.MaKH = k.MaKH
                LEFT JOIN Users u ON h.MaUser = u.MaNV
                ORDER BY h.MaHD DESC;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var hd = new HoaDon
                        {
                            MaHD = reader.GetString(0),
                            NgayLapHD = DateTime.Parse(reader.GetString(1)),
                            TongTienHD = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                            GiamGiaHD = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            MaKH = reader.GetString(4),
                            MaUser = reader.GetString(5),
                            SoHD = reader.GetInt32(6),
                            TrangThaiHD = reader.GetString(7),
                            // Load thông tin KhachHang
                            KhachHang = reader.IsDBNull(8) ? null : new KhachHang
                            {
                                MaKH = reader.GetString(4),
                                TenKH = reader.GetString(8),
                                SDT = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                DiaChi = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                GioiTinh = reader.IsDBNull(11) ? "" : reader.GetString(11),
                                Hang = reader.IsDBNull(12) ? "" : reader.GetString(12),
                                GhiChu = reader.IsDBNull(13) ? "" : reader.GetString(13),
                                TongMua = reader.IsDBNull(14) ? 0 : reader.GetDecimal(14)
                            },
                            TenUser = reader.IsDBNull(15) ? "" : reader.GetString(15)
                        };
                        hoaDons.Add(hd);
                    }
                }
            }

            return hoaDons;
        }

        // ------------------ READ ONE ------------------ //
        public static HoaDon? GetOrderById(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT 
                    h.MaHD, h.NgayLapHD, h.TongTienHD, h.GiamGiaHD, 
                    h.MaKH, h.MaUser, h.SoHD, h.TrangThaiHD,
                    k.TenKH, k.SDT, k.DiaChi, k.GioiTinh, k.Hang, k.GhiChu, k.TongMua,
                    u.HoTen as TenUser
                FROM HoaDon h
                LEFT JOIN KhachHang k ON h.MaKH = k.MaKH
                LEFT JOIN Users u ON h.MaUser = u.MaNV
                WHERE h.MaHD = $MaHD";
                cmd.Parameters.AddWithValue("$MaHD", maHD);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new HoaDon
                        {
                            MaHD = reader.GetString(0),
                            NgayLapHD = DateTime.Parse(reader.GetString(1)),
                            TongTienHD = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                            GiamGiaHD = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                            MaKH = reader.GetString(4),
                            MaUser = reader.GetString(5),
                            SoHD = reader.GetInt32(6),
                            TrangThaiHD = reader.GetString(7),
                            // Load thông tin KhachHang
                            KhachHang = reader.IsDBNull(8) ? null : new KhachHang
                            {
                                MaKH = reader.GetString(4),
                                TenKH = reader.GetString(8),
                                SDT = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                DiaChi = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                GioiTinh = reader.IsDBNull(11) ? "" : reader.GetString(11),
                                Hang = reader.IsDBNull(12) ? "" : reader.GetString(12),
                                GhiChu = reader.IsDBNull(13) ? "" : reader.GetString(13),
                                TongMua = reader.IsDBNull(14) ? 0 : reader.GetDecimal(14)
                            },
                            TenUser = reader.IsDBNull(15) ? "" : reader.GetString(15)
                        };
                    }
                }
            }

            return null;
        }

        // ------------------ UPDATE ------------------
        public static void UpdateOrder(HoaDon hd)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                UPDATE HoaDon SET
                    NgayLapHD = $NgayLapHD,
                    TongTienHD = $TongTienHD,
                    GiamGiaHD = $GiamGiaHD,
                    MaKH = $MaKH,
                    MaUser = $MaUser,
                    SoHD = $SoHD,
                    TrangThaiHD = $TrangThaiHD
                WHERE MaHD = $MaHD;";

                cmd.Parameters.AddWithValue("$NgayLapHD", hd.NgayLapHD.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("$TongTienHD", (double)hd.TongTienHD);
                cmd.Parameters.AddWithValue("$GiamGiaHD", (double)hd.GiamGiaHD);
                cmd.Parameters.AddWithValue("$MaKH", hd.MaKH);
                cmd.Parameters.AddWithValue("$MaUser", hd.MaUser);
                cmd.Parameters.AddWithValue("$SoHD", hd.SoHD);
                cmd.Parameters.AddWithValue("$TrangThaiHD", hd.TrangThaiHD);
                cmd.Parameters.AddWithValue("$MaHD", hd.MaHD);

                cmd.ExecuteNonQuery();
                
                // Gửi message thông báo đã cập nhật hóa đơn
                WeakReferenceMessenger.Default.Send(new HoaDonChangedMessage("Update"));
            }
        }

        // ------------------ DELETE ------------------
        public static void DeleteOrder(string maHD)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM HoaDon WHERE MaHD = $MaHD";
                cmd.Parameters.AddWithValue("$MaHD", maHD);
                cmd.ExecuteNonQuery();
                
                // Gửi message thông báo đã xóa hóa đơn
                WeakReferenceMessenger.Default.Send(new HoaDonChangedMessage("Delete"));
            }
        }

        //ID
        public static string GenerateNewOrderID()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaHD FROM HoaDon ORDER BY MaHD DESC LIMIT 1";
                var result = cmd.ExecuteScalar()?.ToString();

                if (string.IsNullOrEmpty(result))
                    return "HD001";

                int number = int.Parse(result.Substring(2));
                return $"HD{(number + 1):D3}";
            }
        }
        //Đếm hoá đơn 
        public static int CountOrder()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM HoaDon";
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
        //Tổng tiền hôm nay
        public static decimal GetTodayToTalIncome()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT IFNULL(SUM(TongTienHD), 0) 
                    FROM HoaDon 
                    WHERE date(NgayLapHD) = date('now') 
                    AND TrangThaiHD = 'Đã thanh toán'";
                var result = cmd.ExecuteScalar();
                var total = result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
                System.Diagnostics.Debug.WriteLine($"[Service] GetTongTienHomNay() = {total}");
                return total;
            }
        }

        //Tổng tiền tháng , năm nay

        public static decimal GetToTalIncomeMonthInYear(int thang, int nam)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT IFNULL(SUM(TongTienHD), 0) 
                    FROM HoaDon 
                    WHERE month(NgayLapHD) = month(thang) && year(NgayLapHD) = year(nam) 
                    AND TrangThaiHD = 'Đã thanh toán'";
                var result = cmd.ExecuteScalar();
                var total = result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
                System.Diagnostics.Debug.WriteLine($"[Service] GetTongTienThangNam() = {total}");
                return total;
            }
        }
    }
}
