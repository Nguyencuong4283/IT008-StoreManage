using Store.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Store.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;
    
    private readonly HomePageViewModel _homePage = new ();
    private readonly BillPageViewModel _billPage = new ();
    private readonly ProductPageViewModel _productPage = new();
    private readonly IncomePageViewModel _incomePage = new();
    private readonly SettingPageViewModel _settingPage = new();

    public MainWindowViewModel() => CurrentPage = _homePage;
    

    [RelayCommand]
    private void GoHome() => CurrentPage = _homePage;
    
    [RelayCommand]
    private void GoBill() => CurrentPage = _billPage;
    
    [RelayCommand]
    private void GoProduct()
    {
        CurrentPage = _productPage;
    }
    
    [RelayCommand]
    private void GoIncome()
    {
        CurrentPage = _incomePage;
    }
    
    [RelayCommand]
    private void GoSetting() => CurrentPage = _settingPage;
}