using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class SanPham
{
    public string MaSanPham { get; set; } = null!;

    public string TenSanPham { get; set; } = null!;

    public string TenDvt { get; set; } = null!;

    public string? MaNsx { get; set; }

    public string? MaLoaiSp { get; set; }

    public string? MaMau { get; set; }

    public string HinhAnh { get; set; } = null!;

    public long GiaBan { get; set; }

    public int TrangThai { get; set; }

    public string MoTa { get; set; } = null!;

    public int SoLuongDaBan { get; set; }

    public int SoLuongTon { get; set; }

    public virtual ICollection<GioHang> GioHangs { get; set; }

    public virtual LoaiSanPham? LoaiSp { get; set; }

    public virtual MauSac? MauSac { get; set; }

    public virtual NhaSanXuat? Nsx { get; set; }
}
