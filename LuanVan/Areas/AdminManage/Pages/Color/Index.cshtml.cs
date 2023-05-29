using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Color
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
        public List<MauSac> colors { get; set; }

        public List<MauSac> soLuongMauSac { get; set; }
        public async Task OnGetAsync(string Search)
        {
            soLuongMauSac= await _context.MauSacs.ToListAsync();

            if(soLuongMauSac.Count() > 0)
            {
                int totalColor = await _context.MauSacs.CountAsync();

                countPage = (int)Math.Ceiling((double)totalColor / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;
                var qr = (from p in _context.MauSacs orderby p.MaMau select p);

                if (!string.IsNullOrEmpty(Search))
                {
                    colors = await qr.Where(x => x.TenMau.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    colors = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
            }

        }

        public void OnPost() => RedirectToPage();
    }
}
