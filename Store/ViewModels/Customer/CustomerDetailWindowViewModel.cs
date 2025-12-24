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
    public partial class CustomerDetailWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string maKH;
        [ObservableProperty] private string tenKH;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string gioiTinh;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private string ghiChu;
        [ObservableProperty] private decimal tongMua;
        [ObservableProperty] private string hang;


        public CustomerDetailWindowViewModel()
        {
        }

        KhachHang KH { get; set; }


        public void SetKhachHang(KhachHang khachhang)
        {
            KH = khachhang;
        }

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
                TongMua = khachhang.TongMua;
                Hang = khachhang.Hang;
            }

            KH = khachhang;
        }

        [RelayCommand]
        private async Task DeleteCustomerButton()
        {
            try
            {
                if (string.IsNullOrEmpty(MaKH))
                {
                    System.Diagnostics.Debug.WriteLine("Không có mã khách hàng để xóa");
                    return;
                }

                // Hiển thị dialog xác nhận
                if (Avalonia.Application.Current?.ApplicationLifetime is
                    Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var window = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                    if (window != null)
                    {
                        var result = await ShowConfirmDialog(window,
                            "Xác nhận xóa",
                            $"Bạn có chắc chắn muốn xóa khách hàng '{TenKH}' không?");

                        if (!result)
                        {
                            System.Diagnostics.Debug.WriteLine("Người dùng đã hủy xóa khách hàng");
                            return;
                        }
                    }

                    KH.IsDelete = 1;
                    CustomerService.UpdateCustomer(KH);
                    System.Diagnostics.Debug.WriteLine($"✅ Đã xóa khách hàng: {MaKH}");

                    // Gửi message
                    WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage(MaKH));
                    WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage("Delete"));
                    // Đóng window sau khi xóa
                    var closeWindow = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                    closeWindow?.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi xóa khách hàng: {ex.Message}");
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

        // Helper nhỏ để chuyển mã Hex sang Brush (để đoạn code trên chạy mượt)
        public static class ColorHelper
        {
            public static Avalonia.Media.IBrush ToBrush(Avalonia.Media.Color color)
            {
                return new Avalonia.Media.SolidColorBrush(color);
            }
        }
    }
}