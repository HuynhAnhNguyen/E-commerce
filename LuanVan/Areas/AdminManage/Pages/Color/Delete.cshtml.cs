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
using LuanVan.Areas.AdminManage.Pages.Color;
using System.Drawing;
using LuanVan.Services;

namespace LuanVan.Areas.AdminManage.Pages.Color
{
    [Authorize(Roles = "Admin")]


    public class DeleteModel : ColorPageModel
    {
        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public MauSac mauSac { get; set; }

        public List<SanPham> sanPhams { get; set; }
        //public async Task<IActionResult> OnGetAsync(string colorid)
        //{
        //    //if (colorid == null) return NotFound(_localization.Getkey("KhongThayMauSac") + " " + colorid);

        //    //mauSac = await _context.MauSacs.Where(x => x.MaMau == colorid).FirstOrDefaultAsync();

        //    //if (mauSac == null) return NotFound(_localization.Getkey("KhongThayMauSac") + " " + colorid);

        //    //return Page();
        //}
        public async Task<IActionResult> OnGetAsync(string colorid) => RedirectToPage("./Index");

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

            var oldNSX = mauSac.TenMau;

            sanPhams = await _context.SanPhams.Where(x => x.MaMau == colorid).ToListAsync();

            if (sanPhams.Count() > 0)
            {
                foreach (var sanPham in sanPhams)
                {
                    sanPham.MaMau = null;
                    await _context.SaveChangesAsync();
                }
            }

            //var khoaChinh = await _context.SanPhams.Where(x => x.MaMau == colorid).ToListAsync();

            //if (khoaChinh.Any())
            //{
            //    _notyf.Error(_localization.Getkey("CannotDeleteColor") +" "+ oldNSX+" !", 3);
            //    //return Page();
            //    return RedirectToPage("./Index");
            //}

            _context.MauSacs.Remove(await _context.MauSacs.FindAsync(colorid));
            await _context.SaveChangesAsync();
            _notyf.Success(_localization.Getkey("DaXoaMau") +" " + oldNSX +" "+ _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("DaXoaMau") + " " + oldNSX + " "+ _localization.Getkey("ThanhCongLuc") +" " + DateTimeVN();

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
