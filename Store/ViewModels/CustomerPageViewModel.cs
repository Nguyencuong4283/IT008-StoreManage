using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Store.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Store.ViewModels;

public partial class CustomerPageViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<KhachHang> khachHangs = new();
    private DispatcherTimer? _timer;
    
    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
        {
            "TenKH",
            "SDT",
            "DiaChi"
        };
    
    public CustomerPageViewModel()
    {
        LoadKhachHangs();
        StartAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        // Tạo timer lặp lại mỗi 5 giây
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _timer.Tick += (s, e) => LoadKhachHangs();
        _timer.Start();
    }

    public void StopAutoRefresh()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer = null;
        }
    }

    private void LoadKhachHangs()
    {
        try
        {
            var list = KhachHangService.GetAllKhachHang();

            khachHangs.Clear();
            foreach (var kh in list)
            {
                khachHangs.Add(kh);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi load khách hàng: {ex.Message}");
        }
    }

    public void RefreshData()
    {
        LoadKhachHangs();
    }
    [RelayCommand]
    public void TaoKhachHangButton()
    {
       CreateCustomerWindowView createCustomerWindowView = new CreateCustomerWindowView();
       createCustomerWindowView.Show();
    }
    [RelayCommand]
    public void ChiTietButtonCommand(KhachHang khachHang)
    {
        if (khachHang == null) return;
        var detailWindow = new CustomerDetailWindowView
        {
            DataContext = new CustomerDetailWindowViewModel(khachHang)
        };
        detailWindow.Show();
    }
    //private void XemChiTietSanPham(SanPham sanPham)
    //{
    //    if (sanPham == null) return;

    //    var detailWindow = new ProductDetailWindowView
    //    {
    //        DataContext = new ProductDetailWindowViewModel(sanPham)
    //    };
    //    detailWindow.Show();
    //}
}