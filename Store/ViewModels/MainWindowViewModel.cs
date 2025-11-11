using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Store.Views;
using System.Collections.ObjectModel;

namespace Store.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string tenDangNhap;
    [ObservableProperty] private string matKhau;
    [ObservableProperty] private string kiemTraDangNhap;
    [RelayCommand]
    private void LogInButton()
    {
        if (tenDangNhap == null)
        {
            KiemTraDangNhap = "Vui lòng nhập tên đăng nhập!";
            return;
        }
        else if (matKhau == null)
        {
            KiemTraDangNhap = "Vui lòng nhập mật khẩu!";
            return;
        }
        
        var list = UserService.GetAllUser();
        foreach (var user in list)
        {
            if (UserService.VerifyPassword(MatKhau, user.MatKhau) && TenDangNhap == user.TenDangNhap)
            {
                var adminVM = new AdminWindowViewModel();
                var adminWindow = new AdminWindowView();
                // adminWindow.DataContext = adminVM;
                adminWindow.Show();

                // Đóng MainWindow hiện tại
                if (App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    && desktop.MainWindow is Avalonia.Controls.Window mainWindow)
                {
                    mainWindow.Close();
                }
                return; // Thoát khỏi hàm sau khi đăng nhập thành công
            }
        }
        
        // Chỉ hiển thị thông báo lỗi nếu không tìm thấy user phù hợp
        KiemTraDangNhap = "Đăng nhập thất bại !!!";
    }
    
    [RelayCommand]
    private void RegisterButton()
    {
        CreateAcountWindowView createAcountWindowView = new CreateAcountWindowView();
        createAcountWindowView.Show();
    }
    [RelayCommand]
    private void ForgotPasswordButton()
    {
        ForgotPasswordView forgotPasswordView = new ForgotPasswordView();
        forgotPasswordView.Show();
    }


}
