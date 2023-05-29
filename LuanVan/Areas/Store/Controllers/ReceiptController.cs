using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using LuanVan.Areas.Store.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using LuanVan.VNPays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Text.Encodings.Web;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    public class ReceiptController : Controller
    {
        private readonly Service _service;
        private readonly UserManager<KhachHang> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly LanguageService _localization;



        public ReceiptController(Service service, UserManager<KhachHang> userManager, ApplicationDbContext context, LanguageService localization)
        {
            _service = service;
            _userManager = userManager;
            _context = context;
            _localization = localization;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List()
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

            var model = await _service.danhSachHoaDon(maKH).ToListAsync();
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
            return View(model);
        }


        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> Detail(string mahd)
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

            HoaDon hoaDon = await _service.getHoaDon(mahd);
            
            ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();
            ViewData["path"] = "/images/product/";
            ViewData["hoadon"] = await _service.getHoaDon(mahd);

            ViewData["hinhthuctt"] = await (from a in _context.HoaDons join b in _context.ThanhToans on a.MaPttt equals b.MaPttt where a.MaHoaDon == mahd select b.TenPttt).FirstOrDefaultAsync();
            
            if (hoaDon.TrangThaiThanhToan == -1)
            {
                ViewData["trangThaiThanhToan"] = _localization.Getkey("Pay_error");
            }
            else if (hoaDon.TrangThaiThanhToan == 0)
            {
                ViewData["trangThaiThanhToan"] = _localization.Getkey("Waiting_for_refund");
            }
            else if (hoaDon.TrangThaiThanhToan == 1)
            {
                ViewData["trangThaiThanhToan"] = _localization.Getkey("Pay_success");
            }
            else
            {
                ViewData["trangThaiThanhToan"] = _localization.Getkey("ChoThanhToan");
            }
            

            if (hoaDon.TrangThaiDonHang == -1)
            {
                ViewData["trangThaiDonHang"] = _localization.Getkey("Cancel_bill");
            }
            else if (hoaDon.TrangThaiDonHang == 0)
            {
                ViewData["trangThaiDonHang"] = _localization.Getkey("Waiting_for_delivery");
            }
            else if (hoaDon.TrangThaiDonHang == 1)
            {
                ViewData["trangThaiDonHang"] = _localization.Getkey("Delivery_in_progress");
            }
            else 
            {
                ViewData["trangThaiDonHang"] = _localization.Getkey("Delivery_successful");
            }


            if (maKH != null)
            {
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();
            }
            else
            {
                ViewData["cart_items"] = new List<GioHang>();
            }

            ViewData["chiTietHoaDons"] = await _context.ChiTietHds.Where(x => x.MaHoaDon == mahd).ToListAsync();
            List<ChiTietHd> chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == mahd).ToListAsync();

            List<SanPham> sanPhams = new List<SanPham>();
            List<GioHang> gioHangs = new List<GioHang>();
            List<LoaiSanPham> loaiSanPhams = new List<LoaiSanPham>();

            foreach (var chiTietHoaDon in chiTietHoaDons)
            {
                GioHang gioHang = await _context.GioHangs.Where(x => x.MaGioHang == chiTietHoaDon.MaGioHang).FirstOrDefaultAsync();
                SanPham sanPham = await _context.SanPhams.Where(x => x.MaSanPham == gioHang.MaSanPham).FirstOrDefaultAsync();
                LoaiSanPham loaiSanPham = await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == sanPham.MaLoaiSp).FirstOrDefaultAsync();

                sanPhams.Add(sanPham);
                gioHangs.Add(gioHang);
                loaiSanPhams.Add(loaiSanPham);
            }

            ViewData["sanPhams"] = sanPhams;
            ViewData["gioHangs"] = gioHangs;
            ViewData["loaiSanPhams"] = loaiSanPhams;

            var query = await _service.GetMaKM(mahd);

            if (query != null)
            {
                KhuyenMai khuyenMai = await _service.GetKhuyenMai(query);
                ViewData["ApDungKM"] = khuyenMai.MaKm;
                ViewData["PhanTramGiamGia"] = khuyenMai.GiaTriKm;
                double total = 0;
                foreach(var sanPham in sanPhams)
                {
                    total +=sanPham.GiaBan;
                }
                double soTienGiam = total * (khuyenMai.GiaTriKm/100);
                ViewData["SoTienGiamGia"] = @String.Format("{0: ### ### ### ### VNĐ}", soTienGiam);
            }
            else
            {
                ViewData["ApDungKM"] = _localization.Getkey("KAD");
                ViewData["PhanTramGiamGia"] = "0";
                ViewData["SoTienGiamGia"] = "0 VND";
            }


            return View();
        }

        [HttpPost]
        public IActionResult CheckBill(string billCode)
        {
            // Kiểm tra xem mã hóa đơn có tồn tại trong cơ sở dữ liệu hay không
            var billID = _context.HoaDons.FirstOrDefault(d => d.MaHoaDon == billCode);

            if (billID == null)
            {
                return Json(new { success = false, message = "Mã hóa đơn không tồn tại." });
            }

            Console.WriteLine(billCode);

            // Nếu mã hóa đơn hợp lệ, trả về mã hóa đơn tương ứng
            return Json(new { success = true, billID = billID });
            //return RedirectToAction("Detail", "Receipt", new { mahd = billID.MaHoaDon });
        }

        [HttpPost]
        public async Task<IActionResult> CancelBill(string maHoaDon)
        {
            // Kiểm tra xem mã hóa đơn có tồn tại trong cơ sở dữ liệu hay không
            var billID = _context.HoaDons.FirstOrDefault(d => d.MaHoaDon == maHoaDon);

            if (billID == null)
            {
                return Json(new { success = false, message = "Mã hóa đơn không tồn tại." });
            }
            else
            {
                await _service.huyDonHang(maHoaDon);
                string maPTTT = await _service.getMaPTTT(maHoaDon);
                if (maPTTT.Equals("cod"))
                {
                    await _service.suaTrangThaiThanhToan(maHoaDon, -1);
                }
                else
                {
                    await _service.suaTrangThaiThanhToan(maHoaDon, 0);
                }

                await _service.TangHangTon(maHoaDon);

            }

            // Nếu mã hóa đơn hợp lệ, trả về mã hóa đơn tương ứng
            return Json(new { success = true, billID = billID });
            //return RedirectToAction("Detail", "Receipt", new { mahd = billID.MaHoaDon });
        }


    }
}
