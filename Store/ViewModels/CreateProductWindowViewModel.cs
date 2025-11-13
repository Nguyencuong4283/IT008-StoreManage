using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Avalonia.Platform;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Store.ViewModels
{
    public partial class CreateProductWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string maSP;
        [ObservableProperty] private string tenSP;
        [ObservableProperty] private decimal giaSP;
        [ObservableProperty] private int soLuongSP = 1;
        [ObservableProperty] private string loaiSP = "Khác";
        [ObservableProperty] private string kichThuocSP = "M";
        [ObservableProperty] private string moTaSP;
        [ObservableProperty] private string hinhAnhDuongDan;
        [ObservableProperty] private Bitmap hinhAnhSP = new Bitmap(AssetLoader.Open(new Uri("avares://Store/Assets/images/AnhMau_1.png")));
        public ObservableCollection<string> DanhSachLoaiSP { get; } = new()
        {
            "Quần ngắn",
            "Quần dài",
            "Áo bà ba",
            "Khác"
        };
        public ObservableCollection<string> DanhSachKichThuocSP { get; } = new()
        {
            "M",
            "L",
            "XL",
            "XXL",
            "FreeSize"
        };
        public CreateProductWindowViewModel()
        {
            // Tự động tạo mã sản phẩm ban đầu
            MaSP = SanPhanService.GenerateNewMaSP();
        }

        [RelayCommand]
        private void TaoSanPham()
        {
            try
            {
                var sanPham = new SanPham
                {
                    MaSP = MaSP,
                    TenSP = TenSP,
                    GiaSP = GiaSP,
                    SoLuongSP = SoLuongSP,
                    LoaiSP = LoaiSP,
                    KichThuocSP = KichThuocSP,
                    MoTaSP = MoTaSP,
                    HinhAnhDuongDan = hinhAnhDuongDan, // Có thể sau này bạn sẽ load từ file
                };
                SanPhanService.InsertSanPham(sanPham);
                // Sau khi thêm thành công
                System.Diagnostics.Debug.WriteLine($"Đã thêm sản phẩm: {TenSP}");
                // Reset form
                TenSP = "";
                GiaSP = 0;
                SoLuongSP = 0;
                LoaiSP = "";
                KichThuocSP = "";
                MoTaSP = "";
                HinhAnhDuongDan = "";
                HinhAnhSP = new Bitmap(AssetLoader.Open(new Uri("avares://Store/Assets/images/AnhMau_1.png")));
                MaSP = SanPhanService.GenerateNewMaSP();


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo sản phẩm: {ex.Message}");
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
                HinhAnhDuongDan = destPath;
                HinhAnhSP = new Bitmap(destPath);

                System.Diagnostics.Debug.WriteLine($"Ảnh đã chọn: {HinhAnhDuongDan}");
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
