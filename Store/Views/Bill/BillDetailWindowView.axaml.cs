using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Bill;
using System.ComponentModel;

namespace Store.Views.Bill;

public partial class BillDetailWindowView : Window
{
    public BillDetailWindowView()
    {
        InitializeComponent();
        DataContext = new BillDetailWindowViewModel();
    }

 
}