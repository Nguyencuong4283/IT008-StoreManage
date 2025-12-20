using Avalonia.Media.Imaging;
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


namespace Store.ViewModels.Product
{
    public partial class ProductDetailWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private Bitmap hinhAnhSP;
        [ObservableProperty] private string tenSP;
        [ObservableProperty] private decimal giaSP;
        [ObservableProperty] private int soLuongSP;
        [ObservableProperty] private string maSP;
        [ObservableProperty] private string loaiSP;
        [ObservableProperty] private string kichCoSP;
        [ObservableProperty] private string moTaSP;

        public ProductDetailWindowViewModel()
        {
            // Constructor mặc định cho Design.DataContext
        }
        SanPham SP { get; set; }

        public ProductDetailWindowViewModel(SanPham sanPham)
        {
            if (sanPham != null)
            {
                HinhAnhSP = sanPham.HinhAnhSP;
                TenSP = sanPham.TenSP;
                GiaSP = sanPham.GiaSP;
                SoLuongSP = sanPham.SoLuongSP;
                MaSP = sanPham.MaSP;
                LoaiSP = sanPham.LoaiSP;
                KichCoSP = sanPham.KichThuocSP;
                MoTaSP = sanPham.MoTaSP;
            }
            SP = sanPham;
        }
        [RelayCommand]
        private async Task Delete()
        {
            try
            {
                if (string.IsNullOrEmpty(MaSP))
                {
                    System.Diagnostics.Debug.WriteLine("Không có sản phẩm để xóa");
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
                            $"Bạn có chắc chắn muốn xóa sản phẩm '{TenSP}' không?");

                        if (!result)
                        {
                            System.Diagnostics.Debug.WriteLine("Người dùng đã hủy xóa sản phẩm");
                            return;
                        }
                    }

                    SP.IsDelete = 1;
                    ProductService.UpdateProduct(SP);
                    System.Diagnostics.Debug.WriteLine($"✅ Đã xóa san pham: {MaSP}");

                    // Gửi message
                    WeakReferenceMessenger.Default.Send(new SanPhamChangedMessage(maSP));
                    // Đóng window sau khi xóa
                    var closeWindow = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                    closeWindow?.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tạo sản phẩm: {ex.Message}");
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
