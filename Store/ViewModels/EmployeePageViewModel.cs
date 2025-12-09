
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Store.ViewModels;

public partial class EmployeePageViewModel : ViewModelBase
{
    [ObservableProperty] private string hoTen;
    [ObservableProperty] private string sDT;
    [ObservableProperty] private string email;
    [ObservableProperty] private ObservableCollection<User> nhanViens = new();

    public EmployeePageViewModel()
    {
        LoadNhanViens();
    }
    private  void LoadNhanViens()
    {
       var list = UserService.GetAllEmployee();
        nhanViens.Clear();
        foreach (var nv in list)
        {
            nhanViens.Add(nv);
        }
    }
    [RelayCommand]
    private async Task ChiTietButton(User user)
    {
        if (user == null) return;
        
        var editWindow = new Views.EditAccountWindowView
        {
            DataContext = new EditAccountWindowViewModel(user)
        };
        
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var owner = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
            if (owner != null)
            {
                await editWindow.ShowDialog(owner);
                LoadNhanViens(); // Reload sau khi đóng dialog
            }
        }
    }
    [RelayCommand]
    private void ThemNhanVienButton()
    {
        var createWindow = new Views.Auth.CreateAcountWindowView
        {
            DataContext = new Auth.CreateAcountWindowViewModel()
        };
        
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var owner = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
            if (owner != null)
            {
                createWindow.ShowDialog(owner);
                LoadNhanViens(); // Reload sau khi đóng dialog
            }
        }
    }
    /*   private void LoadKhachHangs()
    {
        var list = KhachHangService.GetAllKhachHang();

        khachHangs.Clear();
        foreach (var kh in list)
        {
            khachHangs.Add(kh);
        }
    }*/
}