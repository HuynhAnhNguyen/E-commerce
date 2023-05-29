
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Promotion
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : PromotionPageModel
    {
        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<PromotionPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }
        
        public KhuyenMai khuyenMai { get; set; }

        public List<HoaDon> hoaDons { get; set; }

        //public async Task<IActionResult> OnGetAsync(string promotionid)
        //{
        //    if (promotionid == null) return NotFound(_localization.Getkey("KhongTimThayKM") + " " + promotionid);

        //    khuyenMai =await _context.KhuyenMais.Where(x => x.MaKm == promotionid).FirstOrDefaultAsync();

        //    if (khuyenMai == null) return NotFound(_localization.Getkey("KhongTimThayKM") + " " + promotionid);

        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(string promotionid) => RedirectToPage("./Index");

        public async Task<IActionResult> OnPostAsync(string promotionid)
        {
            if (promotionid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayKM") + " " + promotionid, 3);
                return RedirectToPage("./Index");
            }

            khuyenMai = await _context.KhuyenMais.Where(x => x.MaKm == promotionid).FirstOrDefaultAsync();

            if (khuyenMai == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayKM") + " " + promotionid, 3);
                return RedirectToPage("./Index");
            }


            hoaDons = await _context.HoaDons.Where(x => x.MaKm == promotionid).ToListAsync();

            if (hoaDons.Count() > 0)
            {
                foreach (var hoaDon in hoaDons)
                {
                    hoaDon.MaKm = null;
                    await _context.SaveChangesAsync();
                }
            }

            var oldCTKM = khuyenMai.TenKhuyenMai;

            //var khoaChinh = await _context.HoaDons.Where(x => x.MaKm == promotionid).ToListAsync();
            //if (khoaChinh.Any())
            //{
            //    _notyf.Error(_localization.Getkey("CannotDeleteKM") + " " + oldCTKM + " !", 3);
            //    return RedirectToPage("./Index");
            //}

            _context.KhuyenMais.Remove(await _context.KhuyenMais.FindAsync(promotionid));
            await _context.SaveChangesAsync();

            _notyf.Success(_localization.Getkey("DaXoaKM") + " " + oldCTKM + " " + _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("DaXoaKM") + " " + oldCTKM + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();


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
