using Avalonia.Controls;
using Store.Views;
using Store.Views.Auth;
using Store.Views.Bill;
using Store.Views.Customer;
using Store.Views.Product;
using Store.Views.Import;
using Store.Views.Manager;
using System.Collections.Generic;

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