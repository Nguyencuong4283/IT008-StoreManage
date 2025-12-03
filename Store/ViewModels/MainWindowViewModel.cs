using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Models;
using Store.Services;
using Store.Views;
using System;
using System.Collections.ObjectModel;

namespace Store.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string maDN = "";
    public class LoginSuccessMessage
    {
        public string MaDN { get; }
        public LoginSuccessMessage(string maDN) => MaDN = maDN;
    }

    [ObservableProperty] private string tenDangNhap;
    [ObservableProperty] private string matKhau;
    [ObservableProperty] private string kiemTraDangNhap;
    [RelayCommand]
    private void LogInButton()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                KiemTraDangNhap = "Vui lòng nhập tên đăng nhập!";
                return;
            }
            else if (string.IsNullOrWhiteSpace(matKhau))
            {
                KiemTraDangNhap = "Vui lòng nhập mật khẩu!";
                return;
            }
            
            var list = UserService.GetAllUser();
            
            if (list == null || list.Count == 0)
            {
                KiemTraDangNhap = "Không tìm thấy tài khoản nào trong hệ thống!";
                return;
            }
            
            foreach (var user in list)
            {
                if (UserService.VerifyPassword(MatKhau, user.MatKhau) && TenDangNhap == user.TenDangNhap && user.MaVT == "VT01")
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Đăng nhập Admin thành công: {user.HoTen}");
                    
                    var adminWindow = new AdminWindowView();
                    var adminVM = new AdminWindowViewModel();
                    adminWindow.DataContext = adminVM;
                    adminWindow.Show();
                    
                    maDN = user.MaNV;
                    WeakReferenceMessenger.Default.Send(new LoginSuccessMessage(maDN));

                    // Đóng MainWindow hiện tại
                    if (App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                        && desktop.MainWindow is Avalonia.Controls.Window mainWindow)
                    {
                        mainWindow.Close();
                    }
                    return;
                }
                else if (UserService.VerifyPassword(MatKhau, user.MatKhau) && TenDangNhap == user.TenDangNhap && user.MaVT == "VT02")
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Đăng nhập Nhân viên thành công: {user.HoTen}");
                    
                    var staffWindow = new EmployeeWindowView();
                    staffWindow.Show();
                    
                    maDN = user.MaNV;
                    WeakReferenceMessenger.Default.Send(new LoginSuccessMessage(maDN));
                    
                    if (App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                       && desktop.MainWindow is Avalonia.Controls.Window mainWindow)
                    {
                        mainWindow.Close();
                    }
                    return; 
                }
            }
            
            // Chỉ hiển thị thông báo lỗi nếu không tìm thấy user phù hợp
            KiemTraDangNhap = "Tên đăng nhập hoặc mật khẩu không đúng!";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Lỗi khi đăng nhập: {ex.Message}\n{ex.StackTrace}");
            KiemTraDangNhap = $"Lỗi hệ thống: {ex.Message}";
        }
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
