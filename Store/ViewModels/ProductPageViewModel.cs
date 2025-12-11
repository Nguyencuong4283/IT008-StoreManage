using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;

namespace Store.ViewModels;

public partial class ProductPageViewModel : ViewModelBase, IRecipient<SanPhamChangedMessage>
{
    private List<SanPham> _allSanPhams = new();

    [ObservableProperty] private Bitmap hinhAnhSP;
    [ObservableProperty] private string tenSP;
    [ObservableProperty] private decimal giaSP;
    [ObservableProperty] private string searchKeyword;
    [ObservableProperty] private string _filter = "Tất cả";
    [ObservableProperty] private string _selectedDetail = "Tất cả";
    [ObservableProperty] private ObservableCollection<SanPham> sanPhams = new();
    [ObservableProperty] private decimal minPrice = 0m;
    [ObservableProperty] private decimal maxPrice = decimal.MaxValue;
    [ObservableProperty] private int minQuantity = 0;

    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
    {
        "Tất cả",
        "Quần ngắn",
        "Quần dài",
        "Áo ngắn",
        "Áo dài",
        "Khác"
    };

    public ObservableCollection<string> DanhSachChiTiet { get; } = new()
    {
        "Tất cả",
        "Giá sản phẩm",
        "Tên sản phẩm",
        "Số lượng"
    };
    public ProductPageViewModel()
    {
        WeakReferenceMessenger.Default.Register<SanPhamChangedMessage>(this);
        LoadSanPhams();
    }
    public void Receive(SanPhamChangedMessage message)
    {
        // Đảm bảo cập nhật trên UI thread
        Dispatcher.UIThread.Post(() =>
        {
            LoadSanPhams();
        });
    }
    private void LoadSanPhams()
    {
        var list = SanPhanService.GetAllSanPham();
        _allSanPhams = list;
        UpdateProductsList(_allSanPhams);
    }

    [RelayCommand]
    private void ThemSanPhamButton()
    {
        CreateProductWindowView createProductWindowView = new();
        createProductWindowView.Show();
        LoadSanPhams();
    }

    [RelayCommand]
    private void XemChiTietSanPham(SanPham sanPham)
    {
        var detailWindow = new ProductDetailWindowView
        {
            DataContext = new ProductDetailWindowViewModel(sanPham)
        };
        detailWindow.Show();
    }

    //===== Tìm kiếm và lọc sản phẩm =====//
    partial void OnSearchKeywordChanged(string value)
    {
        ApplyFilter();
    }
    partial void OnFilterChanged(string value)
    {
        ApplyFilter();
    }
    partial void OnSelectedDetailChanged(string value)
    {
        ApplyFilter();
    }
    partial void OnMinPriceChanged(decimal value)
    {
        ApplyFilter();
    }

    partial void OnMaxPriceChanged(decimal value)
    {
        ApplyFilter();
    }

    partial void OnMinQuantityChanged(int value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        IEnumerable<SanPham> query = _allSanPhams;

        if (!string.IsNullOrEmpty(Filter) && !Filter.Equals("Tất cả", StringComparison.OrdinalIgnoreCase))
            query = query.Where(sp => sp.LoaiSP == Filter);

        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            var keyword = SearchKeyword.ToLower().Trim();

            switch (SelectedDetail)
            {
                // Tìm giá sản phẩm
                case "Giá sản phẩm":
                    query = query.Where(sp => sp.GiaSP.ToString().Contains(keyword));
                    break;

                // Tìm tên sản phẩm
                case "Tên sản phẩm":
                    query = query.Where(sp => sp.TenSP.ToLower().Contains(keyword));
                    break;

                // Tìm số lượng
                case "Số lượng":
                    query = query.Where(sp => sp.SoLuongSP.ToString().Contains(keyword));
                    break;

                default: // Tất cả
                    query = query.Where(sp =>
                        sp.TenSP.ToLower().Contains(keyword) ||
                        sp.GiaSP.ToString().Contains(keyword) ||
                        sp.SoLuongSP.ToString().Contains(keyword));
                    break;
            }
        }

        query = query.Where(sp => sp.GiaSP >= MinPrice && sp.GiaSP <= MaxPrice);
        query = query.Where(sp => sp.SoLuongSP >= MinQuantity);

        // Cập nhật danh sách sản phẩm hiển thị
        UpdateProductsList(query);
    }
    private void UpdateProductsList(IEnumerable<SanPham> products)
    {
        SanPhams.Clear();
        foreach (var sp in products) SanPhams.Add(sp);
    }
}