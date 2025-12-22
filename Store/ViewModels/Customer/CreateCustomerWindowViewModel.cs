using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ViewModels.Customer
{
    public partial class CreateCustomerWindowViewModel: ViewModelBase
    {
        [ObservableProperty] private string maKH;
        [ObservableProperty] private string tenKH;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string gioiTinh = "Chọn giới tính" ;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private string ghiChu;
        public Window? ParentWindow { get; set; }
        public ObservableCollection<string> DanhSachGioiTinh { get; } = new()
        {
            "Nam",
            "Nữ",
            "Khác"
        };
        public CreateCustomerWindowViewModel()
        {
           MaKH = CustomerService.GenerateCustommerID();
           
        }
        [RelayCommand]
        public void CreateCusomterButton()
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
                
                CustomerService.InsertCustomer(khachHang);
                // Sau khi thêm thành công
                WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage("Insert"));
                System.Diagnostics.Debug.WriteLine($"Đã thêm khách hàng: {tenKH}");
                ParentWindow?.Close();
                // Reset form
                TenKH = "";
                SDT = "";
                GioiTinh = "";
                DiaChi = "";
                GhiChu = "";
                MaKH =CustomerService.GenerateCustommerID(); // tạo mã mới cho lần tiếp theo
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo khách hàng: {ex.Message}");
            }
        }
    }
}
