using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        ApplyFilter();
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

    //===== Tìm kiếm khách hàng =====//
    partial void OnSearchKeywordChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        IEnumerable<KhachHang> query = _allKhachHangs ?? new List<KhachHang>();

        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            var keyword = SearchKeyword.Trim().ToLower();

            switch (SelectedFilter)
            {
                // Tìm tên khách hàng
                case "Tên khách hàng":
                    query = query.Where(kh => !string.IsNullOrWhiteSpace(kh.TenKH) &&
                                              kh.TenKH.ToLower().Contains(keyword));
                    break;

                // Tìm mã khách hàng
                case "Mã khách hàng":
                    query = query.Where(kh => !string.IsNullOrWhiteSpace(kh.MaKH) &&
                                              kh.MaKH.ToLower().Contains(keyword));
                    break;

                // Tìm số điện thoại
                case "Số điện thoại":
                    query = query.Where(kh => !string.IsNullOrWhiteSpace(kh.SDT) &&
                                              kh.SDT.Contains(keyword));
                    break;

                default:
                    // Tìm tất cả
                    query = query.Where(kh =>
                        (!string.IsNullOrWhiteSpace(kh.TenKH) && kh.TenKH.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrWhiteSpace(kh.MaKH) && kh.MaKH.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrWhiteSpace(kh.SDT) && kh.SDT.Contains(keyword))
                    );
                    break;
            }
        }

        UpdateCustomerList(query.ToList());
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
