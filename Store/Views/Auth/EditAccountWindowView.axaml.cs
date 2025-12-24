using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
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
        
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            var NotificationManager = new WindowNotificationManager(topLevel)
            {
                Position = NotificationPosition.TopCenter,
                MaxItems = 1
            };
        
            if (DataContext is EditAccountWindowViewModel vm)
            {
                vm.NotificationManager = NotificationManager;
            }
        }
    }
}