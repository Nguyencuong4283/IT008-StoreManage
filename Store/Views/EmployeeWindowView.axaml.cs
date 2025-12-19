using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using Store.ViewModels;

namespace Store.Views;

public partial class EmployeeWindowView : Window
{
    private WindowState _previousState = WindowState.Normal;
    public EmployeeWindowView()
    {
        InitializeComponent();
        DataContext = new EmployeeWindowViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void LogoutButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainWindow = new Auth.MainWindow();
        mainWindow.Show();
        this.Close(); // đóng Window
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