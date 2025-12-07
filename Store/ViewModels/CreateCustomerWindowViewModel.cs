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
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;

namespace Store.ViewModels
{
    public partial class CreateCustomerWindowViewModel: ViewModelBase
    {
        [ObservableProperty] private string maKH;
        [ObservableProperty] private string tenKH;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string gioiTinh = "Chọn giới tính" ;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private string ghiChu;

        public ObservableCollection<string> DanhSachGioiTinh { get; } = new()
        {
            "Nam",
            "Nữ",
            "Khác"
        };
        public CreateCustomerWindowViewModel()
        {
           MaKH = KhachHangService.GenerateNewMaKH();
           
        }
        [RelayCommand]
        public void TaoKhachHangButton()
        {
            try
            {
                var khachHang = new KhachHang
                {
                    MaKH = MaKH,
                    TenKH = TenKH,
                    SDT = SDT,
                    GioiTinh = GioiTinh,
                    DiaChi = DiaChi,
                    Hang = "3",
                    GhiChu = GhiChu,
                    TongMua = (decimal)0,
                };
                
                KhachHangService.InsertKhachHang(khachHang);
                // Sau khi thêm thành công
                WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage("Insert"));
                System.Diagnostics.Debug.WriteLine($"Đã thêm khách hàng: {tenKH}");
                // Reset form
                TenKH = "";
                SDT = "";
                GioiTinh = "";
                DiaChi = "";
                GhiChu = "";
                MaKH =KhachHangService.GenerateNewMaKH(); // tạo mã mới cho lần tiếp theo
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo khách hàng: {ex.Message}");
            }
        }
    }
}
