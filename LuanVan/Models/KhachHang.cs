using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class KhachHang : IdentityUser
{

    public string? HoKhachHang { get; set; }
    public string? TenKhachHang { get; set; }

    public DateTime NgaySinh { get; set; }= DateTime.Now;

    public string? GioiTinh { get; set; }

    public string? DiaChi { get; set; }

    public int DisableAccount { get; set; }

    public virtual ICollection<GioHang>? GioHangs { get; set; }
    public virtual ICollection<HoaDon> HoaDons { get; set; }


}
