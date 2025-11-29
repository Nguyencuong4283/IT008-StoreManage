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
                NameTextSize = 15,
                Labels = ["Tháng 1" ,"Tháng 2" ,"Tháng 3" ,"Tháng 4" ,"Tháng 5" ,"Tháng 6" ,"Tháng 7" ,"Tháng 8" ,"Tháng 9" ,"Tháng 10" ,"Tháng 11" ,"Tháng 12"],
            }
        ];

        yAxis =
        [
            new Axis
            {
                Name = "Doanh thu (triệu VNĐ)",
                NameTextSize = 15,
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
            var income = IncomeData.GetTotalIncome(currentYear);
            return income;
        }
    }
    
    //===== Hiển thị tổng số đơn hàng đã thanh toán =====//
    public double TongDonHang
    {
        get
        {
            var orders = IncomeData.GetTotalOrder(currentYear, DateTime.Now.Month);
            return orders;
        }
    }
}