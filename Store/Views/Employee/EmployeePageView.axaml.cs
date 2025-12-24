using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Store.Views.Employee;

public partial class EmployeePageView : UserControl
{
    public EmployeePageView()
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
            var NotificationManager = new Avalonia.Controls.Notifications.WindowNotificationManager(topLevel)
            {
                Position = Avalonia.Controls.Notifications.NotificationPosition.TopCenter,
                MaxItems = 1
            };
            
            if (DataContext is Store.ViewModels.Employee.EmployeePageViewModel vm)
            {
                vm.NotificationManager = NotificationManager;
            }
        }
    }
}