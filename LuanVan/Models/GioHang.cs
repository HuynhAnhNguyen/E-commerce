using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class GioHang
{
    public string MaGioHang { get; set; } = null!;

    public int SoLuongDat { get; set; }

    public int TrangThai { get; set; }

    public string? MaSanPham { get; set; }

    public string? KhachHangId { get; set; }
    public virtual KhachHang? KhachHang { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; }
    
    public virtual SanPham? SanPham { get; set; }
}
