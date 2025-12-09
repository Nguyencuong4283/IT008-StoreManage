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
        // Khi nhấn Tab ở button "Thêm sản phẩm", quay lại ComboBox sản phẩm
        ThemSanPham.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Tab && !e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                e.Handled = true; // Ngăn hành vi Tab mặc định
                CboSanPham.Focus(); // Quay lại ComboBox sản phẩm
            }
        };
    }
    
}