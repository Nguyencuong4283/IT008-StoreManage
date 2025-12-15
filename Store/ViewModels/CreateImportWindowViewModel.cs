using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Store.Messages;
using Store.Models;
using Store.Services;
using Store.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public partial class CreateImportWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string nhaCungCap = "";
    [ObservableProperty] private string? ghiChu;
    public DateOnly NgayNhap { get; } = DateOnly.FromDateTime(DateTime.Now);

    public ObservableCollection<ChiTiet_NhapKho> ChiTietList { get; } = new();

    [ObservableProperty] private SanPham? selectedSanPham;
    [ObservableProperty] private int soLuong;
    [ObservableProperty] private decimal donGia;

    public ObservableCollection<SanPham> SanPhamList { get; }

    public CreateImportWindowViewModel()
    {
        SanPhamList = new ObservableCollection<SanPham>(
            SanPhamService.GetAllSanPham()
        );
    }

    [RelayCommand]
    void AddSanPham()
    {
        if (SelectedSanPham == null || SoLuong <= 0)
            return;

        ChiTietList.Add(new ChiTiet_NhapKho
        {
            MaSP = SelectedSanPham.MaSP,
            SanPham = SelectedSanPham,
            SoLuong = SoLuong,
            DonGia = DonGia,
            ThanhTien = SoLuong * DonGia
        });

        SoLuong = 0;
        DonGia = 0;
    }

    [RelayCommand]
    void SaveImport()
    {
        if (ChiTietList.Count == 0)
            return;

        var nk = new Import
        {
            NgayNhap = NgayNhap,
            NhaCungCap = NhaCungCap,
            TongTien = ChiTietList.Sum(x => x.ThanhTien),
            GhiChu = GhiChu,
            MaUser = 1
        };

        ImportService.InsertNhapKho(nk, ChiTietList.ToList());
        WeakReferenceMessenger.Default.Send(new ImportChangeMessage("Created"));
    }
}
