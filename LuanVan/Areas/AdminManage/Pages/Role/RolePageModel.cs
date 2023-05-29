using LuanVan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Services;

namespace LuanVan.Areas.AdminManage.Pages.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly ApplicationDbContext _context;
        protected readonly INotyfService _notyf;
        protected readonly LanguageService _localization;


        [TempData]
        public string StatusMessage { get; set; }

        public RolePageModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization)
        {
            _roleManager = roleManager;
            _context = context;
            _notyf = notyf;
            _localization = localization;
        }


    }
}
