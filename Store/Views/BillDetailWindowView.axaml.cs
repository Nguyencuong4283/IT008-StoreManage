using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels;
using System.ComponentModel;

namespace Store.Views;

public partial class BillDetailWindowView : Window
{
    public BillDetailWindowView()
    {
        InitializeComponent();
        DataContext = new BillDetailWindowViewModel();
    }

 
}