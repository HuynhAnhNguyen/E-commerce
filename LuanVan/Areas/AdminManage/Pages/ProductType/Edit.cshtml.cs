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


    public class EditModel : ProductTypePageModel
    {
        public EditModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductTypePageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string? MaLoaiSP { get; set; }

            [Required(ErrorMessage = "Tên loại sản phẩm là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên loại sản phẩm phải có độ dài từ 2 đến 255 ký tự!")]

            public string? TenLoaiSP { get; set; }

        }

        public LoaiSanPham loaiSanPham { get; set; }
        public async Task<IActionResult> OnGetAsync(string producttypeid)
        {
            if (producttypeid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayLoaiSP") + " " + producttypeid, 3);
                return RedirectToPage("./Index");
            }

            loaiSanPham = await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == producttypeid).FirstOrDefaultAsync();

            if (loaiSanPham == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayLoaiSP") + " " + producttypeid, 3);
                return RedirectToPage("./Index");
            }
            else
            {
                Input = new InputModel()
                {
                    MaLoaiSP= loaiSanPham.MaLoaiSp,
                    TenLoaiSP = loaiSanPham.TenLoaiSp
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string producttypeid)
        {
            if (producttypeid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayLoaiSP") + " " + producttypeid, 3);
                return RedirectToPage("./Index");
            }

            loaiSanPham = await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == producttypeid).FirstOrDefaultAsync();

            if (loaiSanPham == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayLoaiSP") + " " + producttypeid, 3);
                return RedirectToPage("./Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var oldLSP = loaiSanPham.TenLoaiSp;

            _context.Update(loaiSanPham);
            loaiSanPham.TenLoaiSp = Input.TenLoaiSP;
            await _context.SaveChangesAsync();

            if (oldLSP.Equals(Input.TenLoaiSP))
            {
                _notyf.Information(_localization.Getkey("LoaiSPNotChange") + "", 3);
                //StatusMessage = _localization.Getkey("LoaiSPNotChange");


            }
            else
            {
                //StatusMessage = _localization.Getkey("UpdateLSPName") + " " + oldLSP + " " + _localization.Getkey("Thanh") + " " + Input.TenLoaiSP + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();
                _notyf.Success(_localization.Getkey("UpdateLSPName") + " " + oldLSP + " " + _localization.Getkey("Thanh") + " " + Input.TenLoaiSP + " " + _localization.Getkey("Thanhcong"), 3);

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
