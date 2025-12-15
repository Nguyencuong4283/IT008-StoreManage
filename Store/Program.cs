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
        SanPhamService.Initialize();
        UserService.Initialize();
        KhachHangService.Initialize();
        HoaDonService.Initialize();
        ChiTiet_HoaDonService.Initialize();

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
