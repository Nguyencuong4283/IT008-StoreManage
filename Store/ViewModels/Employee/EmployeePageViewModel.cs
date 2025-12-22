
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Helpers;
using Store.Models;
using Store.Services;
using Store.Views.Auth;
using Store.ViewModels.Auth;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;

namespace Store.ViewModels.Employee;

public partial class EmployeePageViewModel : ViewModelBase, IRecipient<AccountChangeMessage>
{
    [ObservableProperty] private string hoTen;
    [ObservableProperty] private string sDT;
    [ObservableProperty] private string email;
    [ObservableProperty] private ObservableCollection<User> nhanViens = new();
    
    public WindowNotificationManager? NotificationManager { get; set; }
    
    //Nhận thông báo khi có thay đổi nhân viên
    public void Receive(AccountChangeMessage message)
    {
        if(message.Value == "Inserted" || message.Value == "Updated" || message.Value == "Deleted")
        {
            Dispatcher.UIThread.Post(() => { LoadEmployee(); });
        }
    }

    public EmployeePageViewModel()
    {
        WeakReferenceMessenger.Default.Register<AccountChangeMessage>(this);
        LoadEmployee();
    }
    private  void LoadEmployee()
    {
       var list = UserService.GetAllEmployee();
        nhanViens.Clear();
        foreach (var nv in list)
        {
            nhanViens.Add(nv);
        }
    }
    [RelayCommand]
    private async Task DetailButton(User user)
    {
        if (user == null) return;
        
        var editWindow = new Views.Auth.EditAccountWindowView
        {
            DataContext = new EditAccountWindowViewModel(user)
        };
        
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var owner = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
            if (owner != null)
            {
                await editWindow.ShowDialog(owner);
                LoadEmployee(); // Reload sau khi đóng dialog
            }
        }
    }
    [RelayCommand]
    private void InsertEmployeeButton()
    {
        WindowManager.ShowCreateAccountWindow();
    }
    
}