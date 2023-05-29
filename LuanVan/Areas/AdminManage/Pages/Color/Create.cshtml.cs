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
using Microsoft.AspNetCore.Authorization;
using System.Data;
using LuanVan.Services;

namespace LuanVan.Areas.AdminManage.Pages.Color
{
    [Authorize(Roles = "Admin, Test")]
    public class CreateModel : ColorPageModel
    {
        public CreateModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Tên màu là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên màu phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenMauSac { get; set; }

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

            var existColor = await _context.MauSacs.Where(x => x.TenMau == Input.TenMauSac).FirstOrDefaultAsync();

            if(existColor == null)
            {
                var new_MaColor = "" + DateTimeVN().ToString("ddMMyyyyHhmmss") + 1;

                MauSac mauSac = new MauSac();
                mauSac.MaMau = new_MaColor;
                mauSac.TenMau = Input.TenMauSac;
                _context.MauSacs.Add(mauSac);
                await _context.SaveChangesAsync();

                _notyf.Success(_localization.Getkey("ThemMau") + " " + Input.TenMauSac + " "+ _localization.Getkey("Thanhcong"), 3);
                //StatusMessage = _localization.Getkey("ThemMau") + " " + Input.TenMauSac + " "+ _localization.Getkey("ThanhCongLuc") + " "+ DateTimeVN();
                return RedirectToPage("./Index");

            }
            else
            {
                //StatusMessage = _localization.Getkey("MauSac")+ " " + Input.TenMauSac + " "+ _localization.Getkey("DaTonTai");
                _notyf.Error(_localization.Getkey("MauSac") + " " + Input.TenMauSac + " " + _localization.Getkey("DaTonTai"), 5);
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
