using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class ChiTiet_NhapKho
    {
        public int      MaNK { get; set; }
        public int      MaSP { get; set; }
        public int      SoLuong { get; set; }
        public decimal  DonGia { get; set; }
        public decimal  ThanhTien { get; set; }
        public NhapKho NhapKho { get; set; }
        public SanPham SanPham { get; set; }
        public ChiTiet_NhapKho() { }
    }
}
