namespace LuanVan.Areas.Admin.Models
{
    public class BillModel
    {
        public string? MaHD { get; set; }

        public DateTime? NgayXuatHD { get; set; }
        public double? TongGiaTri { get; set; }
        public string? HoVaTenKH { get; set; }
        public string? TenPTTT { get; set; }
        public string? TenCTKM { get; set; }

        public int? TrangThaiThanhToan { get;set; }
        public int? TrangThaiDonHang { get;set; }

    }
}
