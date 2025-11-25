using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Store.Views;

public partial class SettingPageView : UserControl
{
    public SettingPageView()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var app = Application.Current;
        if (app is not null)
        {
            // Nếu đang là Dark thì chuyển sang Light, và ngược lại
            app.RequestedThemeVariant =
                app.RequestedThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
        }
    }
}