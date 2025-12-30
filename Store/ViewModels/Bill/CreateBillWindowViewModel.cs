using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.SkiaSharpView.Avalonia;
using Store.Helpers;
using Store.Messages;
using Store.Models;
using Store.Services;
using Store.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Store.ViewModels.Auth.MainWindowViewModel;



namespace Store.ViewModels.Bill
{
    public partial class CreateBillWindowViewModel : ViewModelBase, 
        IRecipient<LoginSuccessMessage>
    {
        [ObservableProperty] private int soLuong;

        [ObservableProperty] private decimal giaSP;

        [ObservableProperty] private int khuyenMai;

        [ObservableProperty] private decimal tongGiamGia;

        [ObservableProperty] private decimal tongTriGia;

        [ObservableProperty] private decimal tongThanhTien;

        private decimal tongVon;
        private string? _maDN;

        public Window? ParentWindow { get; set; }

        [ObservableProperty] private string thoiGianHienTai = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        [ObservableProperty] private int soHD;
        [ObservableProperty] private ObservableCollection<ChiTiet_HoaDon> chiTietHoaDons = new();

        [ObservableProperty] private ObservableCollection<KhachHang> danhSachKhachHang = new();
        [ObservableProperty] private ObservableCollection<SanPham> danhSachSanPham = new();
       

        [ObservableProperty] private KhachHang? khachHangDuocChon;
        [ObservableProperty] private SanPham? sanPhamDuocChon;
        [ObservableProperty] private string nhanVien = "";

        public WindowNotificationManager? NotificationManager { get; set; }

        private string MaHD;
        private bool isHoaDonCreated = false;
        private User? currentUser;

        public void Receive(LoginSuccessMessage message)
        {
            _maDN = message.MaDN;
            LoadUserInfo();
        }
        private void LoadUserInfo()
        {
            if (!string.IsNullOrEmpty(_maDN))
            {
                currentUser = UserService.GetOneUser(_maDN);
                if (currentUser != null)
                {
                    NhanVien = _maDN + " | " + currentUser.HoTen;
                }
            }
        }

        public CreateBillWindowViewModel()
        {
            // Đăng ký nhận message
            WeakReferenceMessenger.Default.Register<LoginSuccessMessage>(this);
            
            LoadKhachHang();
            LoadSanPham();
            
            // Thử load thông tin user từ session hiện tại nếu có
            TryLoadCurrentUser();

            // Kiểm tra xem có hóa đơn nháp không
            if (DraftBillManager.HasDraft)
            {
                // Load hóa đơn nháp
                var draft = DraftBillManager.LoadDraft();
                MaHD = draft.MaHD;
                SoHD = draft.SoHD;
                ChiTietHoaDons = draft.Items;
                KhachHangDuocChon = draft.KhachHang;
                currentUser = draft.NhanVien;
                
                // Cập nhật hiển thị nhân viên
                if (currentUser != null)
                {
                    NhanVien = currentUser.MaNV + " | " + currentUser.HoTen;
                }

                CapNhatTongTien();
                System.Diagnostics.Debug.WriteLine(
                    $"[ViewModel] Đã load hóa đơn nháp: {MaHD}, {ChiTietHoaDons.Count} sản phẩm");
            }
            else
            {
                // Tạo hóa đơn mới
                MaHD = OrderService.GenerateNewOrderID();
                SoHD = OrderService.GetNextOrderNumber();
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Khởi tạo hóa đơn mới: {MaHD}, SoHD: {SoHD}");
            }

            SoLuong = 1; // Khởi tạo số lượng mặc định
        }

        private void TryLoadCurrentUser()
        {
            try
            {
                // Thử lấy thông tin user từ session hoặc config nếu có
                // Đây là fallback khi không có LoginSuccessMessage
                var allUsers = UserService.GetAllUser();
                if (allUsers.Any())
                {
                    // Tạm thời lấy user đầu tiên làm default (có thể cải thiện logic này)
                    currentUser = allUsers.First();
                    _maDN = currentUser.MaNV;
                    NhanVien = currentUser.MaNV + " | " + currentUser.HoTen;
                    System.Diagnostics.Debug.WriteLine($"[TryLoadCurrentUser] Loaded default user: {currentUser.HoTen}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TryLoadCurrentUser] Lỗi: {ex.Message}");
            }
        }

        // Lưu nháp khi đóng window (nếu chưa thanh toán)
        public void OnWindowClosing()
        {
            if (!isHoaDonCreated && ChiTietHoaDons.Count > 0)
            {
                DraftBillManager.SaveDraft(MaHD, SoHD, ChiTietHoaDons, KhachHangDuocChon, currentUser);
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Đã lưu nháp khi đóng window");
            }
        }


        // Thanh toán và xuất hóa đơn
        [RelayCommand]
        private async Task ThanhToan()
        {
            // Kiểm tra điều kiện
            if (KhachHangDuocChon == null)
            {
                NotificationManager?.Show("Vui lòng chọn khách hàng", NotificationType.Warning);
                System.Diagnostics.Debug.WriteLine("[ThanhToan] Chưa chọn khách hàng");
                return;
            }

            if (currentUser == null)
            {
                NotificationManager?.Show("Lỗi: Không xác định được nhân viên", NotificationType.Error);
                System.Diagnostics.Debug.WriteLine("[ThanhToan] Chưa có thông tin nhân viên");
                return;
            }

            if (ChiTietHoaDons.Count == 0)
            {
                NotificationManager?.Show("Vui lòng thêm sản phẩm vào hóa đơn", NotificationType.Warning);
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
                    TongVon = tongVon,
                    MaKH = KhachHangDuocChon.MaKH,
                    MaUser = currentUser.MaNV,
                    SoHD = SoHD,
                    TrangThaiHD = "Đã thanh toán"
                };
                OrderService.InsertOrder(hoaDon);
                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Đã tạo hóa đơn: {MaHD}");

                KhachHangDuocChon.TongMua += TongThanhTien;
                CustomerService.UpdateCustomer(KhachHangDuocChon);

                // Bước 2: Lưu tất cả chi tiết hóa đơn vào database
                foreach (var chiTiet in ChiTietHoaDons)
                {
                    DetailOrderService.InsertOderDetail(chiTiet);
                    ProductService.TruSoLuongSanPham(chiTiet.MaSP, chiTiet.SoLuong);
                }

                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Đã lưu {ChiTietHoaDons.Count} chi tiết hóa đơn");

                // Bước 3: hiện thông báo và xuất file hóa đơn PDF
                NotificationManager?.Show("Thêm đơn hàng thành công", NotificationType.Success);

                // Gửi message cập nhật
                WeakReferenceMessenger.Default.Send(new HoaDonChangedMessage(MaHD));
                WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage(KhachHangDuocChon.MaKH));

                await Task.Delay(1500);

                await XuatFileHoaDon();

                System.Diagnostics.Debug.WriteLine($"[ThanhToan] Hoàn tất thanh toán hóa đơn {MaHD}");

                // Bước 4: Đánh dấu đã thanh toán và xóa nháp
                isHoaDonCreated = true;
                DraftBillManager.ClearDraft();

                // Gửi message cập nhật
                WeakReferenceMessenger.Default.Send(new HoaDonChangedMessage(MaHD));
                WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage(KhachHangDuocChon.MaKH));
                WeakReferenceMessenger.Default.Send(new SanPhamChangedMessage("Updated"));

                ParentWindow?.Close();
            }
            catch (Exception ex)
            {
                NotificationManager?.Show($"Lỗi khi thanh toán: {ex.Message}", NotificationType.Error);
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
                        tenNhanVien: currentUser?.HoTen ?? "N/A",
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
            var ds = CustomerService.GetAllCustomer();
            danhSachKhachHang = new ObservableCollection<KhachHang>(ds);
        }

        private void LoadSanPham()
        {
            var ds1 = ProductService.GetAllProduct();
            danhSachSanPham = new ObservableCollection<SanPham>(ds1);
        }

        

        [RelayCommand]
        private void Tang()
        {
            if (SanPhamDuocChon == null)
            {
                return;
            }
            if (SoLuong < SanPhamDuocChon.SoLuongSP)
            {
                SoLuong++;
            }
            else
            {
                return;
            }
        }

        [RelayCommand]
        private void GiamKM()
        {
            if (KhuyenMai > 1)
            {
                KhuyenMai--;
            }
            else
            {
                return;
            }
        }

        [RelayCommand]
        private void TangKM()
        {
            if (SanPhamDuocChon == null)
            {
                return;
            }
            if(KhuyenMai < 100)
            {
                KhuyenMai++;
            }
            else
            {
                return ;
            }
        }

        [RelayCommand]
        private void Giam()
        {
            if (SoLuong > 1)
            {
                SoLuong--;
            }
            else
            {
                return;
            }
        }


        [RelayCommand]
        private void ThemSanPham()
        {
            // Kiểm tra sản phẩm được chọn
            if (SanPhamDuocChon == null)
            {
                NotificationManager?.Show("Vui lòng chọn sản phẩm", NotificationType.Warning);
                System.Diagnostics.Debug.WriteLine("Chưa chọn sản phẩm");
                return;
            }

            // Kiểm tra số lượng hợp lệ
            if (SoLuong <= 0)
            {
                NotificationManager?.Show("Số lượng phải lớn hơn 0", NotificationType.Warning);
                System.Diagnostics.Debug.WriteLine("Số lượng phải lớn hơn 0");
                return;
            }

            // Kiểm tra số lượng tồn kho
            if (SoLuong > SanPhamDuocChon.SoLuongSP)
            {
                NotificationManager?.Show($"Số lượng vượt quá tồn kho ({SanPhamDuocChon.SoLuongSP})", NotificationType.Warning);
                System.Diagnostics.Debug.WriteLine($"Số lượng vượt quá tồn kho ({SanPhamDuocChon.SoLuongSP})");
                return;
            }

            try
            {
                // Tính thành tiền
                decimal donGia = SanPhamDuocChon.GiaSP;
                decimal giamGia = donGia * KhuyenMai / 100;
                decimal thanhTien = (donGia - giamGia) * SoLuong;

                System.Diagnostics.Debug.WriteLine($"[ThemSanPham] DonGia: {donGia}, KhuyenMai: {KhuyenMai}%, GiamGia: {giamGia}, SoLuong: {SoLuong}, ThanhTien: {thanhTien}");

                // Kiểm tra sản phẩm đã tồn tại trong chi tiết hóa đơn chưa
                var chiTietTonTai = ChiTietHoaDons.FirstOrDefault(ct => ct.MaSP == SanPhamDuocChon.MaSP);

                if (chiTietTonTai != null)
                {
                    // Kiểm tra tổng số lượng sau khi cộng thêm
                    int tongSoLuongMoi = chiTietTonTai.SoLuong + SoLuong;
                    if (tongSoLuongMoi > SanPhamDuocChon.SoLuongSP)
                    {
                        NotificationManager?.Show($"Tổng số lượng ({tongSoLuongMoi}) vượt quá tồn kho ({SanPhamDuocChon.SoLuongSP})", NotificationType.Warning);
                        return;
                    }

                    // Nếu đã có, cập nhật số lượng và thành tiền (CHỈ TRONG MEMORY)
                    chiTietTonTai.SoLuong = tongSoLuongMoi;
                    chiTietTonTai.KhuyenMai = KhuyenMai; // Cập nhật khuyến mãi mới
                    decimal giamGiaItem = chiTietTonTai.DonGia * chiTietTonTai.KhuyenMai / 100;
                    chiTietTonTai.ThanhTien = (chiTietTonTai.DonGia - giamGiaItem) * chiTietTonTai.SoLuong;

                    // Xóa và thêm lại để trigger UI update
                    var index = ChiTietHoaDons.IndexOf(chiTietTonTai);
                    ChiTietHoaDons.RemoveAt(index);
                    ChiTietHoaDons.Insert(index, chiTietTonTai);

                    System.Diagnostics.Debug.WriteLine(
                        $"Đã cập nhật sản phẩm: {SanPhamDuocChon.TenSP}, SoLuong: {chiTietTonTai.SoLuong}, KhuyenMai: {chiTietTonTai.KhuyenMai}%, ThanhTien: {chiTietTonTai.ThanhTien}");
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
                    System.Diagnostics.Debug.WriteLine(
                        $"Đã thêm sản phẩm mới (tạm): {SanPhamDuocChon.TenSP}, SoLuong: {SoLuong}");
                }

                // Cập nhật tổng tiền sau khi thêm/cập nhật
                CapNhatTongTien();

                // Reset form
                SoLuong = 1;
                KhuyenMai = 0;
                SanPhamDuocChon = null;
                GiaSP = 0;

                NotificationManager?.Show("Đã thêm sản phẩm vào hóa đơn", NotificationType.Success);
            }
            catch (Exception ex)
            {
                NotificationManager?.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", NotificationType.Error);
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thêm chi tiết hóa đơn: {ex.Message}");
            }
        }

        // Phương thức cập nhật tổng tiền
        private void CapNhatTongTien()
        {
            try
            {
                // Tính từ collection thay vì query database
                TongTriGia = ChiTietHoaDons.Sum(ct => ct.DonGia * ct.SoLuong);
                TongGiamGia = ChiTietHoaDons.Sum(ct => (ct.DonGia * ct.SoLuong * ct.KhuyenMai / 100));
                TongThanhTien = ChiTietHoaDons.Sum(ct => ct.ThanhTien);
                
                // Tính tổng vốn với xử lý lỗi
                tongVon = 0;
                foreach (var ct in ChiTietHoaDons)
                {
                    try
                    {
                        var sanPham = ProductService.GetProduct(ct.MaSP);
                        if (sanPham != null)
                        {
                            tongVon += sanPham.GiaNhap * ct.SoLuong;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CapNhatTongTien] Lỗi khi tính vốn cho sản phẩm {ct.MaSP}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[CapNhatTongTien] TongTriGia: {TongTriGia}, TongGiamGia: {TongGiamGia}, TongThanhTien: {TongThanhTien}, TongVon: {tongVon}");

                // Force notify property changed
                OnPropertyChanged(nameof(TongTriGia));
                OnPropertyChanged(nameof(TongGiamGia));
                OnPropertyChanged(nameof(TongThanhTien));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CapNhatTongTien] Lỗi: {ex.Message}");
            }
        }

        [RelayCommand]
        private void XoaChiTiet()
        {
            ChiTietHoaDons.Clear();
            CapNhatTongTien();
            NotificationManager?.Show("Đã xóa tất cả sản phẩm", NotificationType.Information);
        }

        // Cleanup khi đóng window
        public void Cleanup()
        {
            WeakReferenceMessenger.Default.Unregister<LoginSuccessMessage>(this);
        }
    }
}