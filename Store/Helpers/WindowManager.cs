using Avalonia.Controls;
using Store.Models;
using Store.ViewModels.Bill;
using Store.ViewModels.Customer;
using Store.ViewModels.Import;
using Store.ViewModels.Product;
using Store.Views;
using Store.Views.Auth;
using Store.Views.Bill;
using Store.Views.Product;
using Store.Views.Customer;
using Store.Views.Import;
using Store.Views.Manager;
using Store.Views.Product;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Store.Helpers
{
    public static class WindowManager
    {
        private static readonly Dictionary<string, Window> _openWindows = new();

        public static void ShowCreateAccountWindow()
        {
            ShowSingletonWindow("CreateAccount", () => new CreateAcountWindowView());
        }

        public static void ShowCreateBillWindow()
        {
            ShowSingletonWindow("CreateBill", () => new CreateBillWindowView());
        }

        public static void ShowCreateProductWindow()
        {
            ShowSingletonWindow("CreateProduct", () => new CreateProductWindowView());
        }

        public static void ShowCreateCustomerWindow()
        {
            ShowSingletonWindow("CreateCustomer", () => new CreateCustomerWindowView());
        }
        public static void ShowCreateImportWindow()
        {
            ShowSingletonWindow("CreateImport", () => new CreateImportWindowView());
        }
        public static void ShowBillDetailWindow(HoaDon hoaDon)
        {
            const string key = "BillDetailWindow";

            if (_openWindows.TryGetValue(key, out var existingWindow))
            {
                if (existingWindow.DataContext is BillDetailWindowViewModel vm)
                {
                    vm.SetHoaDon(hoaDon); // cập nhật dữ liệu mới
                }

                existingWindow.Activate();
                return;
            }

            var window = new BillDetailWindowView
            {
                DataContext = new BillDetailWindowViewModel(hoaDon)
            };

            _openWindows[key] = window;

            window.Closed += (_, __) => _openWindows.Remove(key);
            window.Show();
        }
        public static void ShowCustomerDetailWindow(KhachHang khachHang)
        {
            const string key = "CustomerDetailWindow";
            if (_openWindows.TryGetValue(key, out var existingWindow))
            {
                if (existingWindow.DataContext is CustomerDetailWindowViewModel vm)
                {
                    vm.SetKhachHang(khachHang); // cập nhật dữ liệu mới
                }
                existingWindow.Activate();
                return;
            }
            var window = new CustomerDetailWindowView
            {
                DataContext = new CustomerDetailWindowViewModel(khachHang)
            };
            _openWindows[key] = window;
            window.Closed += (_, __) => _openWindows.Remove(key);
            window.Show();
        }
        public static void ShowImportDetailWindow(Import phieuNhap)
        {
            const string key = "ImportDetailWindow";
            if (_openWindows.TryGetValue(key, out var existingWindow))
            {
                if (existingWindow.DataContext is ImportDetailWindowViewModel vm)
                {
                    vm.SetImport(phieuNhap); // cập nhật dữ liệu mới
                }
                existingWindow.Activate();
                return;
            }
            var window = new ImportDetailWindowView
            {
                DataContext = new ImportDetailWindowViewModel(phieuNhap)
            };
            _openWindows[key] = window;
            window.Closed += (_, __) => _openWindows.Remove(key);
            window.Show();
        }
        public static void ShowProductDetailWindow(SanPham sp)
        {
            const string key = "ProductDetailWindow";
            if (_openWindows.TryGetValue(key, out var existingWindow))
            {
                if (existingWindow.DataContext is ProductDetailWindowViewModel vm)
                {
                    vm.SetProduct(sp); // cập nhật dữ liệu mới
                }
                existingWindow.Activate();
                return;
            }
            var window = new ProductDetailWindowView
            {
                DataContext = new ProductDetailWindowViewModel(sp)
            };
            _openWindows[key] = window;
            window.Closed += (_, __) => _openWindows.Remove(key);
            window.Show();
        }
        private static void ShowSingletonWindow(string windowKey, System.Func<Window> createWindow)
        {
            // Kiểm tra xem cửa sổ đã mở chưa
            if (_openWindows.ContainsKey(windowKey))
            {
                var existingWindow = _openWindows[windowKey];
                
                // Nếu cửa sổ vẫn còn mở, đưa lên foreground
                if (existingWindow.IsVisible)
                {
                    existingWindow.Activate();
                    existingWindow.BringIntoView();
                    return;
                }
                else
                {
                    // Nếu cửa sổ đã đóng, xóa khỏi dictionary
                    _openWindows.Remove(windowKey);
                }
            }

            // Tạo cửa sổ mới
            var window = createWindow();
            _openWindows[windowKey] = window;

            // Đăng ký sự kiện đóng cửa sổ để cleanup
            window.Closed += (sender, args) =>
            {
                if (_openWindows.ContainsKey(windowKey))
                {
                    _openWindows.Remove(windowKey);
                }
            };

            window.Show();
        }

        public static void CloseWindow(string windowKey)
        {
            if (_openWindows.ContainsKey(windowKey))
            {
                _openWindows[windowKey].Close();
                _openWindows.Remove(windowKey);
            }
        }

        public static bool IsWindowOpen(string windowKey)
        {
            return _openWindows.ContainsKey(windowKey) && _openWindows[windowKey].IsVisible;
        }
    }
}