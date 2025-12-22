using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Store.ViewModels.Import;

namespace Store.Views.Import;

public partial class ImportPageView : UserControl
{
    public ImportPageView()
    {
        InitializeComponent();
        DataContext = new ImportPageViewModel();
    }
}