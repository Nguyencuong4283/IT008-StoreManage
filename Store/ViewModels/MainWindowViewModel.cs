﻿using Store.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Store.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public object CurrentPage { get; set; }

    public MainWindowViewModel()
    {
        CurrentPage = null;
    }

    [RelayCommand]
    private void LogInButton()
    {
        var adminVM = new AdminWindowViewModel();
        var adminWindow = new AdminWindowView();
        adminWindow.DataContext = adminVM;
        adminWindow.Show();

        // Đóng MainWindow hiện tại
        if (App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
     && desktop.MainWindow is Avalonia.Controls.Window mainWindow)
        {
            mainWindow.Close();
        }

    }
}
