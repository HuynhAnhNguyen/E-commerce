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

namespace LuanVan.Areas.AdminManage.Pages.Payment
{
    [Authorize(Roles = "Admin, Test")]

    public class CreateModel : PaymentPageModel
    {
        public CreateModel(ApplicationDbContext context, INotyfService notyf, ILogger<PaymentPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Mã phương thức thanh toán là bắt buộc!")]
            [StringLength(10, MinimumLength = 3, ErrorMessage = "Mã phương thức thanh toán phải có độ dài từ 3 đến 10 ký tự!")]
            public string? MaPTTT { get; set; }

            [Required(ErrorMessage = "Tên phương thức thanh toán là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên phương thức thanh toán phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenPTTT { get; set; }

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

            var existPay = await _context.ThanhToans.Where(x => x.MaPttt == Input.MaPTTT).FirstOrDefaultAsync();

            if (existPay == null)
            {
                ThanhToan thanhToan = new ThanhToan();
                thanhToan.MaPttt = Input.MaPTTT.ToLower();
                thanhToan.TenPttt = Input.TenPTTT; ;
                _context.ThanhToans.Add(thanhToan);
                await _context.SaveChangesAsync();

                _notyf.Success(_localization.Getkey("ThemPTTT") + " " + Input.TenPTTT + " " + _localization.Getkey("Thanhcong"), 3);
                //StatusMessage = _localization.Getkey("ThemPTTT") + " " + Input.TenPTTT + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

                return RedirectToPage("./Index");

            }
            else
            {
                //StatusMessage = _localization.Getkey("PTTT") + " " + Input.MaPTTT + " " + _localization.Getkey("DaTonTai");
                _notyf.Error(_localization.Getkey("PTTT") + " " + Input.MaPTTT + " " + _localization.Getkey("DaTonTai"), 5);

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
