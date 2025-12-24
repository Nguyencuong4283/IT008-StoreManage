using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Setting;

namespace Store.Views.Setting;

public partial class AccountSettingPageView : UserControl
{
    public AccountSettingPageView()
    {
        InitializeComponent();
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
                MaxItems = 1,
            };
            
            if (DataContext is AccountSettingPageViewModel vm)
            {
                vm.NotificationManager = NotificationManager;
            }
        }
    }
}