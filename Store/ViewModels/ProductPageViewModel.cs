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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Store.ViewModels
{
    public partial class ProductPageViewModel : ViewModelBase
    {
        private List<SanPham> allSanPhams = new();
        
        [ObservableProperty] private Bitmap hinhAnhSP;
        [ObservableProperty] private string tenSP;
        [ObservableProperty] private decimal giaSP;
        [ObservableProperty] private string searchKeyword;
        [ObservableProperty] private ObservableCollection<SanPham> sanPhams = new();
        
        private string _boLoc = "Tất cả";
        public string BoLoc
        {
            get => _boLoc;
            set
            {
                if (SetProperty(ref _boLoc, value))
                {
                    ApplyFilter();
                }
            }
        }
        
        private string _selectedDetail = "Tất cả";
        public string SelectedDetail
        {
            get => _selectedDetail;
            set
            {
                if (SetProperty(ref _selectedDetail, value))
                {
                    ApplyFilter();
                }
            }
        }
        
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
        
        private readonly DispatcherTimer _timer;
        public ProductPageViewModel()
        {
            LoadSanPhams();
            
            // Tạo timer lặp lại mỗi 5 giây
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _timer.Tick += (s, e) =>
            {
                if (string.IsNullOrEmpty(SearchKeyword))
                {
                    LoadSanPhams();
                }
            };
            _timer.Start();
        }
        private void LoadSanPhams()
        {
            var list = SanPhanService.GetAllSanPham();
            allSanPhams = list;
            
            if (string.IsNullOrEmpty(SearchKeyword) && BoLoc == "Tất cả")
            {
                UpdateProductsList(allSanPhams);
            }
            else
            {
                ApplyFilter();
            }
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
            if (sanPham == null) return;

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
        private void ApplyFilter()
        {
            if (sanPhams == null) return;
            
            IEnumerable<SanPham> query = allSanPhams;

            if (!string.IsNullOrEmpty(BoLoc) && !BoLoc.Equals("Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(sp => sp.LoaiSP == BoLoc);
            }
            
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                string keyword = SearchKeyword.ToLower().Trim();
                
                switch (SelectedDetail)
                {
                    case "Giá sản phẩm":
                        query = query.Where(sp => sp.GiaSP.ToString().Contains(keyword));
                        break;

                    case "Tên sản phẩm":
                        query = query.Where(sp => sp.TenSP.ToLower().Contains(keyword));
                        break;

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
            UpdateProductsList(query);
        }
        private void UpdateProductsList(IEnumerable<SanPham> products)
        {
            sanPhams.Clear();
            foreach (var sp in products)
            {
                sanPhams.Add(sp);
            }
        }
    }
}
