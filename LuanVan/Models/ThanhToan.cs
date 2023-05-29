using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class ThanhToan
{
    public string MaPttt { get; set; } = null!;

    public string TenPttt { get; set; } = null!;

    public virtual ICollection<HoaDon> HoaDons { get; set; }
}
