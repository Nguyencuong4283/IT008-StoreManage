using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Import;

namespace Store.Views;

public partial class CreateImportWindowView : Window
{
    public CreateImportWindowView()
    {
        InitializeComponent();
        var viewModel = new CreateImportWindowViewModel();
        viewModel.ParentWindow = this;
        DataContext = viewModel;
    }
}