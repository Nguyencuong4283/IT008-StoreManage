using Avalonia;
using Avalonia.Controls;
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
}