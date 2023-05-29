namespace LuanVan.Areas.Admin.Models
{
    public class DiscountModel
    {
        public string? MaCTKM { get; set; }

        public string? TenCTKM { get; set; }
        public double? GiaTriKM { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public int SoLuongConLai { get; set; }
    }
}
