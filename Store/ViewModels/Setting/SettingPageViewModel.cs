using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Styling;
namespace Store.ViewModels.Setting;

public partial class SettingPageViewModel : ViewModelBase
{
    public string Setting1 { get; set; } = "Cài đặt";
    
    [ObservableProperty]
    private bool _isDarkMode;

    public SettingPageViewModel()
    {
        // Khi mở trang, kiểm tra xem App đang ở chế độ nào để gạt cần switch cho đúng
        if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
        {
            _isDarkMode = true;
        }
    }

    // Hàm này tự động chạy khi IsDarkMode thay đổi (tính năng của CommunityToolkit)
    partial void OnIsDarkModeChanged(bool value)
    {
        var app = Application.Current;
        if (app is not null)
        {
            app.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }
}