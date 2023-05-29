using System.ComponentModel.DataAnnotations;

namespace LuanVan.Areas.Store.Models
{
    public class HoaDonModel
    {
        [Required(ErrorMessage = "Họ lót không được trống!")]
        public string Holot { get; set; }
        [Required(ErrorMessage = "Tên không được trống!")]
        public string Ten { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [MaxLength(11)]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        public string GioHangs { get; set; }
        public string ThanhToan { get; set; }
        public string MaHoaDon { get; set; }

        public HoaDonModel()
        {
            this.GioHangs = "";
            this.MaHoaDon = "";
        }
    }
}
