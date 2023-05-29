using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin, Editor, Test")]


    public class ListReceiptModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly LanguageService _localization;
        private readonly INotyfService _notyf;


        public ListReceiptModel(UserManager<KhachHang> userManager, ApplicationDbContext context, LanguageService localization, INotyfService notyf)
        {
            _userManager = userManager;
            _context = context;
            _localization = localization;
            _notyf = notyf;
        }
        [TempData]
        public string StatusMessage { get; set; }

        public KhachHang user { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }
        public List<HoaDon> bills { get; set; }
        public List<HoaDon> soLuongHDById { get; set; }

        public string KhachHangId { get; set; }

        public async Task<IActionResult> OnGetAsync(string id, string Search)
        {
            if (string.IsNullOrEmpty(id))
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            //var nullHoaDonList = await _context.HoaDons.Where(hd => hd.TongGiaTri.ToString() == "").ToListAsync();
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

            soLuongHDById = await _context.HoaDons.Where(a => a.KhachHangId == id).ToListAsync();

            if (soLuongHDById.Count > 0)
            {
                int totalBillById = await _context.HoaDons.Where(a => a.KhachHangId == id).CountAsync();

                countPage = (int)Math.Ceiling((double)totalBillById / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;

                var qr = (from kh in _context.KhachHangs
                          join hd in _context.HoaDons on kh.Id equals hd.KhachHangId
                          orderby hd.NgayXuatHd descending
                          where kh.Id == id
                          select hd);

                if (!string.IsNullOrEmpty(Search))
                {
                    KhachHangId = id;
                    bills = await qr.Where(x => x.MaHoaDon.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    KhachHangId = id;
                    bills = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                //bills = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
            }

            return Page();
        }

        public void OnPost() => RedirectToPage();
    }
}
