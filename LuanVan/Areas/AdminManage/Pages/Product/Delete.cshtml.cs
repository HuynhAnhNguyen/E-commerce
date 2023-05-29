using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Product
{
    [Authorize(Roles = "Admin, Editor")]
    public class DeleteModel : ProductPageModel
    {
        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public SanPham sanPham { get; set; }

        public List<GioHang> gioHangs { get; set; }
        //public async Task<IActionResult> OnGetAsync(string productid)
        //{
        //    if (productid == null) return NotFound(_localization.Getkey("KhongTimThaySP") + " " + productid);

        //    sanPham = await _context.SanPhams.Where(x => x.MaSanPham == productid).FirstOrDefaultAsync();

        //    if (sanPham == null) return NotFound(_localization.Getkey("KhongTimThaySP") + " " + productid);

        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(string productid) => RedirectToPage("./Index");

        public async Task<IActionResult> OnPostAsync(string productid)
        {
            if (productid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThaySP") + " " + productid, 3);
                return RedirectToPage("./Index");
            }

            sanPham = await _context.SanPhams.Where(x => x.MaSanPham == productid).FirstOrDefaultAsync();

            if (sanPham == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThaySP") + " " + productid, 3);
                return RedirectToPage("./Index");
            }

            var oldSanPham = sanPham.TenSanPham;

            gioHangs = await _context.GioHangs.Where(x => x.MaSanPham == productid).ToListAsync();

            if (gioHangs.Count() > 0)
            {
                foreach (var gioHang in gioHangs)
                {
                    gioHang.MaSanPham = null;
                    await _context.SaveChangesAsync();
                }
            }

            //var khoaChinh = await _context.GioHangs.Where(x => x.MaSanPham == productid).ToListAsync();

            //if (khoaChinh.Any())
            //{
            //    _notyf.Error(_localization.Getkey("CannotDeleteProduct") + " " + oldSanPham + " !", 3);
            //    return RedirectToPage("./Index");
            //}

            _context.SanPhams.Remove(await _context.SanPhams.FindAsync(productid));
            await _context.SaveChangesAsync();

            _notyf.Success(_localization.Getkey("DaXoaSP") + " " + oldSanPham + " " + _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("DaXoaSP") + " " + oldSanPham + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

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
