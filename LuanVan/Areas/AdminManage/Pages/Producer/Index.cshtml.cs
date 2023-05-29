using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LuanVan.Areas.AdminManage.Pages.Producer
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class IndexModel : ColorPageModel
    {
        public IndexModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }
        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }
        public List<NhaSanXuat> producers { get; set; }
        public List<NhaSanXuat> soLuongNSX { get; set; }
        public async Task OnGetAsync(string Search)
        {
            soLuongNSX = await _context.NhaSanXuats.ToListAsync();
            if (soLuongNSX.Count() > 0)
            {
                int totalProducer = await _context.NhaSanXuats.CountAsync();
                countPage = (int)Math.Ceiling((double)totalProducer / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;
                var qr = (from p in _context.NhaSanXuats orderby p.MaNsx select p);

                if (!string.IsNullOrEmpty(Search))
                {
                    producers = await qr.Where(x => x.TenNsx.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    producers = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();

                }
            }

        }

        public void OnPost() => RedirectToPage();
    }
}
