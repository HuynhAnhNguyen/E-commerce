using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class KhuyenMai
{
    public string MaKm { get; set; } = null!;

    public string TenKhuyenMai { get; set; } = null!;

    public double GiaTriKm { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public int SoLuongConLai { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } 
}
