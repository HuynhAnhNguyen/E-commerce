using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Services;

namespace LuanVan.Areas.AdminManage.Pages.Color
{
    [Authorize(Roles = "Admin, Test")]

    public class EditModel : ColorPageModel
    {
        public EditModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string? MaMau { get; set; }

            [Required(ErrorMessage = "Tên màu là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên màu phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenMauSac { get; set; }

        }

        public MauSac mauSac { get; set; }
        public async Task<IActionResult> OnGetAsync(string colorid)
        {
            if (colorid == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayMauSac") + " " + colorid, 3);
                return RedirectToPage("./Index");
            }

            mauSac = await _context.MauSacs.Where(x => x.MaMau == colorid).FirstOrDefaultAsync();

            if (mauSac == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayMauSac") + " " + colorid, 3);
                return RedirectToPage("./Index");
            }
            else
            {
                Input = new InputModel()
                {
                    MaMau= mauSac.MaMau,
                    TenMauSac = mauSac.TenMau
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string colorid)
        {
            if (colorid == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayMauSac") + " " + colorid, 3);
                return RedirectToPage("./Index");
            }

            mauSac = await _context.MauSacs.Where(x => x.MaMau == colorid).FirstOrDefaultAsync();

            if (mauSac == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayMauSac") + " " + colorid, 3);
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var oldColor = mauSac.TenMau;

            _context.Update(mauSac);
            mauSac.TenMau = Input.TenMauSac;
            await _context.SaveChangesAsync();

            if (oldColor.Equals(Input.TenMauSac))
            {
                _notyf.Information(_localization.Getkey("ColorNotChange"), 3);
                //StatusMessage = _localization.Getkey("ColorNotChange");
            }
            else
            {
                //StatusMessage = _localization.Getkey("UpdateColorName") + " " + oldColor + " "+ _localization.Getkey("Thanh") +" " + Input.TenMauSac + " " + _localization.Getkey("ThanhCongLuc") + " "+ DateTimeVN();
                _notyf.Success(_localization.Getkey("UpdateColorName") + " " + oldColor + " " + _localization.Getkey("Thanh") + " " + Input.TenMauSac + " "+_localization.Getkey("Thanhcong"), 3);
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
