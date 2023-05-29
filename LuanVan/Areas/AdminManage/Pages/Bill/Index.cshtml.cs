using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Bill
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class IndexModel : BillPageModel
    {
        public IndexModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPage { get; set; }
        public List<HoaDon> bills { get; set; }
        public List<HoaDon> soLuongHD { get; set; }
        public async Task OnGetAsync(string Search)
        {
            var nullHoaDonList = await _context.HoaDons.Where(hd => hd.TongGiaTri == null).ToListAsync();

            if (nullHoaDonList.Count() > 0)
            {
                foreach (var nullHoaDon in nullHoaDonList)
                {
                    var cthdList = await _context.ChiTietHds.Where(cthd => cthd.MaHoaDon == nullHoaDon.MaHoaDon).ToListAsync();

                    // Tiến hành xóa các chi tiết hóa đơn
                    foreach (var cthd in cthdList)
                    {
                        _context.ChiTietHds.Remove(cthd);
                    }
                    // Xóa hóa đơn
                    _context.HoaDons.Remove(nullHoaDon);
                }

                await _context.SaveChangesAsync();
            }

            soLuongHD = await _context.HoaDons.ToListAsync();

            if (soLuongHD.Count > 0)
            {
                int totalBill = await _context.HoaDons.CountAsync();

                countPage = (int)Math.Ceiling((double)totalBill / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;

                if (currentPage > countPage)
                    currentPage = countPage;

                var qr = (from p in _context.HoaDons orderby p.MaHoaDon descending select p);

                if (!string.IsNullOrEmpty(Search))
                {
                    bills = await qr.Where(x => x.MaHoaDon.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    bills = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
            }
        }

        public void OnPost() => RedirectToPage();

    }
}
