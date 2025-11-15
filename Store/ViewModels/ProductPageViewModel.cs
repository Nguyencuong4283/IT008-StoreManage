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
using System.Collections.ObjectModel;

namespace Store.ViewModels
{
    public partial class ProductPageViewModel : ViewModelBase
    {
        [ObservableProperty] private Bitmap hinhAnhSP;
        [ObservableProperty] private string tenSP;
        [ObservableProperty] private decimal giaSP;
        [ObservableProperty] private int soLuongSP;
        [ObservableProperty] private ObservableCollection<SanPham> sanPhams = new();
        private readonly DispatcherTimer _timer;
        public ObservableCollection<string> DanhSachBoLoc { get; } = new()
        {
           "Quần ngắn",
           "Quần dài",
           "Áo ngắn",
           "Áo dài",
           "Tất cả"
        };
        public ObservableCollection<string> DanhSachChiTiet { get; } = new()
        {
            "GiaSP",
            "TenSP",
            "SoLuong",
            "Tất cả"
        };
        public ProductPageViewModel()
        {
            LoadSanPhams();
            // ✅ Tạo timer lặp lại mỗi 5 giây
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _timer.Tick += (s, e) => LoadSanPhams();
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
    }
}
