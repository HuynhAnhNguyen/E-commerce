using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using X.PagedList;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    public class ProductController : Controller
    {
        private readonly Service _service;
        private readonly UserManager<KhachHang> _userManager;
        private readonly LanguageService _localization;



        public ProductController(Service service, UserManager<KhachHang> userManager, LanguageService localization)
        { 
            _service = service;
            _userManager = userManager;
            _localization = localization;
        }


        [HttpGet]
        public async Task<IActionResult> Search(string key, int page = 1, int size = 8)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if (currentCulture == "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            ViewData["hot_items"] = await _service.danhSachSanPhamHot().Take(12).ToListAsync();
            ViewData["discount_items"] = await _service.danhSachSanPhamKhuyenMai().Take(12).ToListAsync();
            ViewData["top12products"] = _service.danhSachSanPhamBanChay().Take(12).ToList();

			string maKH = null;
			if (User.Identity.IsAuthenticated)
			{
				KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
				maKH = user.Id;
				Console.WriteLine(maKH);
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

			ViewData["hot_items"] = await _service.danhSachSanPhamHot().ToListAsync();

			if (key.IsNullOrEmpty())
            {
                ViewData["soluong"] = (int)0;
                ViewData["key"] = "";
                return View();
			}
			else
            {
                ViewData["soluong"] = (int)1;
                ViewData["key"] = key;

                HttpContext.Session.SetString("Key", key);

                page = (page < 1) ? 1 : page;
                size = (size < 1) ? 1 : size;

                IPagedList<SanPham> sanPhamTheoKey = await _service.PagingSanPhamsByKey(key, page, size);

                return View(sanPhamTheoKey);
            }
        }


        [HttpGet]
        public async Task<IActionResult> PagingSanPhamTheoKey(int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 1) ? 1 : size;

            string key = HttpContext.Session.GetString("Key");

            IPagedList<SanPham> sanPhamTheoKey = await _service.PagingSanPhamsByKey(key, page, size);

            return PartialView("_Product_Filters_SanPhamTheoKey", sanPhamTheoKey);
        }


        [HttpGet]
        public async Task<IActionResult> List_product(string maLoai, int page = 1, int size = 8)
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

            ViewData["loaisp"] = await _service.getTenLoaiSP(maLoai);
            ViewData["maloai"] = maLoai;

            ViewData["hot-items"] = await _service.danhSachSanPhamHot().ToListAsync();

            HttpContext.Session.SetString("MaLoai", maLoai);

            page = (page < 1) ? 1 : page;
            size = (size < 1) ? 1 : size;

            IPagedList<SanPham>  sanPhamTheoLoai = await _service.PagingSanPhamsByLoaiSP(maLoai, page, size);

            return View(sanPhamTheoLoai);
        }


        [HttpGet]
        public async Task<IActionResult> PagingSanPhamTheoLoai(int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 1) ? 1 : size;

            string maLoai = HttpContext.Session.GetString("MaLoai");

            IPagedList<SanPham> sanPhamTheoLoai;

            if (maLoai.IsNullOrEmpty())
            {
                sanPhamTheoLoai = await _service.PagingProductByLoaiSP("", page, size);
            }
            else
            {
                sanPhamTheoLoai = await _service.PagingProductByLoaiSP(maLoai, page, size);
            }

            return PartialView("_Product_Filters_SanPhamTheoLoai", sanPhamTheoLoai);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(string maSP)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if (currentCulture == "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            ViewData["hot_items"] = await _service.danhSachSanPhamHot().Take(12).ToListAsync();
            ViewData["discount_items"] = await _service.danhSachSanPhamKhuyenMai().Take(12).ToListAsync();
            ViewData["top12products"] = _service.danhSachSanPhamBanChay().Take(12).ToList();
            string maKH = null;

            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }

            ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();
            ViewData["path"] = "/images/product/";
            ViewData["NSX"] = await _service.getNhaSXBySanPham(maSP);
            ViewData["MauSac"] = await _service.getMauSacBySanPham(maSP);
            ViewData["Mota"] = await _service.getMotaBySanPham(maSP);
            ViewData["hot_items"] = await _service.danhSachSanPhamHot().Take(12).ToListAsync();
            
            if (maKH != null)
            {
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();
            }
            else
            {
                ViewData["cart_items"] = new List<GioHang>();
            }

            return View(await _service.GetSanPham(maSP));
        }


        [HttpPost]
        public async Task<PartialViewResult> Get_by_cate(string maloai="0", int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 1) ? 1 : size;
            IPagedList<SanPham> sanPhams;

            ViewData["path"] = "/images/product/";

            if (maloai == "0")
            {
                maloai = "";
                sanPhams = await _service.PagingProductByLoaiSP("", page, size);
            }
            else
            {
                sanPhams = await _service.PagingProductByLoaiSP(maloai, page, size);
            }

            HttpContext.Session.SetString("MaLoai", maloai);

            return PartialView("_Product_Filters_SanPhamTheoLoai", sanPhams);
        }

        [HttpGet]
        public async Task<IActionResult> Paging(int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 1) ? 1 : size;

            IPagedList<SanPham> sanPhams = await _service.danhSachSanPham().ToPagedListAsync(page, size);

            return PartialView("_Product_Filters", sanPhams);
        }


        [HttpPost]
        public async Task<PartialViewResult> SortName(string maloai, string value, int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 1) ? 1 : size;

            IPagedList<SanPham> sanPhams;

            if (value.Equals("priceASC"))
            {
                sanPhams = await _service.PagingSortProductByPrice(maloai, true, page, size);

            }
            else if (value.Equals("priceDESC"))
            {
                sanPhams = await _service.PagingSortProductByPrice(maloai, false, page, size);

            }
            else if (value.Equals("nameASC"))
            {
                sanPhams = await _service.PagingSortProductByName(maloai, true, page, size);

            }
            else if (value.Equals("nameDESC"))
            {
                sanPhams = await _service.PagingSortProductByName(maloai, false, page, size);

            }
            else
            {
                sanPhams = await _service.PagingSortProductByPrice(maloai, true, page, size);
            }

            string maKH = null;

            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }

            ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();
            ViewData["path"] = "/images/product/";
            ViewData["hot_items"] = await _service.danhSachSanPham().ToListAsync();
            ViewData["loaisp"] = await _service.getTenLoaiSP(maloai);
            ViewData["maloai"] = maloai;

            if (maKH != null)
            {
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();
            }
            else
            {
                ViewData["cart_items"] = new List<GioHang>();
            }

            return PartialView("_Product_Filters_SanPhamTheoLoai", sanPhams);

        }

    }
}
