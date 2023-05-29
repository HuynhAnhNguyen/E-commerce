
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Promotion
{
    [Authorize(Roles = "Admin, Test")]

    public class CreateModel : PromotionPageModel
    {
        public CreateModel(ApplicationDbContext context, INotyfService notyf, ILogger<PromotionPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc!")]
            [StringLength(20, MinimumLength = 6, ErrorMessage = "Mã khuyến mãi phải có độ dài từ 6 đến 20 ký tự!")]
            public string? MaKM { get; set; }

            [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên khuyến mãi phải có độ dài từ 2 đến 255 ký tự!")]
            public string? TenKM { get; set; }

            [Required(ErrorMessage = "Giá trị khuyến mãi là bắt buộc!")]
            public float GiaTriKM { get; set; }

            [Required(ErrorMessage = "Ngày bắt đầu chương trình khuyến mãi là bắt buộc!")]
            public DateTime? NgayBatDau { get; set; }

            [Required(ErrorMessage = "Ngày kết thúc chương trình khuyến mãi là bắt buộc!")]
            public DateTime? NgayKetThuc { get; set; }

            [Required(ErrorMessage = "Số lượng khuyến mãi là bắt buộc!")]
            public int SoLuongKM { get; set; }
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existCTKM = await _context.KhuyenMais.Where(x => x.MaKm == Input.MaKM).FirstOrDefaultAsync();

            if(Input.NgayKetThuc< Input.NgayBatDau)
            {
                _notyf.Error("Ngày bắt đầu không thể trễ hơn ngày kết thúc", 3);

                return Page();
            }
            else if (existCTKM == null)
            {
                KhuyenMai khuyenMai = new KhuyenMai();
                khuyenMai.MaKm = Input.MaKM.ToUpper();
                khuyenMai.TenKhuyenMai = Input.TenKM;
                khuyenMai.GiaTriKm = (float) Input.GiaTriKM;
                khuyenMai.NgayBatDau= (DateTime)Input.NgayBatDau;
                khuyenMai.NgayKetThuc = (DateTime)Input.NgayKetThuc;
                khuyenMai.SoLuongConLai = Input.SoLuongKM;
                _context.KhuyenMais.Add(khuyenMai);
                await _context.SaveChangesAsync();


                _notyf.Success(_localization.Getkey("ThemKM") + " " + _localization.Getkey("Thanhcong"), 3);
                //StatusMessage = _localization.Getkey("ThemKM") + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

                return RedirectToPage("./Index");

            }
            else
            {
                //StatusMessage = _localization.Getkey("MaKMCreate") + " " + Input.MaKM + " " + _localization.Getkey("DaTonTai");
                _notyf.Error(_localization.Getkey("MaKMCreate") + " " + Input.MaKM + " " + _localization.Getkey("DaTonTai"), 5);

                return Page();
            }

        }

        public DateTime DateTimeVN()
        {
            DateTime utcTime = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Lấy thông tin về múi giờ của Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamZone); // Chuyển đổi giá trị DateTime từ múi giờ UTC sang múi giờ của Việt Nam

            return vietnamTime;
        }
    }
}
