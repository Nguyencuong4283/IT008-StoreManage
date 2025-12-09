using Avalonia.Media.Imaging;
using Microsoft.Data.Sqlite;
using Store.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Store.Services
{
    public static class UserService
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
                CREATE TABLE IF NOT EXISTS Users (
                    MaNV TEXT PRIMARY KEY,
                    TenDangNhap TEXT NOT NULL,
                    MatKhau TEXT NOT NULL,
                    HoTen TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    SDT TEXT,
                    DiaChi TEXT NOT NULL,
                    NgaySinh TEXT,
                    GioiTinh TEXT,
                    HinhAnh TEXT NULL,
                    MaVT TEXT NOT NULL,
                    IsDelete INTEGER DEFAULT 0
                );";
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ CREATE ------------------
        public static void InsertUser(User user)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                string newMaNV = GenerateNewMaUser();

                cmd.CommandText = @"
                INSERT INTO Users 
                (MaNV, TenDangNhap, MatKhau, HoTen, Email, SDT, DiaChi, NgaySinh, GioiTinh, HinhAnh, MaVT, IsDelete)
                VALUES ($MaNV, $TenDangNhap, $MatKhau, $HoTen, $Email, $SDT, $DiaChi, $NgaySinh, $GioiTinh, $HinhAnh, $MaVT, $IsDelete)";

                cmd.Parameters.AddWithValue("$MaNV", newMaNV);
                cmd.Parameters.AddWithValue("$TenDangNhap", user.TenDangNhap);
                cmd.Parameters.AddWithValue("$MatKhau", PasswordHelper.HashPassword(user.MatKhau));
                cmd.Parameters.AddWithValue("$HoTen", user.HoTen);
                cmd.Parameters.AddWithValue("$Email", user.Email);
                cmd.Parameters.AddWithValue("$SDT", user.SDT);
                cmd.Parameters.AddWithValue("$DiaChi", user.DiaChi);
                cmd.Parameters.AddWithValue("$NgaySinh", user.NgaySinh?.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("$GioiTinh", user.GioiTinh);
                cmd.Parameters.AddWithValue("$HinhAnh", user.HinhAnh);
                cmd.Parameters.AddWithValue("$MaVT", user.MaVT);
                cmd.Parameters.AddWithValue("$IsDelete", user.IsDelete);

                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ READ ------------------
        public static List<User> GetAllUser()
        {
            var users = new List<User>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaNV, TenDangNhap, MatKhau, HoTen, Email, SDT, DiaChi, NgaySinh, GioiTinh, HinhAnh, MaVT, IsDelete
                FROM Users";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            MaNV = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            TenDangNhap = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MatKhau = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            HoTen = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            SDT = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            DiaChi = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            NgaySinh = reader.IsDBNull(7) ? (DateTime?)null : DateTime.Parse(reader.GetString(7)),
                            GioiTinh = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            HinhAnh = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            MaVT = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            IsDelete = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                        };
                        if(user.IsDelete == 0)
                            users.Add(user);
                    }
                }
            }

            return users;
        }
        public static List<User> GetAllEmployee()
        {
            var users = new List<User>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaNV, TenDangNhap, MatKhau, HoTen, Email, SDT, DiaChi, NgaySinh, GioiTinh, HinhAnh, MaVT, IsDelete
                FROM Users";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            MaNV = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            TenDangNhap = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MatKhau = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            HoTen = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            SDT = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            DiaChi = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            NgaySinh = reader.IsDBNull(7) ? (DateTime?)null : DateTime.Parse(reader.GetString(7)),
                            GioiTinh = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            HinhAnh = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            MaVT = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            IsDelete = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                        };
                        if (user.IsDelete == 0 && user.MaVT == "VT02")
                            users.Add(user);
                    }
                }
            }

            return users;
        }

        //Read one
        public static User GetOneUser(string maNV)
        {
          

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaNV, TenDangNhap, MatKhau, HoTen, Email, SDT, DiaChi, NgaySinh, GioiTinh, HinhAnh, MaVT , IsDelete
                FROM Users";
                User user1 = new User();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) != maNV)
                            continue;

                        else
                        {
                            var user = new User
                            {
                                MaNV = reader.IsDBNull(0) ? "" : reader.GetString(0),
                                TenDangNhap = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                MatKhau = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                HoTen = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                SDT = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                DiaChi = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                NgaySinh = reader.IsDBNull(7) ? (DateTime?)null : DateTime.Parse(reader.GetString(7)),
                                GioiTinh = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                HinhAnh = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                MaVT = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                IsDelete = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                            };
                            user1 = user;
                            return user1;
                        }
                       
                    }
                }
            }
            return null;
        }

        // ------------------ UPDATE ------------------
        public static void UpdateUser(User user, bool updatePassword = false)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                // Nếu không muốn đổi mật khẩu thì bỏ qua cột MatKhau
                cmd.CommandText = updatePassword
                    ? @"
                        UPDATE Users SET
                            TenDangNhap = $TenDangNhap,
                            MatKhau = $MatKhau,
                            HoTen = $HoTen,
                            Email = $Email,
                            SDT = $SDT,
                            DiaChi = $DiaChi,
                            NgaySinh = $NgaySinh,
                            GioiTinh = $GioiTinh,
                            HinhAnh = $HinhAnh,
                            MaVT = $MaVT,
                            IsDelete = $IsDelete
                        WHERE MaNV = $MaNV"
                    : @"
                        UPDATE Users SET
                            TenDangNhap = $TenDangNhap,
                            HoTen = $HoTen,
                            Email = $Email,
                            SDT = $SDT,
                            DiaChi = $DiaChi,
                            NgaySinh = $NgaySinh,
                            GioiTinh = $GioiTinh,
                            HinhAnh = $HinhAnh,
                            MaVT = $MaVT,
                            IsDelete = $IsDelete
                        WHERE MaNV = $MaNV";

                cmd.Parameters.AddWithValue("$MaNV", user.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$TenDangNhap", user.TenDangNhap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$HoTen", user.HoTen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$Email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$SDT", user.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$DiaChi", user.DiaChi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$NgaySinh", user.NgaySinh?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$GioiTinh", user.GioiTinh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$HinhAnh", user.HinhAnh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$MaVT", user.MaVT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$IsDelete", user.IsDelete);

                if (updatePassword)
                {
                    cmd.Parameters.AddWithValue("$MatKhau", PasswordHelper.HashPassword(user.MatKhau));
                }

                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ DELETE ------------------
        public static void DeleteUser(string maNV)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM Users WHERE MaNV = $MaNV";
                cmd.Parameters.AddWithValue("$MaNV", maNV);
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------ AUTO ID ------------------
        public static string GenerateNewMaUser()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaNV FROM Users ORDER BY MaNV DESC LIMIT 1";
                var result = cmd.ExecuteScalar()?.ToString();

                if (string.IsNullOrEmpty(result))
                    return "NV001";

                int number = int.Parse(result.Substring(2));
                return $"NV{(number + 1):D3}";
            }
        }

        // ------------------ PASSWORD HELPER ------------------
        public static class PasswordHelper
        {
            public static string HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = sha256.ComputeHash(bytes);
                    StringBuilder builder = new StringBuilder();
                    foreach (var b in hashBytes)
                        builder.Append(b.ToString("x2"));
                    return builder.ToString();
                }
            }
        }

        // ------------------ VERIFY PASSWORD ------------------
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashOfInput = PasswordHelper.HashPassword(inputPassword);
            return hashOfInput.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }

        // ------------------ GET USER BY EMAIL ------------------
        public static User GetUserByEmail(string email)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                SELECT MaNV, TenDangNhap, MatKhau, HoTen, Email, SDT, DiaChi, NgaySinh, GioiTinh, HinhAnh, MaVT 
                FROM Users
                WHERE Email = $Email";
                cmd.Parameters.AddWithValue("$Email", email);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            MaNV = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            TenDangNhap = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            MatKhau = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            HoTen = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            SDT = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            DiaChi = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            NgaySinh = reader.IsDBNull(7) ? (DateTime?)null : DateTime.Parse(reader.GetString(7)),
                            GioiTinh = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            HinhAnh = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            MaVT = reader.IsDBNull(10) ? "" : reader.GetString(10),
                        };
                    }
                }
            }
            return null;
        }
    }
}
