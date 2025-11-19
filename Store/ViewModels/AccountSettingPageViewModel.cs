using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Models;
using Store.Services;
using Store.Messages;
using Store.Views;
using System;
using System.Collections.ObjectModel;
using static Store.ViewModels.MainWindowViewModel;

namespace Store.ViewModels;
public partial class AccountSettingPageViewModel : ViewModelBase,
    IRecipient<LoginSuccessMessage>
{
    [ObservableProperty] private string hoTen;
    [ObservableProperty] private string sDT;
    [ObservableProperty] private string email;

    private string _maDN;

    public AccountSettingPageViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(LoginSuccessMessage message)
    {
        _maDN = message.MaDN;
        LoadUserInfo();
    }

    private void LoadUserInfo()
    {
        var currentUser = UserService.GetOneUser(_maDN);
        if (currentUser != null)
        {
            HoTen = currentUser.HoTen;
            SDT = currentUser.SDT;
            Email = currentUser.Email;
        }
    }
}
