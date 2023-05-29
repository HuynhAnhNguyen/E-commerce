
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

    public class EditModel : PromotionPageModel
    {
        public EditModel(ApplicationDbContext context, INotyfService notyf, ILogger<PromotionPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
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

        public KhuyenMai khuyenMai { get; set; }
        public async Task<IActionResult> OnGetAsync(string promotionid)
        {
            if (promotionid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayKM") + " " + promotionid, 3);
                return RedirectToPage("./Index");
            }

            khuyenMai =await _context.KhuyenMais.Where(x => x.MaKm == promotionid).FirstOrDefaultAsync();

            if (khuyenMai == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayKM") + " " + promotionid, 3);
                return RedirectToPage("./Index");
            }
            else
            {
                Input = new InputModel()
                {
                    MaKM = khuyenMai.MaKm,
                    TenKM = khuyenMai.TenKhuyenMai,
                    GiaTriKM = (float)khuyenMai.GiaTriKm,
                    NgayBatDau = khuyenMai.NgayBatDau,
                    NgayKetThuc = khuyenMai.NgayKetThuc,
                    SoLuongKM = khuyenMai.SoLuongConLai
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPost(string promotionid)
        {
            if (promotionid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayKM") + " " + promotionid, 3);
                return RedirectToPage("./Index");
            }

            khuyenMai = await _context.KhuyenMais.Where(x => x.MaKm == promotionid).FirstOrDefaultAsync();

            if (khuyenMai == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayKM") + " " + promotionid, 3);
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.NgayKetThuc < Input.NgayBatDau)
            {
                _notyf.Error("Ngày bắt đầu không thể trễ hơn ngày kết thúc", 3);

                return Page();
            }

            _context.Update(khuyenMai);
            khuyenMai.TenKhuyenMai = Input.TenKM;
            khuyenMai.GiaTriKm = Input.GiaTriKM;
            khuyenMai.NgayBatDau= (DateTime)Input.NgayBatDau;
            khuyenMai.NgayKetThuc = (DateTime)Input.NgayKetThuc;
            khuyenMai.SoLuongConLai = Input.SoLuongKM;
            await _context.SaveChangesAsync();

            //StatusMessage = _localization.Getkey("UpdateKM") + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();
            _notyf.Success(_localization.Getkey("UpdateKM") + " " + _localization.Getkey("Thanhcong"), 3);

            return RedirectToPage("./Index");
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
