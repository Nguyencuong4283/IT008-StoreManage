using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels;
using System.ComponentModel;

namespace Store.Views;

public partial class CreateBillWindowView : Window
{
    public CreateBillWindowView()
    {
        InitializeComponent();
        DataContext = new CreateBillWindowViewModel();
        
        // Hook vào sự kiện đóng window
        Closing += OnWindowClosing;
    }
    
    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        // Gọi method lưu nháp trong ViewModel
        if (DataContext is CreateBillWindowViewModel viewModel)
        {
            viewModel.OnWindowClosing();
        }
    }
}