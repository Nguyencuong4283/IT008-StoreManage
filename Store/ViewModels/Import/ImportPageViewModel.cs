using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using Store.Views;

namespace Store.ViewModels.Import;

public partial class ImportPageViewModel : ViewModelBase, IRecipient<ImportChangeMessage>
{
    [ObservableProperty] private ObservableCollection<Models.Import> imports = new();
    [ObservableProperty] private string searchKeyword;
    [ObservableProperty] private string selectedFilter = "Tất cả";
    private List<Models.Import> allImports = new();

    public ObservableCollection<string> FilterList { get; } = new()
    {
        "Tất cả",
        "Mã nhập kho",
        "Nhà cung cấp",
        "Ngày nhập"
    };

    public ImportPageViewModel()
    {
        WeakReferenceMessenger.Default.Register<ImportChangeMessage>(this);
        LoadImports();
    }

    public void Receive(ImportChangeMessage message)
    {
        Debug.WriteLine($"[ImportPageViewModel] Nhận message: Import {message.Value}");
        Dispatcher.UIThread.Post(() => { LoadImports(); });
    }

    private void LoadImports()
    {
        var list = ImportService.GetAllImport();
        allImports = list;
        UpdateImportList(allImports);
    }
    
    private void UpdateImportList(List<Models.Import> imports)
    {
        Imports.Clear();
        foreach (var nk in imports)
        {
            Imports.Add(nk);
        }
    }

    //===== Tìm kiếm lịch sử nhập kho =====//
    partial void OnSearchKeywordChanged(string value)
    {
        SearchImports();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        SearchImports();
    }

    private void SearchImports()
    {
        var allImports = ImportService.GetAllImport();
        var filterList = allImports;

        if (!string.IsNullOrWhiteSpace(SearchKeyword))
        {
            switch (SelectedFilter)
            {
                //Tìm kiếm Mã nhập kho
                case "Mã nhập kho":
                    filterList = allImports.FindAll(nk =>
                        nk.MaNK.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
                    break;

                //Tìm kiếm Nhà cung cấp
                case "Nhà cung cấp":
                    filterList = allImports.FindAll(nk =>
                        nk.NhaCungCap.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
                    break;

                //Tìm kiếm Ngày nhập
                case "Ngày nhập":
                    filterList = allImports.FindAll(nk =>
                        nk.NgayNhap.ToString("dd/MM/yyyy").Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
                    break;

                //Tất cả
                default:
                    filterList = allImports.FindAll(nk =>
                        nk.MaNK.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                        nk.NhaCungCap.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                        nk.NgayNhap.ToString("dd/MM/yyyy").Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)
                    );
                    break;
            }

            // Cập nhật danh sách hiển thị
            UpdateImportList(filterList);
        }
    }
    
    [RelayCommand]

    public void CreateImport()
    {
        var win = new CreateImportWindowView
        {
            DataContext = new CreateImportWindowViewModel() 
        };
        win.Show();
    }


}