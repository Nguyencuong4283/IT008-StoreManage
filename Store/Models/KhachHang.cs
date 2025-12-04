using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class KhachHang
    {
        public string    MaKH { get; set; }
        public string TenKH { get; set; }
        public string SDT { get; set; }
        public string DiaChi { get; set; }
        public string GioiTinh { get; set; }
        public string Hang { get; set; }
        public decimal TongMua { get; set; }
        public string GhiChu { get; set; }
        
        public string MaVaTen => $"{MaKH} | {TenKH}";
        public KhachHang() {}
    }
}
