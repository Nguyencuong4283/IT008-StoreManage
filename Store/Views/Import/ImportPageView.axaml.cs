
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Import;

namespace Store.Views.Import;

public partial class ImportPageView : UserControl
{
    public ImportPageView()
    {
        InitializeComponent();
        DataContext = new ImportPageViewModel();
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
            
            if (DataContext is ImportPageViewModel vm)
            {
                vm.NotificationManager = NotificationManager;
            }
        }
    }
}