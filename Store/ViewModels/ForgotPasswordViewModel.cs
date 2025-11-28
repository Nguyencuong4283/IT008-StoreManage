using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Store.Views;
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace Store.ViewModels
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        [ObservableProperty] private string email;
        [ObservableProperty] private string messageError;

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
        private void ConfirmButton()
        {
           
        }

    }
}
