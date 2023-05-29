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
    public class CartController : Controller
    {
        private readonly Service _service;
        private readonly UserManager<KhachHang> _userManager;
        private readonly LanguageService _localization;

        public CartController(Service service, UserManager<KhachHang> userManager, LanguageService localization)
        {
            _service = service;
            _userManager = userManager;
            _localization = localization;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyItems()
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
                Console.WriteLine(maKH);

                ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();
                ViewData["path"] = "/images/product/";
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();

                return View(await _service.danhSachGioHang(0, maKH).ToListAsync());
            }
            return View();

        }

        [HttpPost]
        public async Task<PartialViewResult> RemoveItems(string maGH)
        {
            var model = await _service.getGioHang(maGH);
            if (model.TrangThai == 0)
            {
                Console.WriteLine("Delete MAGH: " + maGH);
                await _service.xoaGioHang(maGH);
            }
            return await get_cart();

        }


        [HttpPost]
        public async Task<PartialViewResult> RemoveCartItem(string maGH)
        {
            string maKH = null;
            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }

            var model = await _service.getGioHang(maGH);
            if (model.TrangThai == 0)
            {
                await _service.xoaGioHang(maGH);
            }

            ViewData["path"] = "/images/product/";
            return PartialView("_Cart_Full", await _service.danhSachGioHang(0, maKH).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> AddItems(string masp)
        {
            string maKH = null;
            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }

            if (maKH == null)
            {
                return Json(null);
            }
            string kq = "Not add";
            if (!string.IsNullOrEmpty(masp))
            {

                kq = await _service.themGioHang(masp, maKH);
            }
            return Json(kq);
        }


        [HttpPost]
        public async Task<PartialViewResult> get_cart()
        {
            string maKH = null;
            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }
            return PartialView("_Cart", await _service.danhSachGioHang(0, maKH).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> Increase(string maGH, int soluong)
        {
            Console.WriteLine("soLuong: " + soluong);

            await _service.increase(maGH, soluong);
            Console.WriteLine("MAGH: " + maGH + ", SL: " + soluong);
            return Json(maGH + " " + soluong);
        }

    }
}
