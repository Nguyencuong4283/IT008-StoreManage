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
                   SUM(TongTienHD) as TOTAL,
                   COUNT(*) as ORDER_COUNT
            FROM HoaDon
            WHERE strftime('%Y', NgayLapHD) = @Year
            AND TrangThaiHD = 'Da thanh toan'
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
    
    //===== Lấy dữ liệu tổng doanh thu đến thời điểm hiện tại =====//
    public static double GetTotalIncome(int year)
    {
        var mIncomeData = GetMonthlyIncome(year);
        double TotalIncome = 0;
        
        foreach (var Income in mIncomeData)
        {
            TotalIncome += Income;
        }
        
        return TotalIncome;
    }
    
    //===== Lấy dữ liệu tổng số đơn hàng đã thanh toán đến thời điểm hiện tại =====//
    public static double GetTotalOrder(int year, int month)
    {
        var mOrderData = GetMonthlyIncome(year);
        double mTotalOrders = 0;

        foreach (var order in mOrderData)
        {
            mTotalOrders += order;
        }
        
        return mTotalOrders;
    }

    
}