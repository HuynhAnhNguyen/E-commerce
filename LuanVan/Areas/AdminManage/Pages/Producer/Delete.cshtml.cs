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
    [Authorize(Roles = "Admin")]

    public class DeleteModel : ColorPageModel
    {
        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public NhaSanXuat nhaSanXuat { get; set; }

        public List<SanPham> sanPhams { get; set; }
        //public async Task<IActionResult> OnGetAsync(string producerid)
        //{
        //    if (producerid == null) return NotFound(_localization.Getkey("KhongTimThayNSX") + " " + producerid);

        //    nhaSanXuat =await _context.NhaSanXuats.Where(x => x.MaNsx == producerid).FirstOrDefaultAsync();

        //    if(nhaSanXuat == null) return NotFound(_localization.Getkey("KhongTimThayNSX") + " " + producerid);

        //    return Page();
        //}
        public async Task<IActionResult> OnGetAsync(string producerid) => RedirectToPage("./Index");

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

            var oldNSX = nhaSanXuat.TenNsx;

            sanPhams = await _context.SanPhams.Where(x => x.MaNsx == producerid).ToListAsync();

            if (sanPhams.Count() > 0)
            {
                foreach (var sanPham in sanPhams)
                {
                    sanPham.MaNsx = null;
                    await _context.SaveChangesAsync();
                }
            }

            //var khoaChinh = await _context.SanPhams.Where(x => x.MaNsx == producerid).ToListAsync();
            //if (khoaChinh.Any())
            //{
            //    _notyf.Error(_localization.Getkey("CannotDeleteNSX") + " " + oldNSX + " !", 3);
            //    return RedirectToPage("./Index");
            //}

            _context.NhaSanXuats.Remove(await _context.NhaSanXuats.FindAsync(producerid));
            await _context.SaveChangesAsync();

            _notyf.Success(_localization.Getkey("DaXoaNSX") + " " + oldNSX + " " + _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("DaXoaNSX") + " " + oldNSX + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

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
