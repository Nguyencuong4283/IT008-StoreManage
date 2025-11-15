using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.SkiaSharpView.Avalonia;
using Store.Models;
using Store.Services;
using Store.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ViewModels
{
   public partial class CreateBillWindowViewModel : ViewModelBase
    {
        
        [ObservableProperty] 
        private int soLuong;
        
        [ObservableProperty] 
        private decimal giaSP;
        
        [ObservableProperty] 
        private int khuyenMai;

        [ObservableProperty]
        private decimal tongGiamGia;
        
        [ObservableProperty] 
        private decimal tongTriGia;

        [ObservableProperty] 
        private decimal tongThanhTien;



        [ObservableProperty] private string thoiGianHienTai = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        [ObservableProperty] private int soHD;
        [ObservableProperty]
        private ObservableCollection<ChiTiet_HoaDon> chiTietHoaDons = new();

        [ObservableProperty]
        private ObservableCollection<KhachHang> danhSachKhachHang = new();
        [ObservableProperty]
        private ObservableCollection<SanPham> danhSachSanPham = new();
        [ObservableProperty]
        private ObservableCollection<User> danhSachNhanVien = new();

        [ObservableProperty]
        private KhachHang? khachHangDuocChon; 
        [ObservableProperty]
        private SanPham? sanPhamDuocChon;
        [ObservableProperty]
        private User? nhanVienDuocChon;

        private string MaHD;
        private bool isHoaDonCreated = false;


        public CreateBillWindowViewModel()
        {
            MaHD = HoaDonService.GenerateNewMaHD();
            SoHD = HoaDonService.GetNextSoHD();
            SoLuong = 1; // Khởi tạo số lượng mặc định
            
            LoadKhachHang();
            LoadSanPham();
            LoadUser();
            
            System.Diagnostics.Debug.WriteLine($"[ViewModel] Khởi tạo với MaHD: {MaHD}, SoHD: {SoHD}");
        }

        // Tạo hóa đơn tạm thời trong database
        private void TaoHoaDonTamThoi()
        {
            if (!isHoaDonCreated)
            {
                // Kiểm tra khách hàng và nhân viên đã chọn chưa
                if (KhachHangDuocChon == null || NhanVienDuocChon == null)
                {
                    System.Diagnostics.Debug.WriteLine("[ViewModel] Chưa chọn khách hàng hoặc nhân viên");
                    return;
                }
                
                var hoaDonTam = new HoaDon
                {
                    MaHD = MaHD,
                    NgayLapHD = DateTime.Now,
                    TongTienHD = 0,
                    GiamGiaHD = 0,
                    MaKH = KhachHangDuocChon.MaKH,
                    MaUser = NhanVienDuocChon.MaNV,
                    SoHD = SoHD,
                    TrangThaiHD = "Đang tạo"
                };
                
                try
                {
                    HoaDonService.InsertHoaDon(hoaDonTam);
                    isHoaDonCreated = true;
                    System.Diagnostics.Debug.WriteLine($"[ViewModel] Đã tạo hóa đơn tạm thời: {MaHD}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ViewModel] Lỗi tạo hóa đơn tạm: {ex.Message}");
                }
            }
        }
        
        // Thanh toán và xuất hóa đơn
        [RelayCommand]
        private async Task ThanhToan()
        {
            // Kiểm tra điều kiện
            if (KhachHangDuocChon == null || NhanVienDuocChon == null)
            {
                System.Diagnostics.Debug.WriteLine("[ThanhToan] Chưa chọn khách hàng hoặc nhân viên");
                return;
            }
            
            if (ChiTietHoaDons.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("[ThanhToan] Chưa có sản phẩm nào trong hóa đơn");
                return;
            }
            
            try
            {
                // Cập nhật trạng thái hóa đơn thành "Đã thanh toán"
                var hoaDon = HoaDonService.GetHoaDonById(MaHD);
                if (hoaDon != null)
                {
                    hoaDon.TongTienHD = TongThanhTien;
                    hoaDon.GiamGiaHD = TongGiamGia;
                    hoaDon.TrangThaiHD = "Đã thanh toán";
                    HoaDonService.UpdateHoaDon(hoaDon);
                }
                
                // Xuất file hóa đơn
                await XuatFileHoaDon();
                
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Đã thanh toán hóa đơn {MaHD}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Lỗi: {ex.Message}");
            }
        }
        
        // Xuất file hóa đơn PDF
        private async Task XuatFileHoaDon()
        {
            try
            {
                // Tạo thư mục Bills nếu chưa có
                string billsFolder = Path.Combine(AppContext.BaseDirectory, "Bills");
                if (!Directory.Exists(billsFolder))
                {
                    Directory.CreateDirectory(billsFolder);
                }
                
                // Tạo tên file PDF
                string fileName = $"HoaDon_{MaHD}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(billsFolder, fileName);
                
                // Xuất PDF sử dụng PdfExportService
                await Task.Run(() =>
                {
                    PdfExportService.XuatHoaDonPdf(
                        maHD: MaHD,
                        soHD: SoHD,
                        ngayLap: DateTime.Now,
                        tenKhachHang: KhachHangDuocChon?.TenKH ?? "N/A",
                        sdtKhachHang: KhachHangDuocChon?.SDT ?? "N/A",
                        tenNhanVien: NhanVienDuocChon?.HoTen ?? "N/A",
                        chiTietHoaDons: ChiTietHoaDons.ToList(),
                        tongTriGia: TongTriGia,
                        tongGiamGia: TongGiamGia,
                        tongThanhTien: TongThanhTien,
                        outputPath: filePath
                    );
                });
                
                System.Diagnostics.Debug.WriteLine($"[XuatFileHoaDon] Đã xuất file PDF: {filePath}");
                
                // Mở file PDF sau khi xuất
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[XuatFileHoaDon] Lỗi: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[XuatFileHoaDon] Stack trace: {ex.StackTrace}");
            }
        }
        
        // Cập nhật giá khi chọn sản phẩm
        partial void OnSanPhamDuocChonChanged(SanPham? value)
        {
            if (value != null)
            {
                GiaSP = value.GiaSP;
            }
        }
        private void LoadKhachHang()
        {
            var ds = KhachHangService.GetAllKhachHang();
            danhSachKhachHang = new ObservableCollection<KhachHang>(ds);
        }
        private void LoadSanPham()
        {
            var ds1 = SanPhanService.GetAllSanPham();
            danhSachSanPham = new ObservableCollection<SanPham>(ds1);
        }
        private void LoadUser()
        {
            var ds2 = UserService.GetAllUser();
            danhSachNhanVien = new ObservableCollection<User>(ds2);
        }
        [RelayCommand]
        private void Tang()
        {
            if(SoLuong < SanPhamDuocChon.SoLuongSP)
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

            // Kiểm tra số lượng hợp lệ
            if (SoLuong <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Số lượng phải lớn hơn 0");
                return;
            }

            // Kiểm tra số lượng tồn kho
            if (SoLuong > SanPhamDuocChon.SoLuongSP)
            {
                System.Diagnostics.Debug.WriteLine($"Số lượng vượt quá tồn kho ({SanPhamDuocChon.SoLuongSP})");
                return;
            }

            try
            {
                // Tạo hóa đơn tạm thời nếu chưa có
                TaoHoaDonTamThoi();
                
                // Tính thành tiền
                decimal donGia = SanPhamDuocChon.GiaSP;
                decimal giamGia = donGia * KhuyenMai / 100;
                decimal thanhTien = (donGia - giamGia) * SoLuong;

                // Kiểm tra sản phẩm đã tồn tại trong chi tiết hóa đơn chưa
                var chiTietTonTai = ChiTietHoaDons.FirstOrDefault(ct => ct.MaSP == SanPhamDuocChon.MaSP);
                
                if (chiTietTonTai != null)
                {
                    // Nếu đã có, cập nhật số lượng và thành tiền
                    chiTietTonTai.SoLuong += SoLuong;
                    chiTietTonTai.ThanhTien = (chiTietTonTai.DonGia - (chiTietTonTai.DonGia * chiTietTonTai.KhuyenMai / 100)) * chiTietTonTai.SoLuong;
                    
                    // Cập nhật trong database
                    ChiTiet_HoaDonService.UpdateChiTiet_HoaDon(chiTietTonTai);
                    
                    // Xóa và thêm lại để trigger UI update
                    var index = ChiTietHoaDons.IndexOf(chiTietTonTai);
                    ChiTietHoaDons.RemoveAt(index);
                    ChiTietHoaDons.Insert(index, chiTietTonTai);
                }
                else
                {
                    // Thêm chi tiết hóa đơn mới
                    var chiTietMoi = new ChiTiet_HoaDon
                    {            
                        MaHD = MaHD,
                        MaSP = SanPhamDuocChon.MaSP,
                        SoLuong = SoLuong,
                        DonGia = donGia,
                        KhuyenMai = KhuyenMai,
                        ThanhTien = thanhTien,
                        SanPham = SanPhamDuocChon //Gán thông tin sản phẩm để hiển thị
                    };
                    ChiTietHoaDons.Add(chiTietMoi);
                    ChiTiet_HoaDonService.InsertChiTiet_HoaDon(chiTietMoi);
                }
                
                // Cập nhật tổng tiền sau khi thêm/cập nhật
                CapNhatTongTien();

                // Reset form
                SoLuong = 1;
                KhuyenMai = 0;
                SanPhamDuocChon = null;
                GiaSP = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thêm chi tiết hóa đơn: {ex.Message}");
            }
        }

        // Phương thức cập nhật tổng tiền
        private void CapNhatTongTien()
        {
            // Tính từ collection thay vì query database
            TongTriGia = ChiTietHoaDons.Sum(ct => ct.DonGia * ct.SoLuong);
            TongGiamGia = ChiTietHoaDons.Sum(ct => ct.DonGia * ct.SoLuong * ct.KhuyenMai / 100);
            TongThanhTien = ChiTietHoaDons.Sum(ct => ct.ThanhTien);
        }
        
    }
}
