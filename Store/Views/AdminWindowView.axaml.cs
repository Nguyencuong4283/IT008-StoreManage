using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels;

namespace Store.Views;

public partial class AdminWindowView : Window
{
    public AdminWindowView()
    {
        InitializeComponent();
        DataContext = new AdminWindowViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void LogoutButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close(); // đóng AdminWindow
    }
}
