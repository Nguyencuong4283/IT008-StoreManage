using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Models;
using Store.Services;
using Store.Messages;
using Store.Views;
using System;
using System.Collections.ObjectModel;


namespace Store.ViewModels;

public partial class HomePageViewModel : ViewModelBase, 
    IRecipient<HoaDonChangedMessage>,
    IRecipient<SanPhamChangedMessage>,
    IRecipient<KhachHangChangedMessage>
{
    [ObservableProperty] private int soKhachHang;
    [ObservableProperty] private int soSanPham;
    [ObservableProperty] private int soHoaDon;
    [ObservableProperty] private decimal doanhThuHomNay;


    [ObservableProperty] private int soHD;
    [ObservableProperty] private string tenKH;
    [ObservableProperty] private DateTime ngayLapHD;
    [ObservableProperty] private decimal tongTienHD;
    [ObservableProperty]
    private ObservableCollection<HoaDon> hoaDons = new();
    public HomePageViewModel()
    {
        // Đăng ký nhận message
        WeakReferenceMessenger.Default.Register<HoaDonChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<SanPhamChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<KhachHangChangedMessage>(this);
        
        // Load dữ liệu async để tránh đơ UI
        LoadStatistics();
        LoadHoaDons();
    }
    
    // Xử lý khi nhận message HoaDon thay đổi
    public void Receive(HoaDonChangedMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"[HomePageViewModel] Nhận message: HoaDon {message.Action}");
        LoadStatistics();
    }
    
    // Xử lý khi nhận message SanPham thay đổi
    public void Receive(SanPhamChangedMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"[HomePageViewModel] Nhận message: SanPham {message.Action}");
        LoadStatistics();
    }
    
    // Xử lý khi nhận message KhachHang thay đổi
    public void Receive(KhachHangChangedMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"[HomePageViewModel] Nhận message: KhachHang {message.Action}");
        LoadStatistics();
    }
    
    private async void LoadStatistics()
    {
        try
        {
            // Chạy các query trong background thread
            await System.Threading.Tasks.Task.Run(() =>
            {
                SoKhachHang = KhachHangService.CountKhachHang();
                SoSanPham = SanPhanService.CountSanPham();
                SoHoaDon = HoaDonService.CountHoaDon();
                DoanhThuHomNay = HoaDonService.GetTongTienHomNay();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomePageViewModel] Lỗi load statistics: {ex.Message}");
        }
    }
    
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
    private void LoadHoaDons()
    {
        var list = HoaDonService.GetAllHoaDon();

        hoaDons.Clear();
        foreach (var kh in list)
        {
            hoaDons.Add(kh);
        }
    }
}
/*using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Views;
using System;
using System.Collections.ObjectModel;

namespace Store.ViewModels;


public partial class OrderPageViewModel : ViewModelBase
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
    public OrderPageViewModel()
    {
       
    }
    [RelayCommand]
    private void CreateBillButton()
    {
        CreateBillWindowView createBillWindow = new CreateBillWindowView();
        createBillWindow.Show();
    }
}*/