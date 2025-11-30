using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Store.Views;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.ViewModels
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        [ObservableProperty] private string email;
        [ObservableProperty] private string messageError;
        [ObservableProperty] private string messageSuccess;
        [ObservableProperty] private bool isLoading;

        [RelayCommand]
        private void RegisterButton()
        {
            MainWindow mainWindow = new MainWindow();
            if (App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is Avalonia.Controls.Window currentWindow)
            {
                currentWindow.Close();
            }
            mainWindow.Show();
        }
        
        [RelayCommand]
        private async Task ConfirmButton()
        {
            MessageError = string.Empty;
            MessageSuccess = string.Empty;

            // Validate email
            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageError = "Vui lòng nhập địa chỉ email";
                return;
            }

            if (!IsValidEmail(Email))
            {
                MessageError = "Địa chỉ email không hợp lệ";
                return;
            }

            IsLoading = true;

            try
            {
                // Tìm user theo email
                var user = UserService.GetUserByEmail(Email.Trim());

                if (user == null)
                {
                    MessageError = "Email không tồn tại trong hệ thống";
                    IsLoading = false;
                    return;
                }

                // Tạo mật khẩu tạm thời (8 ký tự ngẫu nhiên)
                string tempPassword = GenerateRandomPassword(8);

                // Cập nhật mật khẩu mới vào database
                user.MatKhau = tempPassword;
                UserService.UpdateUser(user, updatePassword: true);

                // Gửi email với thông tin đăng nhập
                var emailService = new EmailService();
                await emailService.SendAccountInfo(Email.Trim(), user.TenDangNhap, tempPassword);

                MessageSuccess = "Thông tin đăng nhập đã được gửi đến email của bạn!";
                Email = string.Empty;
            }
            catch (Exception ex)
            {
                MessageError = $"Có lỗi xảy ra: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
