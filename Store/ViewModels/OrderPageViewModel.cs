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


public partial class OrderPageViewModel : ViewModelBase, 
    IRecipient<HoaDonChangedMessage>
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
        WeakReferenceMessenger.Default.Register<HoaDonChangedMessage>(this);
        LoadHoaDons();
    }

    // Xử lý khi nhận message HoaDon thay đổi
    public void Receive(HoaDonChangedMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"[HomePageViewModel] Nhận message: HoaDon {message.Action}");
        LoadHoaDons();
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
    [RelayCommand]
    private void CreateBillButton()
    {
        CreateBillWindowView createBillWindow = new CreateBillWindowView();
        createBillWindow.Show();
    }
    [RelayCommand]
    private void ChiTietButton(HoaDon hoaDon)
    {
        if(hoaDon == null) return;
        var detailWindow = new BillDetailWindowView
        {
            DataContext = new BillDetailWindowViewModel(hoaDon)
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