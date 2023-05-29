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

namespace LuanVan.Areas.AdminManage.Pages.ProductType
{
    [Authorize(Roles = "Admin, Test")]


    public class CreateModel : ProductTypePageModel
    {
        public CreateModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductTypePageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Tên loại sản phẩm là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên loại sản phẩm phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenLoaiSP { get; set; }

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

            var existLoaiSP = await _context.LoaiSanPhams.Where(x => x.TenLoaiSp == Input.TenLoaiSP).FirstOrDefaultAsync();

            if (existLoaiSP == null)
            {
                var new_maLoaiSP = "" + DateTimeVN().ToString("ddMMyyyyHhmmss") + 1;

                LoaiSanPham loaiSanPham = new LoaiSanPham();
                loaiSanPham.MaLoaiSp = new_maLoaiSP;
                loaiSanPham.TenLoaiSp = Input.TenLoaiSP;
                _context.LoaiSanPhams.Add(loaiSanPham);
                await _context.SaveChangesAsync();


                _notyf.Success(_localization.Getkey("ThemLSP") + " " + Input.TenLoaiSP + " " + _localization.Getkey("Thanhcong"), 3);
                //StatusMessage = _localization.Getkey("ThemLSP") + " " + Input.TenLoaiSP + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

                return RedirectToPage("./Index");

            }
            else
            {
                //StatusMessage = _localization.Getkey("LSP") + " " + Input.TenLoaiSP + " " + _localization.Getkey("DaTonTai");
                _notyf.Error(_localization.Getkey("LSP") + " " + Input.TenLoaiSP + " " + _localization.Getkey("DaTonTai"), 5);

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
