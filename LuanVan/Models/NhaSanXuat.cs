using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class NhaSanXuat
{
    public string MaNsx { get; set; } = null!;

    public string TenNsx { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } 
}
