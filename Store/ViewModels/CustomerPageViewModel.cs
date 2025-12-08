using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using Store.Views;

namespace Store.ViewModels;

public partial class CustomerPageViewModel : ViewModelBase, IRecipient<KhachHangChangedMessage>
{
    [ObservableProperty] private ObservableCollection<KhachHang> khachHangs = new();
    [ObservableProperty] private string searchKeyword;
    [ObservableProperty] private string _selectedFilter = "Tất cả";
    private List<KhachHang> _allKhachHangs = new();

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
        LoadKhachHangs();
    }
    public void Receive(KhachHangChangedMessage message)
    {
        Debug.WriteLine($"[CustomerPageViewModel] Nhận message: KhachHang {message.Value}");
        // Đảm bảo cập nhật trên UI thread
        Dispatcher.UIThread.Post(() =>
        {
            LoadKhachHangs();
        });
    }
    private void LoadKhachHangs()
    {
        var list = KhachHangService.GetAllKhachHang();
        _allKhachHangs = list;
        UpdateCustomerList(_allKhachHangs);
    }

    public void RefreshData()
    {
        LoadKhachHangs();
    }

    [RelayCommand]
    public void TaoKhachHangButton()
    {
        var createCustomerWindowView = new CreateCustomerWindowView();
        createCustomerWindowView.Show();
    }

    [RelayCommand]
    public void ChiTietButtonCommand(KhachHang khachHang)
    {
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
        var allCustomers = KhachHangService.GetAllKhachHang();
        var filterList = allCustomers;

        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            switch (SelectedFilter)
            {
                // Tìm tên khách hàng
                case "Tên khách hàng":
                    filterList = allCustomers.FindAll(kh =>
                        kh.TenKH != null && kh.TenKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm mã khách hàng
                case "Mã khách hàng":
                    filterList = allCustomers.FindAll(kh =>
                        kh.MaKH != null && kh.MaKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm số điện thoại
                case "Số điện thoại":
                    filterList = allCustomers.FindAll(kh =>
                        kh.SDT != null && kh.SDT.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm tất cả
                default:
                    filterList = allCustomers.FindAll(kh =>
                        (kh.TenKH != null &&
                         kh.TenKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (kh.MaKH != null && kh.MaKH.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (kh.SDT != null && kh.SDT.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
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