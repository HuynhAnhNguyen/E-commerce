using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    public class HomeController : Controller
    {
        private readonly Service _service;
        private readonly UserManager<KhachHang> _userManager;
        private readonly LanguageService _localization;


        public HomeController(Service service, UserManager<KhachHang> userManager, LanguageService localization)
        {
            _service = service;
            _userManager = userManager;
            _localization = localization;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int size = 8)
        {
            string maKH = null;

            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
            }

            ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();
            ViewData["path"] = "/images/product/";
            ViewData["hot_items"] = await _service.danhSachSanPhamHot().Take(8).ToListAsync();
            ViewData["discount_items"] = await _service.danhSachSanPhamKhuyenMai().Take(8).ToListAsync();
            ViewData["top12products"] = _service.danhSachSanPhamBanChay().Take(8).ToList();

            if (maKH != null)
            {
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();
            }
            else
            {
                ViewData["cart_items"] = new List<GioHang>();
            }

            page = page < 1 ? 1 : page;
            size = size < 1 ? 1 : size;

            IPagedList<SanPham> sanPhams = await _service.PagingSanPhams(page, size);

            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if(currentCulture== "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            return View(sanPhams);
        }

        //[HttpGet]
        //public async Task<IActionResult> getListCartItem()
        //{
        //    return View(await _service.danhSachGioHang().ToListAsync());
        //}

        [HttpGet]
        public IActionResult pageNotFound()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Paging(int page = 1, int size = 8)
        {
            page = page < 1 ? 1 : page;
            size = size < 1 ? 1 : size;
            IPagedList<SanPham> sanPhams = await _service.PagingSanPhams(page, size);
            return PartialView("_Product_Filter", sanPhams);
        }

        #region Localization
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        #endregion 
    }
}
