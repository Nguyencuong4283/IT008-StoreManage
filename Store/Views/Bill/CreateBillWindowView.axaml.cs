using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Bill;
using System.ComponentModel;
using Avalonia.Controls.Notifications;
using Store.ViewModels.Auth;

namespace Store.Views.Bill;

public partial class CreateBillWindowView : Window
{
    public CreateBillWindowView()
    {
        InitializeComponent();
        if (DataContext == null)
        {
            DataContext = new CreateBillWindowViewModel();
        }

        this.Opened += OnWindowOpened;
        this.Closing += OnWindowClosing;

        if (DataContext is CreateBillWindowViewModel vm)
        {
            vm.ParentWindow = this;
        }
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void OnWindowOpened(object? sender, EventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            var NotificationManager = new WindowNotificationManager(topLevel)
            {
                Position = NotificationPosition.TopCenter,
                MaxItems = 1
            };

            if (DataContext is CreateBillWindowViewModel vm)
            {
                vm.NotificationManager = NotificationManager;
            }
        }
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        if (DataContext is CreateBillWindowViewModel vm)
        {
            vm.OnWindowClosing();
            vm.Cleanup();
        }
    }
}