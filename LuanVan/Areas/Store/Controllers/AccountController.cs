using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly Service _service;
        private readonly LanguageService _localization;


        public AccountController(UserManager<KhachHang> userManager, Service service, LanguageService localization)
        {
            _userManager = userManager;
            _service = service; 
            _localization = localization;   
        }

        [HttpGet]
        public async Task<IActionResult> Info()
        {
            string maKH = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }
            
            var model = await _service.getKH(maKH);

            ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();

            ViewData["path"] = "/images/product/";

            if (maKH != null)
            { 
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync(); 
            }
            else
            {
                ViewData["cart_items"] = new List<GioHang>();
            }

            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if (currentCulture == "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            return View(model);
        }
    }
}
