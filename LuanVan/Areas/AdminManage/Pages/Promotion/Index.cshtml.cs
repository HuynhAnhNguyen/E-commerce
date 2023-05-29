
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Promotion
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class IndexModel : PromotionPageModel
    {
        public IndexModel(ApplicationDbContext context, INotyfService notyf, ILogger<PromotionPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public List<KhuyenMai> soLuongKM { get; set; }

        public List<KhuyenMai> khuyenMais { get; set; }

        public const int ITEMS_PER_PAGE = 10;


        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }

        public async Task OnGetAsync(string Search)
        {
            soLuongKM = await _context.KhuyenMais.ToListAsync();
            if(soLuongKM.Count()> 0)
            {
                int totalKhuyenMai = await _context.KhuyenMais.CountAsync();
                countPage = (int)Math.Ceiling((double)totalKhuyenMai / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;
                var qr = (from p in _context.KhuyenMais orderby p.NgayBatDau descending select p);

                if (!string.IsNullOrEmpty(Search))
                {
                    khuyenMais = await qr.Where(x => x.TenKhuyenMai.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();
                }
                else
                {
                    khuyenMais = await qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToListAsync();

                }
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
