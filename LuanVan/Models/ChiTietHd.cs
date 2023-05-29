using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class ChiTietHd
{
    public string MaChiTietHd { get; set; } = null!;

    public string? MaHoaDon { get; set; }

    public string? MaGioHang { get; set; }

    public virtual GioHang? GioHang { get; set; }

    public virtual HoaDon? HoaDon { get; set; }
}
