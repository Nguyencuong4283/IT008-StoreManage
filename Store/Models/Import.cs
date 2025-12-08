using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models;

public class Import
{
    public string MaNK { get; set; }
    public DateOnly NgayNhap { get; set; }
    public string NhaCungCap { get; set; }
    public int MaUser { get; set; }
    public User User { get; set; }
    public decimal TongTien { get; set; }
    public string GhiChu { get; set; }
    public int IsDelete { get; set; }
    public Import() {}
    
}