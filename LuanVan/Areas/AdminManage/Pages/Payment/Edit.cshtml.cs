using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
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


    public class EditModel : PaymentPageModel
    {
        public EditModel(ApplicationDbContext context, INotyfService notyf, ILogger<PaymentPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string? MaPTTT { get; set; }

            [Required(ErrorMessage = "Tên phương thức thanh toán là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên phương thức thanh toán phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenPTTT { get; set; }

        }

        public ThanhToan thanhToan { get; set; }
        public async Task<IActionResult> OnGetAsync(string paymentid)
        {
            if (paymentid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid, 3);
                return RedirectToPage("./Index");
            }

            thanhToan = await _context.ThanhToans.Where(x => x.MaPttt == paymentid).FirstOrDefaultAsync();

            if (thanhToan == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid, 3);
                return RedirectToPage("./Index");
            }
            else
            {
                Input = new InputModel()
                {
                    MaPTTT = thanhToan.MaPttt,
                    TenPTTT = thanhToan.TenPttt
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string paymentid)
        {
            if (paymentid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid, 3);
                return RedirectToPage("./Index");
            }

            thanhToan = await _context.ThanhToans.Where(x => x.MaPttt == paymentid).FirstOrDefaultAsync();

            if (thanhToan == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid, 3);
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var oldPay = thanhToan.TenPttt;

            _context.Update(thanhToan);
            thanhToan.TenPttt = Input.TenPTTT;
            await _context.SaveChangesAsync();

            if (oldPay.Equals(Input.TenPTTT))
            {
                _notyf.Information(_localization.Getkey("PTTTNotChange")+"", 3);
                //StatusMessage = _localization.Getkey("PTTTNotChange");
            }
            else
            {
                //StatusMessage = _localization.Getkey("UpdatePTTTName") + " " + oldPay + " " + _localization.Getkey("Thanh") + " " + Input.TenPTTT+" " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();
                _notyf.Success(_localization.Getkey("UpdatePTTTName") + " " + oldPay + " " + _localization.Getkey("Thanh") + " " + Input.TenPTTT+" " + _localization.Getkey("Thanhcong"), 3);
            }
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
