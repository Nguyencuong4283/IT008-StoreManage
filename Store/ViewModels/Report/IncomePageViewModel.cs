using System;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Store.Messages;
using Store.Services;

namespace Store.ViewModels.Report;

public partial class IncomePageViewModel : ViewModelBase, IRecipient<HoaDonChangedMessage>
{
    [ObservableProperty] private int totalOrders;
    [ObservableProperty] private double totalIncome;

    int _currentYear = DateTime.Now.Year;
    
    private readonly ObservableCollection<double> _values;
    public ISeries[] Series { get; set; }
    public Axis[] XAxis { get; set; }
    public Axis[] YAxis { get; set; }
    public IncomePageViewModel()
    {
        //===== Nhận thông báo cập nhật dữ liệu =====//
        WeakReferenceMessenger.Default.Register<HoaDonChangedMessage>(this) ;
        
        //===== Cấu hình biểu đồ =====//
        _values = new ObservableCollection<double>();
        
        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = _values,
                GeometrySize = 5,
                LineSmoothness = 0
            }
        };

        XAxis =
        [
            new Axis
            {
                Name = $"Năm {_currentYear}",
                NameTextSize = 15,
                Labels = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"]
            }
        ];

        YAxis =
        [
            new Axis
            {
                Name = "Doanh thu (VNĐ)",
                NameTextSize = 15,
                Labeler = value => value.ToString("N0")
            }
        ];

        LoadData(_currentYear);
        
    }

    //===== Tải dữ liệu =====//
    private void LoadData(int year)
    {
            var list = IncomeService.GetMonthlyIncome(year);
            
            _values.Clear();
            foreach (var m in list)
            {
                _values.Add(m);
            }
    }
    
    //===== Xử lý khi nhận message HoaDon thay đổi =====//
    public void Receive(HoaDonChangedMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"[IncomePageViewModel] Nhận message: HoaDon {message}");
        LoadData(_currentYear);
        OnTotalIncomeChanged(totalIncome);
        OnTotalOrdersChanged(totalOrders);
    }
    
    //===== Hiển thị tổng thu nhập hiện tại =====//
    partial void OnTotalIncomeChanged(double value)
    {
        var income = IncomeService.Monthly_Stat(_currentYear);
        totalIncome = (double)income.TotalIncome;
    }
    
    //===== Hiển thị tổng số đơn hàng đã thanh toán =====//
    partial void OnTotalOrdersChanged(int value)
    {
        var income = IncomeService.Monthly_Stat(_currentYear);
        totalOrders = income.TotalOrders;
    }
}