using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Areas.AdminManage.Pages.Producer;
using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuanVan.Areas.AdminManage.Pages.Color
{
    public class ColorPageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly INotyfService _notyf;
        protected readonly ILogger<ColorPageModel> _logger;
        protected readonly LanguageService _localization;


        [TempData]
        public string StatusMessage { get; set; }
        public ColorPageModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization)
        {
            _context = context;
            _notyf = notyf;
            _logger = logger;
            _localization= localization;    
        }
    }
}
