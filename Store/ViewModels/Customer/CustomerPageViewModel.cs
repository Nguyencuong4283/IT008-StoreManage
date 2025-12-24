using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using Store.Helpers;
using Store.Views.Customer;

namespace Store.ViewModels.Customer;

public partial class CustomerPageViewModel : ViewModelBase, IRecipient<KhachHangChangedMessage>
{
    [ObservableProperty] private ObservableCollection<KhachHang> khachHangs = new();
    [ObservableProperty] private string searchKeyword;
    [ObservableProperty] private string _selectedFilter = "Tất cả";
    private List<KhachHang> _allKhachHangs = new();

    public WindowNotificationManager? NotificationManager { get; set; }

    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
    {
        "Tất cả",
        "Tên khách hàng",
        "Mã khách hàng",
        "Số điện thoại"
    };

    public CustomerPageViewModel()
    {
        WeakReferenceMessenger.Default.Register<KhachHangChangedMessage>(this);
        LoadCustomer();
    }

    public void Receive(KhachHangChangedMessage message)
    {
        Debug.WriteLine($"[CustomerPageViewModel] Nhận message: KhachHang {message.Value}");
        // Đảm bảo cập nhật trên UI thread
        Dispatcher.UIThread.Post(() => { LoadCustomer(); });

        if (message.Value == "Insert")
        {
            NotificationManager?.Show("Thêm khách hàng thành công!", NotificationType.Success);
        }
        else if (message.Value == "Delete")
        {
            NotificationManager?.Show("Xóa khách hàng thành công!", NotificationType.Success);
        }
    }

    private void LoadCustomer()
    {
        var list = CustomerService.GetAllCustomer();
        _allKhachHangs = list;
        UpdateCustomerList(_allKhachHangs);
    }

    [RelayCommand]
    public void CreateCustomerButton()
    {

        WindowManager.ShowCreateCustomerWindow();
    }

    [RelayCommand]
    public void DetailButtonCommand(KhachHang khachHang)
    {
        if(khachHang == null)
        {
            return;
        }
        WindowManager.ShowCustomerDetailWindow(khachHang);
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

    //===== tìm kiếm khách hàng =====//
    partial void OnSearchKeywordChanged(string value)
    {
        SearchCustomers();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        SearchCustomers();
    }

    private void SearchCustomers()
    {
        var allCustomers = CustomerService.GetAllCustomer();
        var filterList = allCustomers;

        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            switch (SelectedFilter)
            {
                // Tìm tên khách hàng
                case "Tên khách hàng":
                    filterList = allCustomers.FindAll(kh =>
                        kh.TenKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm mã khách hàng
                case "Mã khách hàng":
                    filterList = allCustomers.FindAll(kh =>
                        kh.MaKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm số điện thoại
                case "Số điện thoại":
                    filterList = allCustomers.FindAll(kh =>
                        kh.SDT.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm tất cả
                default:
                    filterList = allCustomers.FindAll(kh =>
                        (kh.TenKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (kh.MaKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (kh.SDT.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    );
                    break;
            }

            // Cập nhật danh sách khách hàng hiển thị
            UpdateCustomerList(filterList);
        }
    }

    private void UpdateCustomerList(List<KhachHang> customers)
    {
        KhachHangs.Clear();
        foreach (var kh in customers)
        {
            KhachHangs.Add(kh);
        }
    }
}