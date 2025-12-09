using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels;
using Store.ViewModels.Bill;

namespace Store;

public partial class CreateCustomerWindowView : Window
{
    public CreateCustomerWindowView()
    {
        InitializeComponent();
        DataContext = new CreateCustomerWindowViewModel();
        if (DataContext is CreateCustomerWindowViewModel vm)
        {
            vm.ParentWindow = this;
        }
    }
}