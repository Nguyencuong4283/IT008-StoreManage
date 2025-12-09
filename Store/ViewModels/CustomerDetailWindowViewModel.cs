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

namespace Store.ViewModels
{
    public partial class CustomerDetailWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string maKH;
        [ObservableProperty] private string tenKH;
        [ObservableProperty] private string sDT;
        [ObservableProperty] private string gioiTinh;
        [ObservableProperty] private string diaChi;
        [ObservableProperty] private string ghiChu;
       
      
        public CustomerDetailWindowViewModel() { }
        KhachHang KH { get; set; }
      
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
            }
            KH = khachhang;
        }

        [RelayCommand]
        private async Task XoaKhachHangButton()
        {
            try
            {
                if (string.IsNullOrEmpty(MaKH))
                {
                    System.Diagnostics.Debug.WriteLine("Không có mã khách hàng để xóa");
                    return;
                }

                // Hiển thị dialog xác nhận
                if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
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
                    KhachHangService.UpdateKhachHang(KH);
                    System.Diagnostics.Debug.WriteLine($"✅ Đã xóa khách hàng: {MaKH}");

                    // Gửi message
                    WeakReferenceMessenger.Default.Send(new KhachHangChangedMessage(MaKH));
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
            var dialog = new Avalonia.Controls.Window
            {
                Title = title,
                Width = 400,
                Height = 180,
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var stackPanel = new Avalonia.Controls.StackPanel
            {
                Margin = new Avalonia.Thickness(20),
                Spacing = 20
            };

            var messageText = new Avalonia.Controls.TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                FontSize = 14
            };

            var buttonPanel = new Avalonia.Controls.StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 10
            };

            bool result = false;

            var yesButton = new Avalonia.Controls.Button
            {
                Content = "Có",
                Width = 100,
                Height = 35
            };
            yesButton.Click += (s, e) =>
            {
                result = true;
                dialog.Close();
            };

            var noButton = new Avalonia.Controls.Button
            {
                Content = "Không",
                Width = 100,
                Height = 35
            };
            noButton.Click += (s, e) =>
            {
                result = false;
                dialog.Close();
            };

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);

            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(buttonPanel);

            dialog.Content = stackPanel;

            await dialog.ShowDialog(owner);
            return result;
        }
    }
}
