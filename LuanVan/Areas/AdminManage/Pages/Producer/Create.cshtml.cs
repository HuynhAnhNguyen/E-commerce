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

namespace LuanVan.Areas.AdminManage.Pages.Producer
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
            [Required(ErrorMessage = "Tên nhà sản xuất là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên nhà sản xuất phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenNSX { get; set; }

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

            var existNSX = await _context.NhaSanXuats.Where(x => x.TenNsx == Input.TenNSX).FirstOrDefaultAsync();

            if(existNSX == null)
            {
                var new_maNSX = "" + DateTimeVN().ToString("ddMMyyyyHhmmss") + 1;

                NhaSanXuat nhaSanXuat = new NhaSanXuat();
                nhaSanXuat.MaNsx = new_maNSX;
                nhaSanXuat.TenNsx = Input.TenNSX;
                _context.NhaSanXuats.Add(nhaSanXuat);
                await _context.SaveChangesAsync();

                _notyf.Success(_localization.Getkey("ThemNSX") + " " + Input.TenNSX + " " + _localization.Getkey("Thanhcong"), 3);
                //StatusMessage = _localization.Getkey("ThemNSX") + " " + Input.TenNSX + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();


                return RedirectToPage("./Index");

            }
            else
            {

                //StatusMessage = _localization.Getkey("NSX") + " " + Input.TenNSX + " " + _localization.Getkey("DaTonTai");
                _notyf.Error(_localization.Getkey("NSX") + " " + Input.TenNSX + " " + _localization.Getkey("DaTonTai"), 5);

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
