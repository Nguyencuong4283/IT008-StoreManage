
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
       var list = UserService.GetAllUser();
        nhanViens.Clear();
        foreach (var nv in list)
        {
            nhanViens.Add(nv);
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