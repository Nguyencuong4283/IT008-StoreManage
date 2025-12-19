using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Store.ViewModels;

namespace Store.Views;

public partial class AdminWindowView : Window
{
    private WindowState _previousState = WindowState.Normal;
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
        var mainWindow = new Auth.MainWindow();
        mainWindow.Show();
        this.Close(); // đóng AdminWindow
    }

    private void CloseBtn(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void MinimizeBtn(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void FullscreenBtn(object? sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.FullScreen)
        {
            this.WindowState = _previousState;
        }
        else
        {
            _previousState = this.WindowState;
            this.WindowState = WindowState.FullScreen;
        }
    }
}
