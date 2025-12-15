
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private List<SanPham> _allSanPham = new();
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
        LoadProduct();
    }
    public void Receive(SanPhamChangedMessage message)
    {
        // Đảm bảo cập nhật trên UI thread
        Dispatcher.UIThread.Post(() =>
        {
            LoadProduct();
        });
    }
    private void LoadProduct()
    {
        var list = ProductService.GetAllProduct();
        _allSanPhams = list;
        UpdateProductsList(_allSanPhams);
    }

    [RelayCommand]
    private void ThemSanPhamButton()
    {
        CreateProductWindowView createProductWindowView = new();
        createProductWindowView.Show();
        LoadProduct();
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
        SearchProducts();
    }
    partial void OnFilterChanged(string value)
    {
        SearchProducts();
    }
    partial void OnSelectedDetailChanged(string value)
    {
        SearchProducts();
    }

    private void SearchProducts()
    {
        var allProducts = _allSanPhams;
        var filterList = allProducts;
        // Lọc theo loại sản phẩm
        if (Filter != "Tất cả")
        {
            filterList = filterList.FindAll(sp => sp.LoaiSP == Filter);
        }
        // Tìm kiếm theo từ khóa
        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            switch (SelectedDetail)
            {
                // Tìm theo tên sản phẩm
                case "Tên sản phẩm":
                    filterList = filterList.FindAll(sp => sp.TenSP.ToLower().Contains(SearchKeyword.ToLower()));
                    break;
                
                // Tìm theo giá sản phẩm
                case "Giá sản phẩm":   
                    if (decimal.TryParse(SearchKeyword, out decimal giaSP))
                    {
                        filterList = filterList.FindAll(sp => sp.GiaSP == giaSP);
                    }
                    else
                    {
                        filterList = new List<SanPham>();
                    }
                    break;
                
                // Tìm theo số lượng
                case "Số lượng":
                    if (int.TryParse(SearchKeyword, out int soLuong))
                    {
                        filterList = filterList.FindAll(sp => sp.SoLuongSP == soLuong);
                    }
                    else
                    {
                        filterList = new List<SanPham>();
                    }
                    break;
                
                // Tìm tất cả
                default:
                    filterList = filterList.FindAll(sp =>
                        sp.TenSP.ToLower().Contains(SearchKeyword.ToLower()) ||
                        sp.GiaSP.ToString().Contains(SearchKeyword) ||
                        sp.SoLuongSP.ToString().Contains(SearchKeyword)
                    );
                    break;
            }
        }
        // Cập nhật danh sách sản phẩm hiển thị
        UpdateProductsList(filterList);
    }
    private void UpdateProductsList(IEnumerable<SanPham> products)
    {
        SanPhams.Clear();
        foreach (var sp in products) SanPhams.Add(sp);
    }
}