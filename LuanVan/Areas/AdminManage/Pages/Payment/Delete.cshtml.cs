using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Payment
{
    [Authorize(Roles = "Admin")]

    public class DeleteModel : PaymentPageModel
    {
        public DeleteModel(ApplicationDbContext context, INotyfService notyf, ILogger<PaymentPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public ThanhToan thanhToan { get; set; }

        public List<HoaDon> hoaDons { get; set; }
        //public async Task<IActionResult> OnGetAsync(string paymentid)
        //{
        //    if (paymentid == null) return NotFound(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid);

        //    thanhToan = await _context.ThanhToans.Where(x => x.MaPttt == paymentid).FirstOrDefaultAsync();

        //    if (thanhToan == null) return NotFound(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid);

        //    return Page();
        //}
        public async Task<IActionResult> OnGetAsync(string paymentid) => RedirectToPage("./Index");

        public async Task<IActionResult> OnPostAsync(string paymentid)
        {
            if (paymentid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid, 3);
                return RedirectToPage("./Index");
            }

            thanhToan = await _context.ThanhToans.Where(x => x.MaPttt == paymentid).FirstOrDefaultAsync();

            if (thanhToan == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayPTTT") + " " + paymentid, 3);
                return RedirectToPage("./Index");
            }

            var oldPay = thanhToan.TenPttt;

            hoaDons = await _context.HoaDons.Where(x => x.MaPttt == paymentid).ToListAsync();

            if (hoaDons.Count() > 0)
            {
                foreach (var hoaDon in hoaDons)
                {
                    hoaDon.MaPttt = null;
                    await _context.SaveChangesAsync();
                }
            }

            //var khoaChinh = await _context.HoaDons.Where(x => x.MaPttt == paymentid).ToListAsync();
            //if (khoaChinh.Any())
            //{
            //    _notyf.Error(_localization.Getkey("CannotDeletePTTT") + " " + oldPay + " !", 3);
            //    return RedirectToPage("./Index");
            //}

            _context.ThanhToans.Remove(await _context.ThanhToans.FindAsync(paymentid));
            await _context.SaveChangesAsync();

            _notyf.Success(_localization.Getkey("DaXoaPTTT") + " " + oldPay + " " + _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("DaXoaMau") + " " + oldPay + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

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
