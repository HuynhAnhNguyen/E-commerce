using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Areas.Store.Models;
using LuanVan.Models;
using LuanVan.VNPays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using ZaloPay.Helper;
using ZaloPay.Helper.Crypto;
using System.Web;
using Microsoft.EntityFrameworkCore;
using LuanVan.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.IdentityModel.Tokens;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LuanVan.Services;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Html;

namespace LuanVan.Areas.Store.Controllers
{
    [Area("Store")]
    public class CheckoutController : Controller
    {
        private readonly Service _service;
        private readonly UserManager<KhachHang> _userManager;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly LanguageService _localization;


        public CheckoutController(
            Service service,
            UserManager<KhachHang> userManager,
            INotyfService notyf,
            ApplicationDbContext context,
            IEmailSender emailSender,
            LanguageService localization)
        {
            _service = service;
            _userManager = userManager;
            _notyf = notyf;
            _context = context;
            _emailSender = emailSender;
            _localization   = localization;
        }

        [HttpGet]
        [Authorize]
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

            return View(await _service.danhSachGioHang(2).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string listgh)
        {

            //            -- Trang thai - 1: San pham da xoa khoi don dat
            //            -- Trang thai 0: Chua thanh toan them vao gio hang
            //            -- Thang thai 1: Da thanh toan cho khach hang vang lai/ da dang ky
            //            -- Trang thai 2: Khach hang vang lai them vao gio hang nhung khong mua
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if (currentCulture == "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            var st = System.Text.Json.JsonSerializer.Deserialize<List<string>>(listgh);
            var list = new List<GioHang>();

            if (st?.Count() > 0 && st.First().Contains("GH"))
            {
                foreach (var s in st)
                {
                    var gh = await _service.getGioHang(s);
                    list.Add(gh);
                }
            }

            if (st?.Count() > 0 && !st.First().Contains("GH"))
            {
                string magh = await _service.themGioHang(st.First());
                list.Add(await _service.getGioHang(magh));
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
            ViewData["pay-method"] = await getPayMethod();

            if (maKH != null)
            {
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();
                HoaDonModel info = new HoaDonModel();
                var kh = await _service.getKH(maKH);
                info.Holot = kh.HoKhachHang;
                info.Ten = kh.TenKhachHang;
                info.SoDienThoai = kh.PhoneNumber;
                info.Email = kh.Email;
                ViewData["info"] = info;
            }
            else
            {
                ViewData["cart_items"] = new List<GioHang>();
                ViewData["info"] = null;
            }
            return View(list);
        }

        public async Task<List<ThanhToan>> getPayMethod()
        {
            var result = (from thanhtoan in _context.ThanhToans
                          orderby thanhtoan.TenPttt descending
                          select thanhtoan
                          ).ToListAsync();

            return await result;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Receipt(HoaDonModel model)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Console.WriteLine(currentCulture);
            if (currentCulture == "vi-VN")
                ViewData["language"] = _localization.Getkey("Vietnamese");
            else if (currentCulture == "en-US")
                ViewData["language"] = _localization.Getkey("English");
            else ViewData["language"] = "";

            var st = System.Text.Json.JsonSerializer.Deserialize<List<string>>(model.GioHangs);
            var list = new List<GioHang>();

            string maKH = null;
            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }

            ViewData["Loai"] = await _service.danhSachLoaiSP().ToListAsync();
            ViewData["path"] = "/images/product/";

            //string new_mahd;
            if (maKH != null)
            {
                // Neu co KH tu session lay danh sach gio hang co ma trang thai =0 
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();
                // Them hoa don voi maKH, ten PTTT
                //new_mahd = _service.themHoaDon(maKH, model.ThanhToan);
            }
            else
            {
                // Neu ko co KH tu session tao danh sach gio hang rong
                ViewData["cart_items"] = new List<GioHang>();
                // Them hoa don
                //new_mahd = _service.themHoaDon();
            }

            ViewData["info"] = model;


            return View(list);
        }


        //[HttpPost]
        //public async Task<String> CheckoutWallet(string listgh, string payment)
        //{
        //    String info = "";
        //    if (payment == "zalopay")
        //    {
        //        var st = System.Text.Json.JsonSerializer.Deserialize<List<string>>(listgh);
        //        List<GioHang> list_gh = new List<GioHang>();

        //        long? total = 0;
        //        if (st?.Count() > 0 && !st.First().Contains("GH"))
        //        {
        //            SanPham sp = _service.getSanPham(st.First());
        //            total = sp.GiaBan;
        //            info += sp.TenSanPham + ": " + sp.GiaBan + " x1";
        //        }
        //        if (st?.Count() > 0 && st.First().Contains("GH"))
        //        {
        //            foreach (var s in st)
        //            {
        //                var gh = _service.getGioHang(s);
        //                list_gh.Add(gh);
        //                SanPham sp = gh.SanPham;
        //                info += sp.TenSanPham + ": " + sp.GiaBan + " x" + gh.SoLuongDat + ", ";
        //                total += gh.SoLuongDat * gh.SanPham.GiaBan;
        //            }
        //        }

        //        string app_id = "2554";
        //        string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        //        string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";

        //        Random rnd = new Random();
        //        var embed_data = new { };
        //        var items = new[] { new { msg = "hello" } };
        //        //var items = list_gh;
        //        var param = new Dictionary<string, string>();
        //        var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

        //        param.Add("app_id", app_id);
        //        param.Add("app_user", "user123");
        //        param.Add("app_time", Utils.GetTimeStamp().ToString());
        //        param.Add("amount", total.ToString());
        //        param.Add("app_trans_id", DateTimeVN().ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
        //        param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
        //        param.Add("item", JsonConvert.SerializeObject(items));
        //        param.Add("description", info);
        //        param.Add("bank_code", "zalopayapp");

        //        var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
        //                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
        //        param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

        //        var result = await HttpHelper.PostFormAsync(create_order_url, param);
        //        return JsonConvert.SerializeObject(result);
        //    }
        //    //else if (payment == "vnpay")
        //    //{
        //    //    var st = System.Text.Json.JsonSerializer.Deserialize<List<string>>(listgh);
        //    //    List<GioHang> list_gh = new List<GioHang>();

        //    //    long? total = 0;
        //    //    if (st?.Count() > 0 && !st.First().Contains("GH"))
        //    //    {
        //    //        SanPham sp = _service.getSanPham(st.First());
        //    //        total = sp.GiaBan;
        //    //        info += sp.TenSanPham + ": " + sp.GiaBan + " x1";
        //    //    }
        //    //    if (st?.Count() > 0 && st.First().Contains("GH"))
        //    //    {
        //    //        foreach (var s in st)
        //    //        {
        //    //            var gh = _service.getGioHang(s);
        //    //            list_gh.Add(gh);
        //    //            SanPham sp = gh.SanPham;
        //    //            info += sp.TenSanPham + ": " + sp.GiaBan + " x" + gh.SoLuongDat + ", ";
        //    //            total += gh.SoLuongDat * gh.SanPham.GiaBan;
        //    //        }
        //    //    }

        //    //    string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        //    //    string returnUrl = "https://localhost:44393/Home/PaymentConfirm";
        //    //    string tmnCode = "8DS6XPGY";
        //    //    string hashSecret = "DTLRCGEEIAVLFRLXAYIJOWFFTVCUKHQJ";
        //    //    VNPayLib pay = new VNPayLib();

        //    //    pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
        //    //    pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
        //    //    pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
        //    //    pay.AddRequestData("vnp_Amount", "1000000"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
        //    //    pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
        //    //    pay.AddRequestData("vnp_CreateDate", DateTimeVN().ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
        //    //    pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
        //    //    pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
        //    //    pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
        //    //    pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
        //    //    pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
        //    //    pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
        //    //    pay.AddRequestData("vnp_TxnRef", DateTimeVN().Ticks.ToString()); //mã hóa đơn

        //    //    string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
        //    //    return JsonConvert.SerializeObject(paymentUrl);
        //    //}
        //    return JsonConvert.SerializeObject(new { payment = payment });
        //}
        public DateTime DateTimeVN()
        {
            DateTime utcTime = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Lấy thông tin về múi giờ của Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamZone); // Chuyển đổi giá trị DateTime từ múi giờ UTC sang múi giờ của Việt Nam

            return vietnamTime;
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string listgh, string payment, string? discountCode, long totalPrice)
        {
            Console.WriteLine(payment);

            HoaDonModel model = new HoaDonModel();
            model.ThanhToan = payment;

            string info = "";
            var st = System.Text.Json.JsonSerializer.Deserialize<List<string>>(listgh);

            //Console.WriteLine(listgh);
            //Console.WriteLine(st);

            List<GioHang> list_gh = new List<GioHang>();

            var list = new List<GioHang>();

            string maKH = null;
            if (User.Identity.IsAuthenticated)
            {
                KhachHang user = await _userManager.FindByNameAsync(User.Identity.Name);
                maKH = user.Id;
                Console.WriteLine(maKH);
            }

            // Co dang nhap
            if (st?.Count() > 0 && st.First().Contains("GH"))
            {
                foreach (var s in st)
                {
                    var gh = await _service.getGioHang(s);
                    //-- Trang thai -1: San pham da xoa khoi don dat
                    //-- Trang thai 0: Chua thanh toan them vao gio hang
                    //-- Thang thai 1: Da thanh toan cho khach hang vang lai/ da dang ky
                    //-- Trang thai 2: Khach hang vang lai them vao gio hang nhung khong mua
                    if (gh.TrangThai != 1)
                        list.Add(gh);
                }
            }

            // Khong dang nhap
            if (st?.Count() > 0 && !st.First().Contains("GH"))
            {
                string magh = await _service.themGioHang(st.First());
                list.Add(await _service.getGioHang(magh));
            }

            // Neu danh sach gio hang =0 quay ve trang chu
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Loai"] = await _context.LoaiSanPhams.ToListAsync();
            ViewData["path"] = "/images/product/";

            string new_mahd;
            if (maKH != null)
            {
                // Neu co KH tu session lay danh sach gio hang co ma trang thai =0 
                ViewData["cart_items"] = await _service.danhSachGioHang(0, maKH).ToListAsync();

                // Them hoa don voi maKH, ten PTTT
                HoaDon hoaDon = new HoaDon();
                hoaDon.MaHoaDon = "" + DateTimeVN().Ticks.ToString();
                hoaDon.KhachHangId = maKH;
                hoaDon.MaPttt = payment;
                hoaDon.NgayXuatHd = DateTimeVN();
                if (!discountCode.IsNullOrEmpty())
                {
                    hoaDon.MaKm = discountCode;

                    KhuyenMai khuyenMai = await _context.KhuyenMais.Where(x => x.MaKm == discountCode).FirstOrDefaultAsync();
                    _context.Update(khuyenMai);
                    khuyenMai.SoLuongConLai -= 1;
                    await _context.SaveChangesAsync();

                }
                hoaDon.TrangThaiDonHang = 0;
                await _context.HoaDons.AddAsync(hoaDon);
                await _context.SaveChangesAsync();
                new_mahd = hoaDon.MaHoaDon;
            }
            else
            {
                // Neu ko co KH tu session tao danh sach gio hang rong
                ViewData["cart_items"] = new List<GioHang>();
                // Them hoa don
                HoaDon hoaDon = new HoaDon();
                hoaDon.MaHoaDon = "" + DateTimeVN().Ticks.ToString();
                hoaDon.KhachHangId = null;
                hoaDon.MaPttt = payment;
                hoaDon.TrangThaiDonHang = 0;
                hoaDon.NgayXuatHd = DateTimeVN();
                if (!discountCode.IsNullOrEmpty())
                {
                    hoaDon.MaKm = discountCode;

                    KhuyenMai khuyenMai = await _context.KhuyenMais.Where(x => x.MaKm == discountCode).FirstOrDefaultAsync();
                    _context.Update(khuyenMai);
                    khuyenMai.SoLuongConLai -= 1;
                    await _context.SaveChangesAsync();
                }
                await _context.HoaDons.AddAsync(hoaDon);
                await _context.SaveChangesAsync();
                new_mahd = hoaDon.MaHoaDon;
            }

            // Them chi tiet hoa don cho gio hang
            foreach (var gh in list)
            {
                await _service.themChiTietHD(new_mahd, gh.MaGioHang);
            }

            model.MaHoaDon = new_mahd;

            //long? total = 0;
            if (st?.Count() > 0 && !st.First().Contains("GH"))
            {
                SanPham sp = await _service.getSanPham(st.First());
                //total = sp.GiaBan;
                info += sp.TenSanPham + ": " + sp.GiaBan + " x1";
            }

            if (st?.Count() > 0 && st.First().Contains("GH"))
            {
                foreach (var s in st)
                {
                    var gh = await _service.getGioHang(s);
                    list_gh.Add(gh);
                    SanPham sp = gh.SanPham;
                    info += sp.TenSanPham + ": " + sp.GiaBan + " x" + gh.SoLuongDat + ", ";
                    //total += gh.SoLuongDat * gh.SanPham.GiaBan;
                }
            }

            var hoaDonUpdate = await _context.HoaDons.Where(x => x.MaHoaDon == new_mahd).FirstOrDefaultAsync();
            _context.Update(hoaDonUpdate);
            hoaDonUpdate.TongGiaTri = totalPrice;
            await _context.SaveChangesAsync();


            if (payment.Equals("cod"))
            {

                await _service.suaTrangThaiThanhToan(new_mahd, 2);
                _notyf.Success(_localization.Getkey("Pay_Success_Bill") + " " + new_mahd);
                await _service.GiamHangTon(new_mahd);
                await sendMail(new_mahd.ToString());
                //string returnUrl = "https://test-ishopping.azurewebsites.net/Store/Receipt/Detail?mahd=" + new_mahd; // host azure 1
                //string returnUrl = "https://ct554-ishopping.azurewebsites.net/Store/Receipt/Detail?mahd=" + new_mahd; // host azure 2
                string returnUrl = "https://localhost:7279/Store/Receipt/Detail?mahd=" + new_mahd; // localhost
                //string returnUrl = "https://c697-2405-4800-5f24-300-f559-dfdd-5c04-b970.ngrok-free.app/Store/Receipt/Detail?mahd=" + new_mahd; // ngrok
                //return RedirectToAction("Detail", "Receipt", new { mahd = new_mahd });
                return Json(returnUrl);

            }
            else
            {
                totalPrice = totalPrice * 100;
                string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                //string returnUrl = "https://test-ishopping.azurewebsites.net/Store/Checkout/PaymentConfirm"; // host azure 1
                //string returnUrl = "https://ct554-ishopping.azurewebsites.net/Store/Checkout/PaymentConfirm"; // host azure 2
                string returnUrl = "https://localhost:7279/Store/Checkout/PaymentConfirm"; // localhost
                //string returnUrl = "https://c697-2405-4800-5f24-300-f559-dfdd-5c04-b970.ngrok-free.app/Store/Checkout/PaymentConfirm"; // ngrok
                string tmnCode = "8DS6XPGY";
                string hashSecret = "DTLRCGEEIAVLFRLXAYIJOWFFTVCUKHQJ";
                VNPayLib pay = new VNPayLib();

                pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
                pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                pay.AddRequestData("vnp_Amount", totalPrice.ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
                pay.AddRequestData("vnp_CreateDate", DateTimeVN().ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
                pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                pay.AddRequestData("vnp_OrderInfo", info); //Thông tin mô tả nội dung thanh toán
                pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                pay.AddRequestData("vnp_TxnRef", model.MaHoaDon); //mã hóa đơn

                string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
                //Console.WriteLine(paymentUrl);
                //return Redirect(paymentUrl);
                return Json(paymentUrl);
            }
        }


        [HttpGet]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.Query.Count > 0)
            {
                string hashSecret = "DTLRCGEEIAVLFRLXAYIJOWFFTVCUKHQJ"; //Chuỗi bí mật
                var vnpayData = Request.Query;
                VNPayLib pay = new VNPayLib();

                foreach (var keyValuePair in vnpayData)
                {
                    if (!string.IsNullOrEmpty(keyValuePair.Key) && keyValuePair.Key.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.Query["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {

                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        await _service.suaTrangThaiThanhToan(orderId.ToString(), 1);
                        _notyf.Success(_localization.Getkey("Pay_Success_Bill") + " " + orderId);
                        // Cập nhật sl tồn ở đây
                        //List<SanPham> sanPhams = new List<SanPham>();
                        //List<GioHang> gioHangs = new List<GioHang>();
                        //List<ChiTietHd> chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == orderId.ToString()).ToListAsync();

                        //foreach (var chiTietHoaDon in chiTietHoaDons)
                        //{
                        //	GioHang gioHang = await _context.GioHangs.Where(x => x.MaGioHang == chiTietHoaDon.MaGioHang).FirstOrDefaultAsync();
                        //	SanPham sanPham = await _context.SanPhams.Where(x => x.MaSanPham == gioHang.MaSanPham).FirstOrDefaultAsync();

                        //	sanPhams.Add(sanPham);
                        //	gioHangs.Add(gioHang);
                        //}

                        //ViewData["sanPhams"] = sanPhams;
                        //ViewData["gioHangs"] = gioHangs;

                        //var sanPhamss = ViewData["sanPhams"] as IEnumerable<SanPham>;
                        //var gioHangss = ViewData["gioHangs"] as IEnumerable<GioHang>;

                        //for (int i = 0; i < chiTietHoaDons.Count(); i++)
                        //{
                        //	var sanPham = sanPhamss.ElementAt(i);
                        //	var gioHang = gioHangss.ElementAt(i);
                        //	sanPham.SoLuongTon -= gioHang.SoLuongDat;
                        //	_context.Update(sanPham);
                        //	Console.WriteLine(sanPham.SoLuongTon);
                        //}
                        //await _context.SaveChangesAsync();
                        await _service.GiamHangTon(orderId.ToString());

                        await sendMail(orderId.ToString());
                        return RedirectToAction("Detail", "Receipt", new { mahd = orderId });
                        //Store / Receipt / Detail ? mahd = 638140401614983559
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        _notyf.Error(_localization.Getkey("Pay_Error_Bill") + " " + orderId);
                        await _service.suaTrangThaiThanhToan(orderId.ToString(), -1);
                        await _service.suaTrangThaiDonHang(orderId.ToString(), -1);
						await sendMail(orderId.ToString());
                        //return RedirectToAction("Index", "Home");
                        return RedirectToAction("Detail", "Receipt", new { mahd = orderId });

                    }
                }
                else
                {
                    ViewBag.Message = _localization.Getkey("Pay_Error_Bill") + " " + orderId;
                    await _service.suaTrangThaiThanhToan(orderId.ToString(), -1);
                    await _service.suaTrangThaiDonHang(orderId.ToString(), -1);
                    _notyf.Error(_localization.Getkey("Pay_Error_Bill") + " " + orderId);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //[Area("Store")]
        //[HttpPost]
        //public async Task<ActionResult> CheckoutWallet(string listgh, string payment)
        //{
        //    String info = "";
        //    if (payment == "vnpay")
        //    {
        //        var st = System.Text.Json.JsonSerializer.Deserialize<List<string>>(listgh);
        //        List<GioHang> list_gh = new List<GioHang>();

        //        long? total = 0;
        //        if (st?.Count() > 0 && !st.First().Contains("GH"))
        //        {
        //            SanPham sp = _service.getSanPham(st.First());
        //            total = sp.GiaBan;
        //            info += sp.TenSanPham + ": " + sp.GiaBan + " x1";
        //        }
        //        if (st?.Count() > 0 && st.First().Contains("GH"))
        //        {
        //            foreach (var s in st)
        //            {
        //                var gh = _service.getGioHang(s);
        //                list_gh.Add(gh);
        //                SanPham sp = gh.SanPham;
        //                info += sp.TenSanPham + ": " + sp.GiaBan + " x" + gh.SoLuongDat + ", ";
        //                total += gh.SoLuongDat * gh.SanPham.GiaBan;
        //            }
        //        }

        //        string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        //        string returnUrl = "https://localhost:44393/Home/PaymentConfirm";
        //        string tmnCode = "8DS6XPGY";
        //        string hashSecret = "DTLRCGEEIAVLFRLXAYIJOWFFTVCUKHQJ";
        //        VNPayLib pay = new VNPayLib();

        //        pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
        //        pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
        //        pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
        //        pay.AddRequestData("vnp_Amount", "1000000"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
        //        pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
        //        pay.AddRequestData("vnp_CreateDate", DateTimeVN().ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
        //        pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
        //        pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
        //        pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
        //        pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
        //        pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
        //        pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
        //        pay.AddRequestData("vnp_TxnRef", DateTimeVN().Ticks.ToString()); //mã hóa đơn

        //        string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
        //        return Redirect(paymentUrl);
        //    }
        //    return Redirect()
        //}

        //[HttpPost]
        //public IActionResult CheckDiscount(string discountCode)
        //{
        //    List<KhuyenMai> discounts = _service.getDanhSachGiamGia();
        //    foreach (var discount in discounts)
        //    {
        //        if (discount.MaKm == discountCode)
        //        {
        //            // Trả về giá trị khuyến mãi nếu mã giảm giá đúng
        //            return Json(new { discountAmount = 100000 }); // Giả sử khuyến mãi là 100.000 đồng
        //        }
        //    }
        //    return Json(new { error = "Mã giảm giá không đúng!" });

        //}

        [HttpPost]
        public IActionResult CheckDiscount(string discountCode)
        {
            // Kiểm tra xem mã giảm giá có tồn tại trong cơ sở dữ liệu hay không
            var discount = _context.KhuyenMais.FirstOrDefault(d => d.MaKm == discountCode);
           
            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại." });
            }
            //Console.WriteLine(discount.NgayKetThuc);
            // Kiểm tra xem mã giảm giá có còn hiệu lực hay không
            if (discount.NgayKetThuc.AddDays(1) < DateTimeVN())
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn." });
            }

            // Kiểm tra xem mã giảm giá còn số lượng hay không
            if (discount.SoLuongConLai == 0)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết số lượng." });
            }

            // Nếu mã giảm giá hợp lệ, trả về giá trị khuyến mãi tương ứng
            return Json(new { success = true, discount = discount.GiaTriKm });
        }

        [HttpPost]
        public ActionResult GetEmailAddress(string email)
        {
            HttpContext.Session.SetString("Email", email);

            return Json(new { success = true, message = "Email đã được lưu." });
        }

        public async Task sendMail(string mahd)
        {
            string diaChiEmail = HttpContext.Session.GetString("Email");

            _notyf.Information(_localization.Getkey("Send_email_bill")+ " " + diaChiEmail);

            HoaDon hoaDon = await _context.HoaDons.Where(x => x.MaHoaDon == mahd).FirstOrDefaultAsync();

            List<ChiTietHd> chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == mahd).ToListAsync();

            string tenPhuongThucThanhToan;
            string khuyenMai;
            string trangThaiThanhToan;
            string trangThaiDonHang;

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

            tenPhuongThucThanhToan = await(from a in _context.HoaDons join b in _context.ThanhToans on a.MaPttt equals b.MaPttt where a.MaHoaDon == mahd select b.TenPttt).FirstOrDefaultAsync();


            if (!hoaDon.MaKm.IsNullOrEmpty())
            {
                khuyenMai = await(from a in _context.KhuyenMais
                                  join b in _context.HoaDons on a.MaKm equals b.MaKm
                                  where b.MaHoaDon == mahd
                                  select a.TenKhuyenMai).FirstOrDefaultAsync();
            }
            else khuyenMai = _localization.Getkey("Not_apply");

            if (hoaDon.TrangThaiThanhToan == -1)
            {
                trangThaiThanhToan = _localization.Getkey("Pay_error");
            }else if (hoaDon.TrangThaiThanhToan == 0)
            {
                trangThaiThanhToan = _localization.Getkey("Waiting_for_refund");
            }
            else if (hoaDon.TrangThaiThanhToan == 1)
            {
                trangThaiThanhToan = _localization.Getkey("Pay_success");
            }
            else
            {
                trangThaiThanhToan = _localization.Getkey("ChoThanhToan");
            }

            if (hoaDon.TrangThaiDonHang == -1)
            {
                trangThaiDonHang = _localization.Getkey("Cancel_bill");
            }
            else if (hoaDon.TrangThaiDonHang == 0)
            {
                trangThaiDonHang = _localization.Getkey("Waiting_for_delivery");
            }
            else if (hoaDon.TrangThaiDonHang == 1)
            {
                trangThaiDonHang = _localization.Getkey("Delivery_in_progress");
            }
            else
            {
                trangThaiDonHang = _localization.Getkey("Delivery_successful");
            }

            string htmlString = "<head><meta charset=\"UTF-8\">" +
                "<style>@page {size: A4;margin: 0;}" +
                "body {font-family: Arial, sans-serif;" +
                "font-size: 14px;margin: 0;padding: 20px;}" +
                "h1 {text-align: center;}" +
                "table {border-collapse: collapse;width: 100%;}" +
                "th, td {border: 1px solid #ddd;padding: 8px;text-align: left;}" +
                "tr:nth-child(even) {background-color: #f2f2f2;}" +
                "@media print {body * {  visibility: hidden;}#print-section, " +
                "#print-section * {  visibility: visible;}" +
                "#print-section {  position: absolute;  left: 0;  top: 0;}}" +
                "</style></head><body>" +
                "<div style=\"display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center;\">" +
                "<div style=\"display: flex; align-items: center;\">    " +
                "<h1 style=\"margin-left: 10px;\">" + _localization.Getkey("ISHOPPING") + "</h1></div>" +
                "<p>" + _localization.Getkey("TenDiaChi") + ": " + _localization.Getkey("DiaChiShop") + "</p>" +
                "<p>" + _localization.Getkey("TenSDT") + ": " + _localization.Getkey("SoDienThoai") + "</p>" +
                "<p>" + _localization.Getkey("TenEmail") + ": " + _localization.Getkey("DiaChiEmail") + "</p></div>" +
                "<hr>" +
                "<h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("ThongTinHD") + "</h1>" +
                "<div style=\"display:flex;\"><table style=\"margin-right: 50px;\">" +
                "<tbody>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Code_bill") + "</b></td>" +
                "<td>" + mahd + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Invoice_issue_date") + "</b></td>" +
                "<td>" + hoaDon.NgayXuatHd + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("NgayInHD") + "</b></td>" +
                "<td>" + DateTimeVN().ToString() + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Total_value") + "</b></td>" +
                "<td>" + System.@String.Format("{0: ### ### ### ### VNĐ}", hoaDon.TongGiaTri) + "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "<table>" +
                "<tbody>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Discount") + "</b></td>" +
                "<td>" + khuyenMai + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Payments") + "</b></td>" +
                "<td>" + tenPhuongThucThanhToan + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Payment_status") + "</b></td>" +
                "<td>" + trangThaiThanhToan + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("Order_Status") + "</b></td>" +
                "<td>" + trangThaiDonHang + "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "</div>" +
                "<hr>" +
                "<h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("TTSPHD") + "</h1>" +
                "<div>" +
                "<table>" +
                "<thead>" +
                "<tr>" +
                "<th>" + _localization.Getkey("STT") + "</th>" +
                "<th>" + _localization.Getkey("Product_name") + "</th>" +
                "<th>" + _localization.Getkey("Product_Type") + "</th>" +
                "<th>" + _localization.Getkey("Quantity") + "</th>" +
                "<th>" + _localization.Getkey("Price") + "</th>" +
                "</tr>" +
                "</thead><tbody>";

            var sanPhamss = ViewData["sanPhams"] as IEnumerable<SanPham>;
            var gioHangss = ViewData["gioHangs"] as IEnumerable<GioHang>;
            var loaiSanPhamss = ViewData["loaiSanPhams"] as IEnumerable<LoaiSanPham>;
            int stt = 1;

            for (int i = 0; i < chiTietHoaDons.Count(); i++)
            {
                var sanPham = sanPhamss.ElementAt(i);
                var gioHang = gioHangss.ElementAt(i);
                var loaiSanPham = loaiSanPhamss.ElementAt(i);

                htmlString += "<tr><td>" + stt + "</td>";
                htmlString += "<td>" + sanPham.TenSanPham + "</td>";
                htmlString += "<td>" + loaiSanPham.TenLoaiSp + "</td>";
                htmlString += "<td>" + gioHang.SoLuongDat + "</td>";
                htmlString += "<td>" + System.@String.Format("{0: ### ### ### ### VNĐ}", sanPham.GiaBan) + "</td></tr>";
                stt++;
            }

            htmlString += "</tbody></table></div></body>";


            await _emailSender.SendEmailAsync(diaChiEmail, _localization.Getkey("Invoice_information"), htmlString);

        }
    }
}
