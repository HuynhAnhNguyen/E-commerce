using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    public class LoginController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;
        private readonly SignInManager<KhachHang> _signInManager;


        public LoginController(INotyfService notyf, SignInManager<KhachHang> signInManager, LanguageService localization)
        {
            _notyf = notyf;
            _signInManager = signInManager;
            _localization = localization;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            _notyf.Information(_localization.Getkey("Logout_Success"), 3);

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
