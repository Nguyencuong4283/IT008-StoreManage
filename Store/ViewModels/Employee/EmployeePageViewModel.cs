using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Store.Helpers;
using Store.Models;
using Store.Services;
using Store.Views.Auth;
using Store.ViewModels.Auth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;

namespace Store.ViewModels.Employee;

public partial class EmployeePageViewModel : ViewModelBase, IRecipient<AccountChangeMessage>
{
    [ObservableProperty] private string hoTen;
    [ObservableProperty] private string sDT;
    [ObservableProperty] private string email;
    [ObservableProperty] private string searchKeyword;
    [ObservableProperty] private string _selectedFilter = "Tất cả";
    [ObservableProperty] private ObservableCollection<User> nhanViens = new();

    private List<User> _allNhanViens = new();

    public WindowNotificationManager? NotificationManager { get; set; }

    //Nhận thông báo khi có thay đổi nhân viên
    public void Receive(AccountChangeMessage message)
    {
        if (message.Value == "Insert")
        {
            Dispatcher.UIThread.Post(() => { LoadEmployee(); });
        }

        else if (message.Value == "Delete")
        {
            NotificationManager?.Show("Xóa nhân viên thành công!", NotificationType.Success);
            Dispatcher.UIThread.Post(() => { LoadEmployee(); });
        }

        else if (message.Value == "Update")
        {
            NotificationManager?.Show("Cập nhật nhân viên thành công!", NotificationType.Success);
            Dispatcher.UIThread.Post(() => { LoadEmployee(); });
        }
    }

    public ObservableCollection<string> DanhSachBoLoc { get; } = new()
    {
        "Tất cả",
        "Họ tên",
        "Số điện thoại",
        "Email"
    };

    public EmployeePageViewModel()
    {
        WeakReferenceMessenger.Default.Register<AccountChangeMessage>(this);
        LoadEmployee();
    }

    private void LoadEmployee()
    {
        var list = UserService.GetAllEmployee();
        _allNhanViens = list;
        UpdateEmployeeList(_allNhanViens);
    }

    private void UpdateEmployeeList(List<User> employee)
    {
        nhanViens.Clear();
        foreach (var nv in employee)
        {
            nhanViens.Add(nv);
        }
    }

    [RelayCommand]
    private async Task DetailButton(User user)
    {
        if (user == null) return;

        var editWindow = new Views.Auth.EditAccountWindowView
        {
            DataContext = new EditAccountWindowViewModel(user)
        };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var owner = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
            if (owner != null)
            {
                await editWindow.ShowDialog(owner);
                LoadEmployee(); // Reload sau khi đóng dialog
            }
        }
    }

    [RelayCommand]
    private void InsertEmployeeButton()
    {
        WindowManager.ShowCreateAccountWindow();
    }

    //====== Tìm kiếm nhân viên ======//
    partial void OnSearchKeywordChanged(string value)
    {
        SearchEmployee();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        SearchEmployee();
    }

    private void SearchEmployee()
    {
        var filterList = _allNhanViens;

        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            switch (SelectedFilter)
            {
                // Tìm theo họ tên
                case "Họ tên":
                    filterList = _allNhanViens.FindAll(nv =>
                        nv.HoTen.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm theo số điện thoại
                case "Số điện thoại":
                    filterList = _allNhanViens.FindAll(nv =>
                        nv.SDT.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm theo email
                case "Email":
                    filterList = _allNhanViens.FindAll(nv =>
                        nv.Email.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;

                // Tìm tất cả
                default:
                    filterList = _allNhanViens.FindAll(nv =>
                        (nv.HoTen.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (nv.SDT.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (nv.Email.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    );
                    break;
            }

            // Cập nhật danh sách nhân viên hiển thị
            UpdateEmployeeList(filterList);
        }
    }
}