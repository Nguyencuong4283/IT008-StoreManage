using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Helpers;
using Store.Models;
using Store.Services;
using Store.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Store.ViewModels.Bill;


public partial class OrderPageViewModel : ViewModelBase, 
    IRecipient<HoaDonChangedMessage>
{
    private List<HoaDon> allOrder = new();
    [ObservableProperty] private int soHD;
    [ObservableProperty] private string tenKH;
    [ObservableProperty] private DateTime  ngayLapHD;
    [ObservableProperty] private decimal tongTienHD;
    
    [ObservableProperty]
    private ObservableCollection<HoaDon> hoaDons = new();
    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
    { "Tất cả",
     "TenKH",
     "NgayLapHD",
     "TongTienHD"
    };


    

    [ObservableProperty] private string searchKeyword;
    
    [ObservableProperty] private string _selectedFilterBy = "Tất cả";
    
    public OrderPageViewModel()
    {
        WeakReferenceMessenger.Default.Register<HoaDonChangedMessage>(this);
        LoadHoaDons();
    }

    // Xử lý khi nhận message HoaDon thay đổi
    public void Receive(HoaDonChangedMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"[OrderPageViewModel] Nhận message: HoaDon {message.Value}");
        LoadHoaDons();
    }
    private void LoadHoaDons()
    {
        var list = OrderService.GetAllOrder();

        allOrder = list;

        hoaDons.Clear();
        foreach (var kh in list)
        {
            hoaDons.Add(kh);
        }
    }
    [RelayCommand]
    private void CreateBillButton()
    {
        WindowManager.ShowCreateBillWindow();
    }
    [RelayCommand]
    private void ChiTietButton(HoaDon hoaDon)
    {
        if(hoaDon == null) return;
        WindowManager.ShowBillDetailWindow(hoaDon);

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
    
    //===== Tìm kiếm và lọc đơn hàng =====//
    partial void OnSearchKeywordChanged(string value)
    {
        FilterOrders();
    }
    
    partial void OnSelectedFilterByChanged(string value)
    {
        FilterOrders();
    }
    
    private void FilterOrders()
    {
        // Nếu danh sách gốc chưa có dữ liệu thì thoát
        if (allOrder == null) return;

        IEnumerable<HoaDon> query = allOrder;

        // Xử lý tìm kiếm
        if (!string.IsNullOrWhiteSpace(searchKeyword))
        {
            string keyword = searchKeyword.ToLower().Trim();

            // Tìm kiếm đa năng: Số HĐ hoặc Tên Khách Hàng
            query = query.Where(x => 
                    x.SoHD.ToString().Contains(keyword) || 
                    (x.TenKH != null && x.TenKH.ToLower().Contains(keyword)) ||
                    (x.TenUser != null && x.TenUser.ToLower().Contains(keyword)) 
            );
        }

        // Cập nhật lại danh sách hiển thị
        HoaDons = new ObservableCollection<HoaDon>(query);
    }
    
}