using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Customer;

namespace Store.Views.Customer;

public partial class CustomerPageView : UserControl
{
    public CustomerPageView()
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
                MaxItems = 1
            };
            
            if (DataContext is CustomerPageViewModel vm)
            {
                vm.NotificationManager = NotificationManager;
            }
        }
    }
}