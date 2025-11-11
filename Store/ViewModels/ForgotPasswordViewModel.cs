using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Models;
using Store.Services;
using Store.Views;
using System.Collections.ObjectModel;

namespace Store.ViewModels
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        [ObservableProperty] private string email;
        [ObservableProperty] private string messageError;


        [RelayCommand]
        private void RegisterButton()
        {
            MainWindow createAcountWindowView = new MainWindow();
            createAcountWindowView.Show();
        }
        [RelayCommand]
        private void ConfirmButton()
        {
           
        }

    }
}
