using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Bill;
using System.ComponentModel;

namespace Store.Views.Bill;

public partial class CreateBillWindowView : Window
{
    public CreateBillWindowView()
    {
        InitializeComponent();
        DataContext = new CreateBillWindowViewModel();

        if (DataContext is CreateBillWindowViewModel vm)
        {
            vm.ParentWindow = this;
        }
        
    }
    
}