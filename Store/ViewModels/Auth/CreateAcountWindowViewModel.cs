using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Store.ViewModels.Auth
{
    public partial class CreateAcountWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string tenDangNhap;
        [ObservableProperty] private string matKhau;
        [ObservableProperty] private string hoTen;
        [ObservableProperty] private string email;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private DateTime? ngaySinh = DateTime.Now;
        [ObservableProperty] private string gioiTinh = "Nam";

        // ✅ đường dẫn để lưu DB
        [ObservableProperty] private string hinhAnhPath;

        // ✅ ảnh hiển thị trên UI
        [ObservableProperty] private Bitmap hinhAnh = new Bitmap(AssetLoader.Open(new Uri("avares://Store/Assets/images/AnhMau_2.png")));

        public ObservableCollection<string> DanhSachGioiTinh { get; } = new()
        {
            "Nam",
            "Nữ",
            "Khác"
        };

        [RelayCommand]
        private void DangKyButton()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(TenDangNhap) || string.IsNullOrWhiteSpace(MatKhau) || 
                    string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Email))
                {
                    System.Diagnostics.Debug.WriteLine("Vui lòng điền đầy đủ thông tin!");
                    return;
                }

                var user = new User
                {
                    TenDangNhap = TenDangNhap,
                    MatKhau = MatKhau,
                    HoTen = HoTen,
                    Email = Email,
                    SDT = SDT ?? "",
                    DiaChi = DiaChi ?? "",
                    NgaySinh = NgaySinh,
                    GioiTinh = GioiTinh,
                    HinhAnh = HinhAnhPath, // ✅ Lưu đường dẫn ảnh
                    MaVT = "VT01" 
                };
                UserService.InsertUser(user);

                System.Diagnostics.Debug.WriteLine($"✅ Đã tạo tài khoản thành công: {HoTen}");

                // Đóng window sau khi tạo thành công
                var window = GetActiveWindow();
                window?.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi tạo User: {ex.Message}\n{ex.StackTrace}");
            }
        }

        [RelayCommand]
        public async Task ThemAnhButtonAsync()
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Chọn ảnh đại diện",
                AllowMultiple = false,
                Filters =
                {
                    new FileDialogFilter() { Name = "Ảnh", Extensions = { "png", "jpg", "jpeg", "bmp" } }
                }
            };

            var window = GetActiveWindow();
            if (window == null)
            {
                System.Diagnostics.Debug.WriteLine("Không tìm thấy cửa sổ hoạt động để mở dialog.");
                return;
            }

            var result = await dialog.ShowAsync(window);
            if (result != null && result.Length > 0)
            {
                string selectedPath = result[0];

                // ✅ Copy ảnh vào thư mục riêng của app (ví dụ "Images")
                string imageDir = Path.Combine(AppContext.BaseDirectory, "Images");
                Directory.CreateDirectory(imageDir);

                string destPath = Path.Combine(imageDir, Path.GetFileName(selectedPath));
                File.Copy(selectedPath, destPath, overwrite: true);

                // ✅ Gán để hiển thị và lưu DB
                HinhAnhPath = destPath;
                HinhAnh = new Bitmap(destPath);

                System.Diagnostics.Debug.WriteLine($"Ảnh đã chọn: {HinhAnhPath}");
            }
        }

        private Window? GetActiveWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.Windows.FirstOrDefault(w => w.IsActive);
            return null;
        }
    }
}
