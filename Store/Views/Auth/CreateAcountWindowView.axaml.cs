using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Store.Views.Auth;

public partial class CreateAcountWindowView : Window
{
    public CreateAcountWindowView()
    {
        InitializeComponent();
      
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}