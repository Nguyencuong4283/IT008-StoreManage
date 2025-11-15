using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Store.Models;
using System;

namespace Store.ViewModels
{
    public partial class ProductDetailWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private Bitmap hinhAnhSP;
        [ObservableProperty] private string tenSP;
        [ObservableProperty] private decimal giaSP;
        [ObservableProperty] private int soLuongSP;
        [ObservableProperty] private string maSP;
        [ObservableProperty] private string loaiSP;
        [ObservableProperty] private string kichCoSP;
        [ObservableProperty] private string moTaSP;

        public ProductDetailWindowViewModel()
        {
            // Constructor mặc định cho Design.DataContext
        }

        public ProductDetailWindowViewModel(SanPham sanPham)
        {
            if (sanPham != null)
            {
                HinhAnhSP = sanPham.HinhAnhSP;
                TenSP = sanPham.TenSP;
                GiaSP = sanPham.GiaSP;
                SoLuongSP = sanPham.SoLuongSP;
                MaSP = sanPham.MaSP;
                LoaiSP = sanPham.LoaiSP;
                KichCoSP = sanPham.KichThuocSP;
                MoTaSP = sanPham.MoTaSP;
            }
        }
    }
}
