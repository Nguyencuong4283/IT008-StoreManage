using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ViewModels
{
    public partial class CustomerDetailWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string maKH;
        [ObservableProperty] private string tenKH;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string gioiTinh;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private string ghiChu;

        public CustomerDetailWindowViewModel() { }
      
        public CustomerDetailWindowViewModel(KhachHang khachhang)
        {
           if (khachhang != null)
           {
               MaKH = khachhang.MaKH;
               TenKH = khachhang.TenKH;
               SDT = khachhang.SDT;
               GioiTinh = khachhang.GioiTinh;
               DiaChi = khachhang.DiaChi;
               GhiChu = khachhang.GhiChu;
            }
        }

        [RelayCommand]
        private void XoaKhachHangButton()
        {
            try
            {
                if (string.IsNullOrEmpty(MaKH))
                {
                    System.Diagnostics.Debug.WriteLine("Không có mã khách hàng để xóa");
                    return;
                }
                KhachHangService.DeleteKhachHang(MaKH);
                System.Diagnostics.Debug.WriteLine($"✅ Đã xóa khách hàng: {MaKH}");

                // Đóng window sau khi xóa
                if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var window = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                    window?.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi xóa khách hàng: {ex.Message}");
            }
        }
    }
}
