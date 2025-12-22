using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Store.Views.Components;

public partial class WindowControls : UserControl
{
    public WindowControls()
    {
        InitializeComponent();
        CloseBtn.Click += (s, e) => (VisualRoot as Window)?.Close();
        
        MinBtn.Click += (s, e) => 
        {
            var window = VisualRoot as Window;
            if (window != null) window.WindowState = WindowState.Minimized;
        };
        
        MaxBtn.Click += (s, e) => 
        {
            var window = VisualRoot as Window;
            if (window == null) return;
            window.WindowState = window.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        };
    }
}