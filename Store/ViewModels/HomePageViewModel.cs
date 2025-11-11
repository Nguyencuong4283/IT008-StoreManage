using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Store.Views;
using System;
using System.Collections.ObjectModel;


namespace Store.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{
    [ObservableProperty] private int soKhachHang = KhachHangService.CountKhachHang();
    [ObservableProperty] private int soSanPham = SanPhanService.CountSanPham();
    [RelayCommand]
    private void TaoDonButton()
    {
        CreateBillWindowView createBillWindow = new CreateBillWindowView();
        createBillWindow.Show();
    }
    [RelayCommand]
    private void ThemSanPhamButton()
    {
        CreateProductWindowView createProductWindowView = new CreateProductWindowView();
        createProductWindowView.Show();
        SoSanPham = SanPhanService.CountSanPham();
    }
    [RelayCommand]
    private void ThemKhachHangButton()
    {
        CreateCustomerWindowView createCustomerWindowView = new CreateCustomerWindowView();
        createCustomerWindowView.Show();
        // Sau khi thêm xong, cập nhật lại số lượng
        SoKhachHang = KhachHangService.CountKhachHang();
    }
}
/*using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Views;
using System;
using System.Collections.ObjectModel;

namespace Store.ViewModels;


public partial class BillPageViewModel : ViewModelBase
{
    [ObservableProperty] private int soHD;
    [ObservableProperty] private string tenKH;
    [ObservableProperty] private DateTime  ngayLapHD;
    [ObservableProperty] private decimal tongTienHD;
    [ObservableProperty]
    private ObservableCollection<HoaDon> hoaDons = new();
    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
 {
     "TenKH",
     "NgayLapHD",
     "TongTienHD",
     "Tất cả"
 };
    public BillPageViewModel()
    {
       
    }
    [RelayCommand]
    private void CreateBillButton()
    {
        CreateBillWindowView createBillWindow = new CreateBillWindowView();
        createBillWindow.Show();
    }
}*/