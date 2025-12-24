using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace Store.ViewModels.Auth
{
    public partial class EditAccountWindowViewModel : ViewModelBase
    {
        private string _maNV;
        private string _maVT;
        private int _isDelete;
        
        [ObservableProperty] private string tenDangNhap;
        [ObservableProperty] private string matKhau;
        [ObservableProperty] private string hoTen;
        [ObservableProperty] private string email;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private DateTime? ngaySinh;
        [ObservableProperty] private string gioiTinh;
        [ObservableProperty] private string chucVu;
        User NV { get; set; }
        
        public WindowNotificationManager? NotificationManager { get; set; }
        
        public ObservableCollection<string> DanhSachChucVu { get; } = new()
        {
           "Nhân Viên Bán Hàng",
           "Quản Lý"
        };
        public Window? ParentWindow { get; set; }

        // Constructor mặc định cho XAML designer
        public EditAccountWindowViewModel() { }

        public EditAccountWindowViewModel(User user)
        {
            if(user != null)
            {
                _maNV = user.MaNV;
                _maVT = user.MaVT;
                _isDelete = user.IsDelete;
                
                tenDangNhap = user.TenDangNhap;
                if(user.MaVT == "VT02")
                    chucVu = "Nhân Viên Bán Hàng";
                else if (user.MaVT == "VT01")
                    chucVu = "Quản Lý";
                hoTen = user.HoTen;
                email = user.Email;
                sDT = user.SDT;
                diaChi = user.DiaChi;
                ngaySinh = user.NgaySinh;
                gioiTinh = user.GioiTinh;
                hinhAnhPath = user.HinhAnh;
                
                // Load ảnh nếu có đường dẫn
                if (!string.IsNullOrEmpty(hinhAnhPath) && File.Exists(hinhAnhPath))
                {
                    hinhAnh = new Bitmap(hinhAnhPath);
                }

            }
            NV = user;
        }
        // ✅ đường dẫn để lưu DB
        [ObservableProperty] private string hinhAnhPath;

        // ✅ ảnh hiển thị trên UI
        [ObservableProperty] private Bitmap hinhAnh = new Bitmap(AssetLoader.Open(new Uri("avares://Store/Assets/images/AnhMau_2.png")));


        [RelayCommand]
        private async Task DangKyButton()
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(TenDangNhap)  ||
                    string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Email))
                {
                    System.Diagnostics.Debug.WriteLine("Vui lòng điền đầy đủ thông tin!");
                    return;
                }

                var user = new User
                {
                    MaNV = _maNV,
                    TenDangNhap = TenDangNhap,
                    HoTen = HoTen,
                    Email = Email,
                    SDT = SDT ?? "",
                    DiaChi = DiaChi ?? "",
                    NgaySinh = NgaySinh.Value,
                    GioiTinh = GioiTinh ?? "",
                    HinhAnh = HinhAnhPath ?? "",
                    MaVT = _maVT ?? "VT01",
                    IsDelete = _isDelete
                };
                if (chucVu == "Nhân Viên Bán Hàng")
                    user.MaVT = "VT02";
                else if (chucVu == "Quản Lý")
                    user.MaVT = "VT01";
                UserService.UpdateUser(user, updatePassword: false);

                System.Diagnostics.Debug.WriteLine($"✅ Đã tạo cập nhật  thành công: {HoTen}");
                
                NotificationManager?.Show("✅ Cập nhật tài khoản thành công!", NotificationType.Success);
                
                WeakReferenceMessenger.Default.Send(new AccountChangeMessage("Update"));
                
                await Task.Delay(1000);

                if (ParentWindow != null)
                {
                    ParentWindow.Close();
                }
                else
                {
                    var activeWindow = GetActiveWindow();
                    activeWindow?.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi cập nhật User: {ex.Message}\n{ex.StackTrace}");
                
                NotificationManager?.Show("Cập nhật tài khoản không thành công", NotificationType.Error);
            }
        }

        [RelayCommand]
        public async Task InsertPictureAsyncButton()
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
        [RelayCommand]
        private async Task Delete()
        {
            try
            {
                if (string.IsNullOrEmpty(_maNV))
                {
                    System.Diagnostics.Debug.WriteLine("Không có mã nhân viên  để xóa");
                    return;
                }

                // Hiển thị dialog xác nhận
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var window = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                    if (window != null)
                    {
                        var result = await ShowConfirmDialog(window,
                            "Xác nhận xóa",
                            $"Bạn có chắc chắn muốn xóa nhân viên  '{_maNV}' không?");

                        if (!result)
                        {
                            System.Diagnostics.Debug.WriteLine("Người dùng đã hủy xóa nhân viên");
                            return;
                        }
                    }

                    NV.IsDelete = 1;
                    UserService.UpdateUser(NV);
                    System.Diagnostics.Debug.WriteLine($"✅ Đã xóa nhân viên: {_maNV}");
                    
                    // Gửi message
                    WeakReferenceMessenger.Default.Send(new NhanVienChangedMessage(_maNV));
                    WeakReferenceMessenger.Default.Send(new AccountChangeMessage("Delete"));
                    
                    // Đóng window sau khi xóa
                    var closeWindow = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                    closeWindow?.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi xóa nhân viên: {ex.Message}");
                
                NotificationManager?.Show("Xóa nhân viên không thành công", NotificationType.Error);
            }
        }

        private async Task<bool> ShowConfirmDialog(Avalonia.Controls.Window owner, string title, string message)
        {
            // 1. Cấu hình Window: Trong suốt, không viền hệ thống
            var dialog = new Avalonia.Controls.Window
            {
                Title = title,
                Width = 400,
                Height = 200, // Tăng chiều cao một chút cho thoáng
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
                CanResize = false,
                SystemDecorations = Avalonia.Controls.SystemDecorations.None, // Tắt thanh tiêu đề mặc định
                TransparencyLevelHint = new[] { Avalonia.Controls.WindowTransparencyLevel.Transparent },
                Background = Avalonia.Media.Brushes.Transparent, // Nền window trong suốt để hiện bo góc
                ExtendClientAreaToDecorationsHint = true
            };

            // 2. Tạo khung chính (Card) có bo góc và đổ bóng
            var mainBorder = new Avalonia.Controls.Border
            {
                CornerRadius = new Avalonia.CornerRadius(12), // Bo tròn góc
                BoxShadow = new Avalonia.Media.BoxShadows(new Avalonia.Media.BoxShadow
                {
                    Blur = 20,
                    OffsetY = 5,
                    Color = Avalonia.Media.Color.Parse("#40000000") // Bóng mờ màu đen
                }),
                ClipToBounds = true // Đảm bảo nội dung con không bị chờm ra ngoài góc bo
            };
            
            mainBorder.Bind(Avalonia.Controls.Border.BackgroundProperty, owner.GetResourceObservable("BgCard"));

            // 3. Layout chính dùng Grid: 3 dòng (Header, Nội dung, Nút bấm)
            var grid = new Avalonia.Controls.Grid
            {
                RowDefinitions = Avalonia.Controls.RowDefinitions.Parse("45,*,Auto")
            };

            // --- Header (Tiêu đề) ---
            // Sử dụng Gradient tím xanh giống AdminWindow của bạn
            var headerBorder = new Avalonia.Controls.Border
            {
                Background = new Avalonia.Media.LinearGradientBrush
                {
                    StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                    EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                    GradientStops = new Avalonia.Media.GradientStops
                    {
                        new Avalonia.Media.GradientStop(Avalonia.Media.Color.Parse("#667EEA"), 0),
                        new Avalonia.Media.GradientStop(Avalonia.Media.Color.Parse("#764BA2"), 1)
                    }
                },
                Padding = new Avalonia.Thickness(15, 0)
            };

            var titleText = new Avalonia.Controls.TextBlock
            {
                Text = title,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = Avalonia.Media.Brushes.White,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = 16
            };
            headerBorder.Child = titleText;

            // --- Nội dung thông báo ---
            var messageText = new Avalonia.Controls.TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                FontSize = 15,
                Margin = new Avalonia.Thickness(20),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                TextAlignment = Avalonia.Media.TextAlignment.Center
            };

            // --- Khu vực nút bấm ---
            var buttonPanel = new Avalonia.Controls.StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Spacing = 10,
                Margin = new Avalonia.Thickness(0, 0, 20, 20)
            };

            bool result = false;

            // Nút Hủy (Màu xám)
            var noButton = new Avalonia.Controls.Button
            {
                Content = "Hủy bỏ",
                Width = 100,
                Height = 35,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                CornerRadius = new Avalonia.CornerRadius(6)
            };
            
            noButton.Click += (s, e) =>
            {
                result = false;
                dialog.Close();
            };

            noButton.Bind(Avalonia.Controls.Button.BackgroundProperty, owner.GetResourceObservable("BgHover"));
            noButton.Bind(Avalonia.Controls.Button.ForegroundProperty, owner.GetResourceObservable("TextPrimary"));
            
            // Nút Đồng ý (Màu đỏ để cảnh báo Xóa)
            var yesButton = new Avalonia.Controls.Button
            {
                Content = "Xóa",
                Width = 100,
                Height = 35,
                Background = Avalonia.Media.Brushes.Red,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                CornerRadius = new Avalonia.CornerRadius(6)
            };
            yesButton.Click += (s, e) =>
            {
                result = true;
                dialog.Close();
            };
            
            yesButton.Bind(Avalonia.Controls.Button.ForegroundProperty, owner.GetResourceObservable("TextPrimary"));
            
            // Thêm nút (Lưu ý: Thêm Hủy trước, Xóa sau để Xóa nằm ngoài cùng bên phải)
            buttonPanel.Children.Add(noButton);
            buttonPanel.Children.Add(yesButton);

            // Ghép các phần lại với nhau
            Avalonia.Controls.Grid.SetRow(headerBorder, 0);
            Avalonia.Controls.Grid.SetRow(messageText, 1);
            Avalonia.Controls.Grid.SetRow(buttonPanel, 2);

            grid.Children.Add(headerBorder);
            grid.Children.Add(messageText);
            grid.Children.Add(buttonPanel);

            mainBorder.Child = grid;
            dialog.Content = mainBorder;

            // Helper extension để convert color string sang Brush
            // Nếu bạn chưa có extension method ToBrush, có thể dùng trực tiếp Brushes.Parse hoặc SolidColorBrush

            await dialog.ShowDialog(owner);
            return result;
        }
    
        private Window? GetActiveWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return desktop.Windows.FirstOrDefault(w => w.IsActive);
            return null;
        }
    }
}
