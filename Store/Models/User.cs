using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class User 
    {
        public string    MaNV { get; set; }
        public string    TenDangNhap { get; set; }
        public string    HoTen { get; set; }
        public string    MatKhau { get; set; }
        public string    Email { get; set; }
        public string    SDT { get; set; }
        public string    DiaChi { get; set; }
        public string    GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string    HinhAnh { get; set; }
        public string MaVT { get; set; }
        public string MaNV_HoTen => $"{MaNV} | {HoTen}";


     
    }
}
