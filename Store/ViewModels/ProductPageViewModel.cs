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
        [ObservableProperty] private Bitmap hinhAnhSP;
        [ObservableProperty] private string tenSP;
        [ObservableProperty] private decimal giaSP;
        [ObservableProperty] private ObservableCollection<SanPham> sanPhams = new();
        
        private string _boLoc;
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
        
        [ObservableProperty] private string searchKeyword;
        [ObservableProperty] private string _selectedFilterBy = "TenSP";
        
        private readonly DispatcherTimer _timer;
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
            "GiaSP",
            "TenSP",
            "SoLuong"
        };
        public ProductPageViewModel()
        {
            LoadSanPhams();
            
            // ✅ Tạo timer lặp lại mỗi 5 giây
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _timer.Tick += (s, e) => ApplyFilter();
            _timer.Start();
        }
        private void LoadSanPhams()
        {
            var list = SanPhanService.GetAllSanPham();

            sanPhams.Clear();
            foreach (var sp in list)
            {
                sanPhams.Add(sp);
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
        private void ApplyFilter()
        {
            List<SanPham> list;
            
            if (string.IsNullOrEmpty(BoLoc) || BoLoc == "Tất cả")
            {
                list = SanPhanService.GetAllSanPham();
            }
            else
            {
                list = SanPhanService.GetSearchSanPham(BoLoc);
            }

            sanPhams.Clear();
            foreach (var sp in list)
            {
                sanPhams.Add(sp);
            }
        }
        
    }
}
