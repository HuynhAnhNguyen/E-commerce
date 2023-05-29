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

namespace LuanVan.Areas.AdminManage.Pages.Producer
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
            public string? MaNSX { get; set; }

            [Required(ErrorMessage = "Tên nhà sản xuất là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên nhà sản xuất phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenNSX { get; set; }

        }

        public NhaSanXuat nhaSanXuat { get; set; }
        public async Task<IActionResult> OnGetAsync(string producerid)
        {
            if (producerid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayNSX") + " " + producerid, 3);
                return RedirectToPage("./Index");
            }

            nhaSanXuat = await _context.NhaSanXuats.Where(x => x.MaNsx == producerid).FirstOrDefaultAsync();

            if (nhaSanXuat == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayNSX") + " " + producerid, 3);
                return RedirectToPage("./Index");
            }
            else
            {
                Input = new InputModel()
                {
                    MaNSX = nhaSanXuat.MaNsx,
                    TenNSX = nhaSanXuat.TenNsx
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string producerid)
        {
            if (producerid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayNSX") + " " + producerid, 3);
                return RedirectToPage("./Index");
            }

            nhaSanXuat = await _context.NhaSanXuats.Where(x => x.MaNsx == producerid).FirstOrDefaultAsync();

            if (nhaSanXuat == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayNSX") + " " + producerid, 3);
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var oldNSX = nhaSanXuat.TenNsx;

            _context.Update(nhaSanXuat);
            nhaSanXuat.TenNsx = Input.TenNSX;
            await _context.SaveChangesAsync();

            if (oldNSX.Equals(Input.TenNSX))
            {
                _notyf.Information(_localization.Getkey("NSXNotChange") + "", 3);
                //StatusMessage = _localization.Getkey("NSXNotChange");
            }
            else
            {
                //StatusMessage = _localization.Getkey("UpdateNSXName") + " " + oldNSX + " " + _localization.Getkey("Thanh") + " " + Input.TenNSX + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();
                _notyf.Success(_localization.Getkey("UpdateNSXName") + " " + oldNSX + " " + _localization.Getkey("Thanh") + " " + Input.TenNSX + " " + _localization.Getkey("Thanhcong"), 3);
               
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
