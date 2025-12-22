using Avalonia.Controls;
using Store.ViewModels.Import;

namespace Store.Views.Import
{
    public partial class ImportDetailWindowView : Window
    {
        public ImportDetailWindowView()
        {
            InitializeComponent();
            DataContext = new ImportDetailWindowViewModel();
        }
    }
}
