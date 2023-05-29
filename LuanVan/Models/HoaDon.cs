using System;
using System.Collections.Generic;

namespace LuanVan.Models;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public DateTime NgayXuatHd { get; set; }

    public string? KhachHangId { get; set; }

    public double? TongGiaTri { get; set; }

    public string? MaKm { get; set; }

    public string? MaPttt { get; set; }

    public int TrangThaiThanhToan { get; set; }
    public int TrangThaiDonHang { get; set; }


    public virtual KhachHang? KhachHang { get; set; }
    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; }

    public virtual KhuyenMai? KhuyenMai { get; set; }

    public virtual ThanhToan? ThanhToan { get; set; }

}
