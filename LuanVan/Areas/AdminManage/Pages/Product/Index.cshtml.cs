using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Product
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class IndexModel : ProductPageModel
    {
        public IndexModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }
        public List<SanPham> soLuongSanPham { get; set; }

        public List<SanPham> products { get; set; }

        public string path = "/images/product";

        public const int ITEMS_PER_PAGE = 5;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }
        public async Task OnGetAsync(string Search)
        {
            soLuongSanPham = await _context.SanPhams.ToListAsync();
            if(soLuongSanPham.Count() > 0)
            {
                int totalProduct = await _context.SanPhams.CountAsync();

                countPage = (int)Math.Ceiling((double)totalProduct / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;

                var qr = (from p in _context.SanPhams orderby p.SoLuongTon ascending select p);

                if (!string.IsNullOrEmpty(Search))
                {
                    products = await qr.Where(x => x.TenSanPham.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    products = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();

                }
            }

        }

        public void OnPost() => RedirectToPage();
       
    }
}
