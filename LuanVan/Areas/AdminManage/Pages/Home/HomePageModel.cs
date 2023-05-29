using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuanVan.Areas.AdminManage.Pages.Home
{
    public class HomePageModel: PageModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly LanguageService _localization;


        public HomePageModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, LanguageService localization)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _localization = localization;
        }
    }
}
