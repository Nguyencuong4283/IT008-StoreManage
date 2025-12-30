using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.LogicalTree;
using Store.ViewModels.Manager;
using System.Threading.Tasks;
using System.Linq;

namespace Store.Views.Manager;

public partial class AdminWindowView : Window
{
    public AdminWindowView()
    {
        InitializeComponent();
        DataContext = new AdminWindowViewModel();
        
        // Set focus to first button when window is loaded
        this.Loaded += OnWindowLoaded;
    }
    
    private async void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        // Delay để đảm bảo UI đã render xong
        await Task.Delay(100);
        
        // Tìm button đầu tiên có name HomeButton
        var firstButton = this.FindControl<Button>("HomeButton");
        if (firstButton != null)
        {
            firstButton.Focus();
        }
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
        this.Close(); // đóng AdminWindow
    }

    private void Header_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            // nhấn 2 lần để phóng to/thu nhỏ cửa sổ
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
