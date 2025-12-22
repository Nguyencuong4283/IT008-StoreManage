using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Auth;

namespace Store.Views.Auth;

public partial class ForgotPasswordView : Window
{
    public ForgotPasswordView()
    {
        InitializeComponent();
        DataContext = new ForgotPasswordViewModel();
    }
}