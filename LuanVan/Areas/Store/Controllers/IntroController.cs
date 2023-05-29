using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    public class IntroController : Controller
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly Service _service;
        private readonly LanguageService _localization;


        public IntroController(UserManager<KhachHang> userManager, Service service, LanguageService localization)
        {
            _userManager = userManager;
            _service = service;
            _localization = localization;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if (currentCulture == "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            string maKH = null;

            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine("MAKH: " + maKH);
            }

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

            return View();
        }
    }
}
