using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.ProductType
{
    [Authorize(Roles = "Admin, Editor, Test")]


    public class IndexModel : ProductTypePageModel
    {
        public IndexModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductTypePageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public List<LoaiSanPham> soLuongLoaiSP { get; set; }


        public List<LoaiSanPham> productTypes { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }

        public async Task OnGetAsync(string Search)
        {
            soLuongLoaiSP= await _context.LoaiSanPhams.ToListAsync();
           
            if(soLuongLoaiSP.Count() > 0)
            {
                int totalProductType = await _context.LoaiSanPhams.CountAsync();

                countPage = (int)Math.Ceiling((double)totalProductType / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;
                var qr = (from p in _context.LoaiSanPhams orderby p.MaLoaiSp select p);

                if (!string.IsNullOrEmpty(Search))
                {
                    productTypes = await qr.Where(x => x.TenLoaiSp.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    productTypes = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();

                }
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
