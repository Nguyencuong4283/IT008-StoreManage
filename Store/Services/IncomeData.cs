using Microsoft.Data.Sqlite;
using Store.Models;
using Store.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1.Cmp;

namespace Store.Services;

public static class IncomeData
{
    private static readonly string dbPath = Path.Combine(AppContext.BaseDirectory, "store.db");
    
    //===== Lấy dữ liệu doanh thu mỗi tháng =====//
    public static double[] GetMonthlyIncome(int year)
    {
        double[] MonthlyIncomeData = new double[12];

        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT strftime('%m',NgayLapHD) as MONTH,
                   SUM(TongTienHD) as TOTAL
            FROM HoaDon
            WHERE strftime('%Y', NgayLapHD) = @Year
            AND TrangThaiHD = 'Đã thanh toán'
            GROUP BY MONTH";
            
            command.Parameters.AddWithValue("@Year", year.ToString());
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(reader.IsDBNull(0))
                        continue;
                    double TOTAL = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);

                    if (int.TryParse(reader.GetString(0), out int month))
                    {
                        int monthIndex = month - 1;

                        if (monthIndex >= 0 && monthIndex < 12)
                        {
                            MonthlyIncomeData[monthIndex] = TOTAL;
                        }
                    }
                }
            }
        }
        return MonthlyIncomeData;
    }
    
    //===== Lấy dữ liệu tổng doanh thu và số đơn hàng mỗi tháng=====//
    public class Monthyly_statistics
    {
        public double TotalIncome { get; set; }
        public int TotalOrders { get; set; }
    }
    public static Monthyly_statistics Monthly_Stat(int year)
    {
        Monthyly_statistics Ms = new Monthyly_statistics();
        
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"
            SELECT strftime('%m',NgayLapHD) as MONTH,
                   SUM(TongTienHD) as TOTAL_INCOME,
                   COUNT(*) as TOTAL_ORDERS
            FROM HoaDon
            WHERE strftime('%Y', NgayLapHD) = @Year
            AND strftime('%m', NgayLapHD) = @Month
            AND TrangThaiHD = 'Đã thanh toán'
            GROUP BY MONTH";
            
            command.Parameters.AddWithValue("@Year",  year.ToString());
            command.Parameters.AddWithValue("@Month", DateTime.Now.Month.ToString());
            
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(reader.IsDBNull(0))
                        continue;
                    Ms.TotalIncome = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                    Ms.TotalOrders = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                }
            }
        }

        return Ms;
    }
    
}