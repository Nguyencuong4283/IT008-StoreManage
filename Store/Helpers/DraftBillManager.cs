using Store.Models;
using System.Collections.ObjectModel;

namespace Store.Helpers
{
    /// <summary>
    /// Quản lý hóa đơn nháp (chưa thanh toán)
    /// </summary>
    public static class DraftBillManager
    {
        private static ObservableCollection<ChiTiet_HoaDon>? _draftItems;
        private static string? _draftMaHD;
        private static int? _draftSoHD;
        private static KhachHang? _draftKhachHang;
        private static User? _draftNhanVien;
        
        public static bool HasDraft => _draftItems != null && _draftItems.Count > 0;
        
        public static void SaveDraft(
            string maHD,
            int soHD,
            ObservableCollection<ChiTiet_HoaDon> items,
            KhachHang? khachHang,
            User? nhanVien)
        {
            _draftMaHD = maHD;
            _draftSoHD = soHD;
            _draftItems = new ObservableCollection<ChiTiet_HoaDon>(items);
            _draftKhachHang = khachHang;
            _draftNhanVien = nhanVien;
            
            System.Diagnostics.Debug.WriteLine($"[DraftBillManager] Đã lưu nháp: {maHD}, {items.Count} sản phẩm");
        }
        
        public static (string MaHD, int SoHD, ObservableCollection<ChiTiet_HoaDon> Items, KhachHang? KhachHang, User? NhanVien) LoadDraft()
        {
            var items = _draftItems ?? new ObservableCollection<ChiTiet_HoaDon>();
            var maHD = _draftMaHD ?? "";
            var soHD = _draftSoHD ?? 0;
            
            System.Diagnostics.Debug.WriteLine($"[DraftBillManager] Đã load nháp: {maHD}, {items.Count} sản phẩm");
            
            return (maHD, soHD, items, _draftKhachHang, _draftNhanVien);
        }
        
        public static void ClearDraft()
        {
            _draftMaHD = null;
            _draftSoHD = null;
            _draftItems = null;
            _draftKhachHang = null;
            _draftNhanVien = null;
            
            System.Diagnostics.Debug.WriteLine("[DraftBillManager] Đã xóa nháp");
        }
    }
}
