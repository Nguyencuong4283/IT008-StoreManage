using Avalonia;
using System;
using Store.Services; // ✅ thêm dòng này

namespace Store;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // ✅ Khởi tạo CSDL (tạo file store.db + bảng nếu chưa có)
        ProductService.Initialize();
        UserService.Initialize();
        CustomerService.Initialize();
        OrderService.Initialize();
        DetailOrderService.Initialize();
        ImportService.Initialize();

        // ✅ Sau đó mới khởi chạy ứng dụng
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
