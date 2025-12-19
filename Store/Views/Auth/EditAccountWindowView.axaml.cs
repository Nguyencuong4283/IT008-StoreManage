using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Auth;
namespace Store.Views.Auth;

public partial class EditAccountWindowView : Window
{
    public EditAccountWindowView()
    {
        InitializeComponent();
        if (DataContext is EditAccountWindowViewModel vm)
        {
            vm.ParentWindow = this;
        }

    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}