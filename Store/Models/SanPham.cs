using Avalonia.Media.Imaging;
using System;

namespace Store.Models
{
    public class SanPham
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public decimal GiaSP { get; set; }
        public int SoLuongSP { get; set; }
        public string? HinhAnhDuongDan { get; set; } // 🔹 lưu đường dẫn ảnh trong CSDL
        public Bitmap? HinhAnhSP { get; set; }        // 🔹 dùng để hiển thị trên UI
        public string KichThuocSP { get; set; } = string.Empty;
        public string LoaiSP { get; set; } = string.Empty;

        public int IsDelete { get; set; }  
        public string? MoTaSP { get; set; }

        public string TenSP_Size_TonKho => $"{TenSP} | {KichThuocSP} | {SoLuongSP}";
        public SanPham() { }
    }
}
