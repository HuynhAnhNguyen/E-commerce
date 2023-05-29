using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LuanVan.Areas.AdminManage.Pages.Payment
{
    [Authorize(Roles ="Admin, Test, Editor")]
    public class IndexModel : PaymentPageModel
    {
        public IndexModel(ApplicationDbContext context, INotyfService notyf, ILogger<PaymentPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }
        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }
        public List<ThanhToan> pays { get; set; }
        public List<ThanhToan> soLuongPays { get; set; }

        public async Task OnGetAsync(string Search)
        {
            soLuongPays = await _context.ThanhToans.ToListAsync();
            if(soLuongPays.Count()> 0)
            {
                int totalPay = await _context.ThanhToans.CountAsync();

                countPage = (int)Math.Ceiling((double)totalPay / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;
                var qr = (from p in _context.ThanhToans orderby p.MaPttt select p);


                if (!string.IsNullOrEmpty(Search))
                {
                    pays = await qr.Where(x => x.MaPttt.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    pays = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
