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
            LoadKhachHang();
            LoadSanPham();
            LoadUser();
            
            // Kiểm tra xem có hóa đơn nháp không
            if (DraftBillManager.HasDraft)
            {
                // Load hóa đơn nháp
                var draft = DraftBillManager.LoadDraft();
                MaHD = draft.MaHD;
                SoHD = draft.SoHD;
                ChiTietHoaDons = draft.Items;
                KhachHangDuocChon = draft.KhachHang;
                NhanVienDuocChon = draft.NhanVien;
                
                CapNhatTongTien();
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Đã load hóa đơn nháp: {MaHD}, {ChiTietHoaDons.Count} sản phẩm");
            }
            else
            {
                // Tạo hóa đơn mới
                MaHD = HoaDonService.GenerateNewMaHD();
                SoHD = HoaDonService.GetNextSoHD();
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Khởi tạo hóa đơn mới: {MaHD}, SoHD: {SoHD}");
            }
            
            SoLuong = 1; // Khởi tạo số lượng mặc định
        }
        
        // Lưu nháp khi đóng window (nếu chưa thanh toán)
        public void OnWindowClosing()
        {
            if (!isHoaDonCreated && ChiTietHoaDons.Count > 0)
            {
                DraftBillManager.SaveDraft(MaHD, SoHD, ChiTietHoaDons, KhachHangDuocChon, NhanVienDuocChon);
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Đã lưu nháp khi đóng window");
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
                // Bước 1: Tạo hóa đơn trong database
                var hoaDon = new HoaDon
                {
                    MaHD = MaHD,
                    NgayLapHD = DateTime.Now,
                    TongTienHD = TongThanhTien,
                    GiamGiaHD = TongGiamGia,
                    MaKH = KhachHangDuocChon.MaKH,
                    MaUser = NhanVienDuocChon.MaNV,
                    SoHD = SoHD,
                    TrangThaiHD = "Đã thanh toán"
                };
                HoaDonService.InsertHoaDon(hoaDon);
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Đã tạo hóa đơn: {MaHD}");
                
                // Bước 2: Lưu tất cả chi tiết hóa đơn vào database
                foreach(var chiTiet in ChiTietHoaDons)
                {
                    ChiTiet_HoaDonService.InsertChiTiet_HoaDon(chiTiet);
                }
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Đã lưu {ChiTietHoaDons.Count} chi tiết hóa đơn");
                
                // Bước 3: Xuất file hóa đơn PDF
                await XuatFileHoaDon();
                
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Hoàn tất thanh toán hóa đơn {MaHD}");
                
                // Bước 4: Đánh dấu đã thanh toán và xóa nháp
                isHoaDonCreated = true;
                DraftBillManager.ClearDraft();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Lỗi: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Stack trace: {ex.StackTrace}");
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
                // Tính thành tiền
                decimal donGia = SanPhamDuocChon.GiaSP;
                decimal giamGia = donGia * KhuyenMai / 100;
                decimal thanhTien = (donGia - giamGia) * SoLuong;

                // Kiểm tra sản phẩm đã tồn tại trong chi tiết hóa đơn chưa
                var chiTietTonTai = ChiTietHoaDons.FirstOrDefault(ct => ct.MaSP == SanPhamDuocChon.MaSP);
                
                if (chiTietTonTai != null)
                {
                    // Nếu đã có, cập nhật số lượng và thành tiền (CHỈ TRONG MEMORY)
                    chiTietTonTai.SoLuong += SoLuong;
                    chiTietTonTai.ThanhTien = (chiTietTonTai.DonGia - (chiTietTonTai.DonGia * chiTietTonTai.KhuyenMai / 100)) * chiTietTonTai.SoLuong;
                    
                    // Xóa và thêm lại để trigger UI update
                    var index = ChiTietHoaDons.IndexOf(chiTietTonTai);
                    ChiTietHoaDons.RemoveAt(index);
                    ChiTietHoaDons.Insert(index, chiTietTonTai);
                    
                    System.Diagnostics.Debug.WriteLine($"Đã cập nhật số lượng sản phẩm (tạm): {SanPhamDuocChon.TenSP}, SoLuong mới: {chiTietTonTai.SoLuong}");
                }
                else
                {
                    // Thêm chi tiết hóa đơn mới (CHỈ TRONG MEMORY)
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
                    System.Diagnostics.Debug.WriteLine($"Đã thêm sản phẩm mới (tạm): {SanPhamDuocChon.TenSP}, SoLuong: {SoLuong}");
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
