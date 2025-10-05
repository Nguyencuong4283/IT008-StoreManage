using Store.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Store.ViewModels;

public partial class AdminWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;



    private readonly HomePageViewModel _homePage = new();
    private readonly BillPageViewModel _billPage = new();
    private readonly ProductPageViewModel _productPage = new();
    private readonly IncomePageViewModel _incomePage = new();
    private readonly SettingPageViewModel _settingPage = new();
    private readonly AccountSettingPageViewModel _accountSettingPage = new();
    private readonly CustomerPageViewModel _customerPage = new();
    private readonly EmployeePageViewModel _employeePage = new();
    private readonly HistoryPageViewModel _historyPageView = new();
    private readonly AnalysePageViewModel _analysePageView = new();

    public AdminWindowViewModel() => CurrentPage = _homePage;


    [RelayCommand]
    private void GoHome() => CurrentPage = _homePage;

    [RelayCommand]
    private void GoBill() => CurrentPage = _billPage;

    [RelayCommand]
    private void GoProduct() => CurrentPage = _productPage;

    [RelayCommand]
    private void GoIncome() => CurrentPage = _incomePage;

    [RelayCommand]
    private void GoSetting() => CurrentPage = _settingPage;

    [RelayCommand]
    private void GoAccountSetting() => CurrentPage = _accountSettingPage;

    [RelayCommand]
    private void GoCustomer() => CurrentPage = _customerPage;

    [RelayCommand]
    private void GoEmployee() => CurrentPage = _employeePage;

    [RelayCommand]
    private void GoHistory() => CurrentPage = _historyPageView;

    [RelayCommand]
    private void GoAnalyse() => CurrentPage = _analysePageView;
}