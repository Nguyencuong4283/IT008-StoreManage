using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using Store.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Store.ViewModels.Import
{
    public partial class CreateImportWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string maNhapKho;
        [ObservableProperty] private string nhaCungCap = string.Empty;
        [ObservableProperty] private string? ghiChu;
        [ObservableProperty] private string thoiGianHienTai = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        
        [ObservableProperty] private int soLuong = 1;
        [ObservableProperty] private decimal donGia;
        [ObservableProperty] private decimal tongTien;
        [ObservableProperty] private decimal donGiaNhap = 0;

        public Window? ParentWindow { get; set; }

        public ObservableCollection<ChiTiet_NhapKho> ChiTietList { get; } = new();

        [ObservableProperty] 
        private ObservableCollection<SanPham> danhSachSanPham = new();
        
        [ObservableProperty] 
        private SanPham? sanPhamDuocChon;

        public CreateImportWindowViewModel()
        {
            LoadSanPham();
            maNhapKho = ImportService.GenerateNewImportID();
        }

        private void LoadSanPham()
        {
            var ds = ProductService.GetAllProduct();
            DanhSachSanPham = new ObservableCollection<SanPham>(ds);
        }

        // Cập nhật giá khi chọn sản phẩm
        partial void OnSanPhamDuocChonChanged(SanPham? value)
        {
            if (value != null)
            {
                DonGia = value.GiaSP;
            }
        }

        [RelayCommand]
        private void IncreasePrice()
        {


            if (SanPhamDuocChon == null)
            {
                System.Diagnostics.Debug.WriteLine("Chưa chọn sản phẩm");
                return;
            }
            if (DonGiaNhap < SanPhamDuocChon.GiaSP - 1000)
            {
                DonGiaNhap += 1000;
            }
            
        }

        [RelayCommand]
        private void DecreasePrice()
        {
            if (DonGiaNhap >= 2000)
                DonGiaNhap -= 1000;
        }

        [RelayCommand]
        private void Tang()
        {
            SoLuong++;
        }

        [RelayCommand]
        private void Giam()
        {
            if (SoLuong > 1)
                SoLuong--;
        }

        [RelayCommand]
        private void ThemSanPham()
        {
            // Kiểm tra sản phẩm được chọn
            if (SanPhamDuocChon == null)
            {
                System.Diagnostics.Debug.WriteLine("Chưa chọn sản phẩm");
                return;
            }

            // Kiểm tra số lượng và đơn giá hợp lệ
            if (SoLuong <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Số lượng phải lớn hơn 0");
                return;
            }

            if (DonGia <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Đơn giá phải lớn hơn 0");
                return;
            }

            try
            {
                // Tính thành tiền
                decimal thanhTien = DonGiaNhap * SoLuong;

                // Kiểm tra sản phẩm đã tồn tại trong chi tiết nhập kho chưa
                var chiTietTonTai = ChiTietList.FirstOrDefault(ct => ct.MaSP == SanPhamDuocChon.MaSP);
                
                if (chiTietTonTai != null)
                {
                    // Nếu đã có, cập nhật số lượng và thành tiền
                    chiTietTonTai.SoLuong += SoLuong;
                    chiTietTonTai.DonGia = DonGiaNhap;
                    chiTietTonTai.ThanhTien = chiTietTonTai.SoLuong * chiTietTonTai.DonGia;
                    
                    // Xóa và thêm lại để trigger UI update
                    var index = ChiTietList.IndexOf(chiTietTonTai);
                    ChiTietList.RemoveAt(index);
                    ChiTietList.Insert(index, chiTietTonTai);
                    
                    System.Diagnostics.Debug.WriteLine($"Đã cập nhật số lượng sản phẩm: {SanPhamDuocChon.TenSP}, SoLuong mới: {chiTietTonTai.SoLuong}");
                }
                else
                {
                    // Thêm chi tiết nhập kho mới
                    var chiTietMoi = new ChiTiet_NhapKho
                    {            
                        MaNK = MaNhapKho,
                        MaSP = SanPhamDuocChon.MaSP,
                        SoLuong = SoLuong,
                        DonGia = DonGiaNhap,
                        ThanhTien = thanhTien,
                        SanPham = SanPhamDuocChon // Gán thông tin sản phẩm để hiển thị
                    };
                    ChiTietList.Add(chiTietMoi);
                    System.Diagnostics.Debug.WriteLine($"Đã thêm sản phẩm mới: {SanPhamDuocChon.TenSP}, SoLuong: {SoLuong}");
                }
                
                // Cập nhật tổng tiền sau khi thêm/cập nhật
                CapNhatTongTien();

                // Reset form
                SoLuong = 1;
                DonGia = 0;
                DonGiaNhap = 0;
                SanPhamDuocChon = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thêm chi tiết nhập kho: {ex.Message}");
            }
        }

        // Phương thức cập nhật tổng tiền
        private void CapNhatTongTien()
        {
            TongTien = ChiTietList.Sum(ct => ct.ThanhTien);
        }

        [RelayCommand]
        private void XoaChiTiet()
        {
            ChiTietList.Clear();
            CapNhatTongTien();
        }

        [RelayCommand]
        private void TaoPhieuNhap()
        {
            // Kiểm tra điều kiện
            if (string.IsNullOrWhiteSpace(NhaCungCap))
            {
                System.Diagnostics.Debug.WriteLine("[TaoPhieuNhap] Chưa nhập nhà cung cấp");
                return;
            }
            
            if (ChiTietList.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[TaoPhieuNhap] Chưa có sản phẩm nào trong phiếu nhập");
                return;
            }
            
            try
            {
                // Tạo lại MaNhapKho để đảm bảo unique
                MaNhapKho = ImportService.GenerateNewImportID();
                
                // Cập nhật MaNK cho tất cả chi tiết
                foreach (var chiTiet in ChiTietList)
                {
                    chiTiet.MaNK = MaNhapKho;
                }
                
                // Tạo phiếu nhập kho
                var nk = new Models.Import
                {
                    MaNK = MaNhapKho,
                    NgayNhap = DateOnly.FromDateTime(DateTime.Now),
                    NhaCungCap = NhaCungCap,
                    TongTien = TongTien,
                    GhiChu = GhiChu,
                    MaUser = 1
                };

                ImportService.InsertNhapKho(nk, ChiTietList.ToList());
                System.Diagnostics.Debug.WriteLine($"[TaoPhieuNhap] Đã tạo phiếu nhập kho: {MaNhapKho}");
                
                // Gửi message cập nhật
                WeakReferenceMessenger.Default.Send(new ImportChangeMessage("Created"));
                WeakReferenceMessenger.Default.Send(new SanPhamChangedMessage("Updated"));

                // Đóng window
                ParentWindow?.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TaoPhieuNhap] Lỗi: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[TaoPhieuNhap] Stack trace: {ex.StackTrace}");
            }
        }
    }
}
