
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.ViewModels.Customer;
using Store.ViewModels.Home;
using Store.ViewModels.Product;
using Store.ViewModels.Report;
using Store.ViewModels.Setting;
using Store.ViewModels.Import;

namespace Store.ViewModels.Employee;

public partial class EmployeeWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;
    private readonly HomePageViewModel _homePage = new();
    private readonly Bill.OrderPageViewModel _orderPage = new();
    private readonly ProductPageViewModel _productPage = new();
    private readonly SettingPageViewModel _settingPage = new();
    private readonly AccountSettingPageViewModel _accountSettingPage = new();
    private readonly CustomerPageViewModel _customerPage = new();
    private readonly ImportPageViewModel _importPageView = new();
    private readonly AnalysePageViewModel _analysePageView = new();

    public EmployeeWindowViewModel() => CurrentPage = _homePage;


    [RelayCommand]
    private void GoHome() => CurrentPage = _homePage;

    [RelayCommand]
    private void GoBill() => CurrentPage = _orderPage;

    [RelayCommand]
    private void GoProduct() => CurrentPage = _productPage;
    
    [RelayCommand]
    private void GoSetting() => CurrentPage = _settingPage;

    [RelayCommand]
    private void GoAccountSetting() => CurrentPage = _accountSettingPage;

    [RelayCommand]
    private void GoCustomer() => CurrentPage = _customerPage;
    
    [RelayCommand]
    private void GoHistory() => CurrentPage = _importPageView;

    [RelayCommand]
    private void GoAnalyse() => CurrentPage = _analysePageView;
}