using Microsoft.Data.Sqlite;
using Store.Models;
using Store.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace Store.Services
{
    public static class ProductService 
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
                
                // Check if table exists and needs migration
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT sql FROM sqlite_master WHERE type='table' AND name='SanPham'";
                var existingSchema = checkCmd.ExecuteScalar()?.ToString();
                
                if (!string.IsNullOrEmpty(existingSchema) && existingSchema.Contains("HinhAnhDuongDan TEXT NOT NULL"))
                {
                    // Migrate: recreate table without NOT NULL constraint on HinhAnhDuongDan
                    Console.WriteLine("Migrating SanPham table schema...");
                    var migrateCmd = connection.CreateCommand();
                    migrateCmd.CommandText = @"
                        BEGIN TRANSACTION;
                        
                        CREATE TABLE SanPham_new (
                            MaSP TEXT PRIMARY KEY,
                            TenSP TEXT NOT NULL,
                            GiaSP REAL NOT NULL,
                            SoLuongSP INTEGER NOT NULL,
                            HinhAnhDuongDan TEXT,
                            KichThuocSP TEXT NOT NULL,
                            LoaiSP TEXT NOT NULL,
                            MoTaSP TEXT,
                            IsDelete INTEGER DEFAULT 0
                        );
                        
                        INSERT INTO SanPham_new (MaSP, TenSP, GiaSP, SoLuongSP, HinhAnhDuongDan, KichThuocSP, LoaiSP, MoTaSP, IsDelete)
                        SELECT MaSP, TenSP, GiaSP, SoLuongSP, HinhAnhDuongDan, KichThuocSP, LoaiSP, MoTaSP, IsDelete
                        FROM SanPham;
                        
                        DROP TABLE SanPham;
                        
                        ALTER TABLE SanPham_new RENAME TO SanPham;
                        
                        COMMIT;
                    ";
                    migrateCmd.ExecuteNonQuery();
                    Console.WriteLine("Migration completed.");
                }
                else
                {
                    // Create table if it doesn't exist
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SanPham (
                        MaSP TEXT PRIMARY KEY,
                        TenSP TEXT NOT NULL,
                        GiaSP REAL NOT NULL,
                        SoLuongSP INTEGER NOT NULL,
                        HinhAnhDuongDan TEXT,
                        KichThuocSP TEXT NOT NULL,
                        LoaiSP TEXT NOT NULL,
                        MoTaSP TEXT,
                        IsDelete INTEGER DEFAULT 0
                    );";
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //CRUD
        //Create
        public static void InsertProduct(SanPham sp)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                string newMaSP = GenerateNewProductID();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO SanPham 
                (MaSP, TenSP, GiaSP, SoLuongSP, HinhAnhDuongDan, KichThuocSP, LoaiSP, MoTaSP, IsDelete)
                VALUES ($MaSP, $TenSP, $GiaSP, $SoLuongSP, $HinhAnhDuongDan, $KichThuocSP, $LoaiSP, $MoTaSP, $IsDelete)";
                cmd.Parameters.AddWithValue("$MaSP", newMaSP);
                cmd.Parameters.AddWithValue("$TenSP", sp.TenSP);
                cmd.Parameters.AddWithValue("$GiaSP", (double)sp.GiaSP);
                cmd.Parameters.AddWithValue("$SoLuongSP", sp.SoLuongSP);
                cmd.Parameters.AddWithValue("$HinhAnhDuongDan", sp.HinhAnhDuongDan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$KichThuocSP", sp.KichThuocSP);
                cmd.Parameters.AddWithValue("$LoaiSP", sp.LoaiSP);
                cmd.Parameters.AddWithValue("$MoTaSP", sp.MoTaSP ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$IsDelete", 0);

                cmd.ExecuteNonQuery();
            }
        }

        //Read one
        public static SanPham GetProduct(string MaSP)
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
        public static int CountProduct()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM SanPham WHERE IsDelete = 0";
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
        //Read All
        public static List<SanPham> GetAllProduct()
        {
            var sanPhams = new List<SanPham>();

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaSP, TenSP, GiaSP, SoLuongSP, LoaiSP, KichThuocSP, MoTaSP, HinhAnhDuongDan, IsDelete FROM SanPham";

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
                            IsDelete = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)

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
                        if (sp.IsDelete == 0)
                        {
                            sanPhams.Add(sp);
                        }
                    }
                }
            }

            return sanPhams;
        }
        //Update
        public static void UpdateProduct(SanPham sp)
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
                    MoTaSP = $MoTaSP,
                    IsDelete = $IsDelete
                WHERE MaSP = $MaSP";
                cmd.Parameters.AddWithValue("$MaSP", sp.MaSP);
                cmd.Parameters.AddWithValue("$TenSP", sp.TenSP);
                cmd.Parameters.AddWithValue("$GiaSP", (double)sp.GiaSP);
                cmd.Parameters.AddWithValue("$SoLuongSP", sp.SoLuongSP);
                cmd.Parameters.AddWithValue("$HinhAnhDuongDan", sp.HinhAnhDuongDan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$KichThuocSP", sp.KichThuocSP);
                cmd.Parameters.AddWithValue("$LoaiSP", sp.LoaiSP);
                cmd.Parameters.AddWithValue("$MoTaSP", sp.MoTaSP ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$IsDelete", sp.IsDelete);
                cmd.ExecuteNonQuery();
            }
        }

        // Delete
        public static void DeleteProduct(string maSP)
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
        public static string GenerateNewProductID()
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
        //Tìm kiếm ds bộ lọc 
        //
        // public static List<SanPham> GetSearchSanPham(string boLoc)
        // {
        //     var sanPhams = new List<SanPham>();
        //
        //     using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        //     {
        //         connection.Open();
        //         var cmd = connection.CreateCommand();
        //         cmd.CommandText = "SELECT MaSP, TenSP, GiaSP, SoLuongSP, LoaiSP, KichThuocSP, MoTaSP, HinhAnhDuongDan FROM SanPham";
        //
        //         using (var reader = cmd.ExecuteReader())
        //         {
        //             while (reader.Read() && reader.GetString(4) == boLoc)
        //             {
        //                 var sp = new SanPham
        //                 {
        //                     MaSP = reader.GetString(0),
        //                     TenSP = reader.IsDBNull(1) ? "" : reader.GetString(1),
        //                     GiaSP = reader.IsDBNull(2) ? 1000 : reader.GetDecimal(2),
        //                     SoLuongSP = reader.IsDBNull(3) ? 1 : reader.GetInt32(3),
        //                     LoaiSP = reader.IsDBNull(4) ? "" : reader.GetString(4),
        //                     KichThuocSP = reader.IsDBNull(5) ? "" : reader.GetString(5),
        //                     MoTaSP = reader.IsDBNull(6) ? "" : reader.GetString(6),
        //                 };
        //
        //                 if (!reader.IsDBNull(7))
        //                 {
        //                     try
        //                     {
        //                         string imagePath = reader.GetString(7);
        //                         if (File.Exists(imagePath))
        //                             sp.HinhAnhSP = new Bitmap(imagePath);
        //                     }
        //                     catch { }
        //                 }
        //
        //                 sanPhams.Add(sp);
        //             }
        //         }
        //     }
        //
        //     return sanPhams;
        // }
    }
}
