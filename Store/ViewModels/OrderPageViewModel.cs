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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Store.ViewModels;


public partial class OrderPageViewModel : ViewModelBase, IRecipient<HoaDonChangedMessage>
{
    private List<HoaDon> _allOrder = new();
    [ObservableProperty] private int soHD;
    [ObservableProperty] private string tenKH;
    [ObservableProperty] private DateTime  ngayLapHD;
    [ObservableProperty] private decimal tongTienHD;
    
    [ObservableProperty]
    private ObservableCollection<HoaDon> hoaDons = new();

    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
    {
        "Tất cả",
        "Số hóa đơn",
        "Tên khách hàng",
        "Ngày lập hóa đơn",
        "Tổng tiền hóa đơn"
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
        System.Diagnostics.Debug.WriteLine($"[HomePageViewModel] Nhận message: HoaDon {message}");
        // Đảm bảo cập nhật trên UI thread
        Dispatcher.UIThread.Post(() =>
        {
            LoadHoaDons();
        });
    }
    private void LoadHoaDons()
    {
        var list = HoaDonService.GetAllHoaDon();
        _allOrder = list;
        UpdateOrdersList(_allOrder);
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
    
    //===== Tìm kiếm và lọc đơn hàng =====//
    partial void OnSearchKeywordChanged(string value)
    {
        SearchOrders();
    }
    
    partial void OnSelectedFilterByChanged(string value)
    {
        SearchOrders();
    }
    
    private void SearchOrders()
    {
        // Nếu danh sách gốc chưa có dữ liệu thì thoát
        if (_allOrder == null) return;

        IEnumerable<HoaDon> query = _allOrder;

        // Xử lý tìm kiếm
        if (!string.IsNullOrWhiteSpace(searchKeyword))
        {
            string keyword = searchKeyword.ToLower().Trim();
            switch (SelectedFilterBy)
            {
                // Tìm kiếm số hóa đơn
                case "Số hóa đơn":
                    query = query.Where(x => x.SoHD.ToString().Contains(keyword));
                    break;
                
                // Tìm kiếm tên khách hàng
                case "Tên khách hàng":
                    query = query.Where(x => x.KhachHang.TenKH.ToLower().Contains(keyword));
                    break;
                
                // Tìm kiếm ngày lập hóa đơn
                case "Ngày lập hóa đơn":
                    query = query.Where(x => x.NgayLapHD.ToString("dd/MM/yyyy").Contains(keyword));
                    break;
                
                // Tìm kiếm tổng tiền hóa đơn
                case "Tổng tiền hóa đơn":
                    query = query.Where(x => x.TongTienHD.ToString().Contains(keyword));
                    break;
                
                // Tất cả
                default:
                    query = query.Where(x => 
                        x.SoHD.ToString().Contains(keyword) ||
                        x.KhachHang.TenKH.ToLower().Contains(keyword) ||
                        x.NgayLapHD.ToString("dd/MM/yyyy").Contains(keyword) ||
                        x.TongTienHD.ToString().Contains(keyword));
                    break;
            }
        }
        
        // Cập nhật lại danh sách hiển thị
        UpdateOrdersList(query);
    }
    private void UpdateOrdersList(IEnumerable<HoaDon> orders)
    {
        hoaDons.Clear();
        foreach (var hd in orders)
        {
            hoaDons.Add(hd);
        }
    }
    
}