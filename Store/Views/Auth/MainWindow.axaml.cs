using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Auth;

namespace Store.Views.Auth;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        this.AttachedToVisualTree += (s, e) =>
        {
            // Tìm control theo tên và focus
            var textBox = this.FindControl<TextBox>("UsernameTextBox");
            textBox?.Focus();
        };
    }
   
}