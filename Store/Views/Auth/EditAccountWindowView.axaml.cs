using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Auth;
namespace Store.Views.Auth;

public partial class EditAccountWindowView : Window
{
    public EditAccountWindowView()
    {
        InitializeComponent();
        if (DataContext is EditAccountWindowViewModel vm)
        {
            vm.ParentWindow = this;
        }
        this.Opened += OnWindowOpened;

    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        //var topLevel = TopLevel.GetTopLevel(this);
        //if (topLevel != null)
        //{
        //   var NotificationManager = new WindowNotificationManager(topLevel)
        //    {
        //        Position = NotificationPosition.TopCenter,
        //        MaxItems = 1
        //    };
        //
        //    if (DataContext is CreateAcountWindowViewModel vm)
        //    {
        //        vm.NotificationManager = NotificationManager;
        //   }
        //}NotificationManager
    }
}