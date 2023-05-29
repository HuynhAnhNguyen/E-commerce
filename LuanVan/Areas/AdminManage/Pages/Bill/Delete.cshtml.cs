using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Bill
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : BillPageModel
    {

        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public HoaDon hoaDon { get; set; }

        public List<ChiTietHd> chiTietHoaDons { get; set; }
        //public async Task<IActionResult> OnGetAsync(string billid)
        //{
        //    if (billid == null) return NotFound("" + _localization.Getkey("KhongThayHoaDon") + billid);

        //    hoaDon = await _context.HoaDons.Where(x => x.MaHoaDon == billid).FirstOrDefaultAsync();

        //    if (hoaDon == null) return NotFound("" + _localization.Getkey("KhongThayHoaDon") + billid);

        //    return Page();

        //}
        public async Task<IActionResult> OnGetAsync(string billid) => RedirectToPage("./Index");

        public async Task<IActionResult> OnPostAsync(string billid)
        {
            if (billid == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }

            hoaDon = await _context.HoaDons.Where(x => x.MaHoaDon == billid).FirstOrDefaultAsync();

            if (hoaDon == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }

            chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == billid).ToListAsync();

            if (chiTietHoaDons.Count()> 0)
            {
                foreach (var chiTietHoaDon in chiTietHoaDons)
                {
                    _context.ChiTietHds.Remove(await _context.ChiTietHds.FindAsync(chiTietHoaDon.MaChiTietHd));
                    await _context.SaveChangesAsync();
                }
            }

            _context.HoaDons.Remove(await _context.HoaDons.FindAsync(billid));
            await _context.SaveChangesAsync();
            _notyf.Success(_localization.Getkey("DeleteBillSuccess") +" " + billid + " "+ _localization.Getkey("Thanhcong"), 3);

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
