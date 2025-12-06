using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels;
using Store.Views;

namespace Store.Views;

public partial class EditAccountWindowView : Window
{
    public EditAccountWindowView()
    {
        InitializeComponent();

    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}