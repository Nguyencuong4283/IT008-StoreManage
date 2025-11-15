using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class HoaDon
    {
        public string   MaHD { get; set; }
        public DateTime NgayLapHD { get; set; }
        public decimal  TongTienHD { get; set; }
        public decimal  GiamGiaHD { get; set; }
        public string     MaKH { get; set; }
        public string      MaUser { get; set; }
        public string TenUser { get; set; }
        public int      SoHD { get; set; }
        public string   TrangThaiHD { get; set; }

        public KhachHang? KhachHang { get; set; }
        public string TenKH => KhachHang?.TenKH ?? "";

       
        public HoaDon() { }
    }
}
