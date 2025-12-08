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
    }
   
}