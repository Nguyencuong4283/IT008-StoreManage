using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class ChiTiet_HoaDon
    {
        public string   MaHD { get; set; }
        public string   MaSP { get; set; }
        public int      SoLuong { get; set; }
        public decimal  DonGia { get; set; }
        public int      KhuyenMai { get; set; }
        public decimal  ThanhTien { get; set; }
       
        public HoaDon? HoaDon { get; set; }
        public SanPham? SanPham { get; set; }
        
        // Thuộc tính để hiển thị
        public string TenSP => SanPham?.TenSP ?? "";
        public string KichThuocSP => SanPham?.KichThuocSP ?? "";
        public decimal GiamGia => DonGia * KhuyenMai / 100;
        public decimal DonGiaSauGiam => DonGia - GiamGia;
        
        public ChiTiet_HoaDon() { }
    }
}
