using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class MauSac
{
    public string MaMau { get; set; } = null!;

    public string TenMau { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; }
}
