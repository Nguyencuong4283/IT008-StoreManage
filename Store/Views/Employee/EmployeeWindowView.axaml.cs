using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using Store.ViewModels.Employee;

namespace Store.Views.Employee;

public partial class EmployeeWindowView : Window
{
    public EmployeeWindowView()
    {
        InitializeComponent();
        DataContext = new EmployeeWindowViewModel();
    }
    
    private Window? GetParentWindow(Control control)
    {
        return this.VisualRoot as Window;
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

    private void Header_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            this.WindowState = this.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        }
        else
        {
            // Nếu nhấp một lần thì cho phép kéo cửa sổ
            this.BeginMoveDrag(e);
        }
    }

    private void CloseBtn(object? sender, RoutedEventArgs e)
    {
        GetParentWindow(this)?.Close();
    }
    
    private void MinimizeBtn(object? sender, RoutedEventArgs e)
    {
        GetParentWindow(this)!.WindowState = WindowState.Minimized;
    }
    
    private void MaximizeBtn(object? sender, RoutedEventArgs e)
    {
        var window = GetParentWindow(this);
        if (window != null)
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}