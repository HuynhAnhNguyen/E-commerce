using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.ProductType
{
    [Authorize(Roles = "Admin")]


    public class DeleteModel : ProductTypePageModel
    {
        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductTypePageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public LoaiSanPham loaiSanPham { get; set; }

        public List<SanPham> sanPhams { get; set; }
        //public async Task<IActionResult> OnGetAsync(string producttypeid)
        //{
        //    if (producttypeid == null) return NotFound(_localization.Getkey("KhongTimThayLoaiSP") + " " + producttypeid);

        //    loaiSanPham = await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == producttypeid).FirstOrDefaultAsync();

        //    if (loaiSanPham == null) return NotFound(_localization.Getkey("KhongTimThayLoaiSP") + " " + producttypeid);

        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(string producttypeid) => RedirectToPage("./Index");

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

            var oldLSP = loaiSanPham.TenLoaiSp;

            sanPhams = await _context.SanPhams.Where(x => x.MaLoaiSp == producttypeid).ToListAsync();

            if (sanPhams.Count() > 0)
            {
                foreach (var sanPham in sanPhams)
                {
                    sanPham.MaLoaiSp = null;
                    await _context.SaveChangesAsync();
                }
            }

            //var khoaChinh =await _context.SanPhams.Where(x => x.MaLoaiSp == producttypeid).ToListAsync();
            //if (khoaChinh.Any())
            //{
            //    _notyf.Error(_localization.Getkey("CannotDeleteLSP") + " " + oldLSP + " !", 3);
            //    return RedirectToPage("./Index");
            //}

            _context.LoaiSanPhams.Remove(await _context.LoaiSanPhams.FindAsync(producttypeid));
            await _context.SaveChangesAsync();

            _notyf.Success(_localization.Getkey("DaXoaLSP") + " " + oldLSP + " " + _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("DaXoaLSP") + " " + oldLSP + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();


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
