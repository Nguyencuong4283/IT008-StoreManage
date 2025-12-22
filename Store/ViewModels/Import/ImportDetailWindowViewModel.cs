using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Store.Models;
using Store.Services;

namespace Store.ViewModels.Import
{
    public partial class ImportDetailWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private string maNhapKho = string.Empty;
        [ObservableProperty] private string nhaCungCap = string.Empty;
        [ObservableProperty] private string thoiGianNhap = string.Empty;
        [ObservableProperty] private string ghiChu = string.Empty;
        [ObservableProperty] private decimal tongTien;
        [ObservableProperty] private ObservableCollection<ChiTiet_NhapKho> chiTietNhapKho = new();

        public ImportDetailWindowViewModel()
        {
            // Constructor mặc định
        }

        public ImportDetailWindowViewModel(Store.Models.Import import)
        {
            if (import != null)
            {
                MaNhapKho = import.MaNK;
                NhaCungCap = import.NhaCungCap;
                ThoiGianNhap = import.NgayNhap.ToString("dd/MM/yyyy");
                GhiChu = import.GhiChu ?? string.Empty;
                TongTien = import.TongTien;

                // Lấy chi tiết nhập kho
                var chiTietList = ImportService.GetImportDetail(import.MaNK);
                ChiTietNhapKho.Clear();
                foreach (var ct in chiTietList)
                {
                    ChiTietNhapKho.Add(ct);
                }
            }
        }
    }
}