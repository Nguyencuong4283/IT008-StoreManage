using System;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Store.Services;

namespace Store.ViewModels;

public class IncomePageViewModel : ViewModelBase
{
    int currentYear = DateTime.Now.Year;
    //====== Biểu đồ thống kê thu nhập theo năm ======//
    private readonly ObservableCollection<double> _values;
    public ISeries[] _series { get; set; }
    public Axis[] xAxis { get; set; }
    public Axis[] yAxis { get; set; }
    public IncomePageViewModel()
    {
        _values = new ObservableCollection<double>();
        
        _series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = _values,
                GeometrySize = 5,
                LineSmoothness = 0
            }
        };

        xAxis =
        [
            new Axis
            {
                Name = $"Năm {currentYear}",
                NameTextSize = 10,
                Labels = [$"Tháng 1/{currentYear}" ,
                          $"Tháng 2/{currentYear}" ,
                          $"Tháng 3/{currentYear}" ,
                          $"Tháng 4/{currentYear}" ,
                          $"Tháng 5/{currentYear}" ,
                          $"Tháng 6/{currentYear}" ,
                          $"Tháng 7/{currentYear}" ,
                          $"Tháng 8/{currentYear}" ,
                          $"Tháng 9/{currentYear}" ,
                          $"Tháng 10/{currentYear}" ,
                          $"Tháng 11/{currentYear}" ,
                          $"Tháng 12/{currentYear}"],
            }
        ];

        yAxis =
        [
            new Axis
            {
                Name = "Doanh thu (VNĐ)",
                NameTextSize = 10,
                Labeler = value => value.ToString("N0")
            }
        ];

        LoadData(currentYear);
        
    }

    private void LoadData(int year)
    {
            var monthlyData = IncomeData.GetMonthlyIncome(year);

            _values.Clear();
            foreach (var m in monthlyData)
            {
                _values.Add(m);
            }
    }
    
    //===== Hiển thị tổng thu nhập hiện tại =====//
    public double TongThuNhap
    {
        get
        {
            var income = IncomeData.Monthly_Stat(currentYear);
            return income.TotalIncome;
        }
    }
    
    //===== Hiển thị tổng số đơn hàng đã thanh toán =====//
    public double TongDonHang
    {
        get
        {
            var orders = IncomeData.Monthly_Stat(currentYear);
            return orders.TotalOrders;
        }
    }
}