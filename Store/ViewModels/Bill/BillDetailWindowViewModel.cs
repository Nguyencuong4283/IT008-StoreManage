using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.SkiaSharpView.Avalonia;
using Store.Models;
using Store.Services;
using Store.Messages;
using Store.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ViewModels.Bill
{
    public partial class BillDetailWindowViewModel : ViewModelBase
    {

        [ObservableProperty] private string tenNhanVien;
        [ObservableProperty] private string tenKhachHang;
        [ObservableProperty] private string thoiGianHienTai;
        [ObservableProperty] private int soHD;

        [ObservableProperty]
        private decimal tongGiamGia;

        [ObservableProperty]
        private decimal tongTriGia;

        [ObservableProperty]
        private decimal tongThanhTien;
       
        [ObservableProperty]
        private ObservableCollection<ChiTiet_HoaDon> chiTietHoaDons = new();

        public BillDetailWindowViewModel()
        {
           
        }
        public BillDetailWindowViewModel(HoaDon hoaDon)
        {
            if (hoaDon != null)
            {
                TenNhanVien = hoaDon.TenUser;
                TenKhachHang = hoaDon.TenKH;

                SoHD = hoaDon.SoHD;
                ThoiGianHienTai = hoaDon.NgayLapHD.ToString("dd/MM/yyyy HH:mm:ss");
                var chiTietList = ChiTiet_HoaDonService.GetChiTiet_HoaDon(hoaDon.MaHD);
                ChiTietHoaDons.Clear();
                foreach (var ct in chiTietList)
                {
                    ChiTietHoaDons.Add(ct);
                }
                // Tính toán tổng giá trị
                TongTriGia = hoaDon.TongTienHD;
                TongGiamGia = hoaDon.GiamGiaHD;
                TongThanhTien = TongTriGia - TongGiamGia;
            }
        }

        //public ProductDetailWindowViewModel()
        //{
        //    // Constructor mặc định cho Design.DataContext
        //}

        //public ProductDetailWindowViewModel(SanPham sanPham)
        //{
        //    if (sanPham != null)
        //    {
        //        HinhAnhSP = sanPham.HinhAnhSP;
        //        TenSP = sanPham.TenSP;
        //        GiaSP = sanPham.GiaSP;
        //        SoLuongSP = sanPham.SoLuongSP;
        //        MaSP = sanPham.MaSP;
        //        LoaiSP = sanPham.LoaiSP;
        //        KichCoSP = sanPham.KichThuocSP;
        //        MoTaSP = sanPham.MoTaSP;
        //    }
        //}
    }
}
