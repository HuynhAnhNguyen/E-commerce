using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using LuanVan.Areas.Admin.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Bill
{
    public class PrintBillModel : BillPageModel
    {
        public PrintBillModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string billid)
        {
            var renderer = new HtmlToPdf();
            renderer.PrintOptions.MarginTop = 20;
            renderer.PrintOptions.MarginBottom = 20;
            renderer.PrintOptions.MarginLeft = 10;
            renderer.PrintOptions.MarginRight = 10;
            renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
            renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

            HoaDon hoaDon;
            List<ChiTietHd> chiTietHoaDons;
            string tenPhuongThucThanhToan;
            string khuyenMai;
            string trangThaiThanhToan;
            string trangThaiDonHang;

            hoaDon = await _context.HoaDons.Where(x => x.MaHoaDon == billid).FirstOrDefaultAsync();

            switch (hoaDon.TrangThaiThanhToan)
            {
                case -1:
					trangThaiThanhToan = "" + _localization.Getkey("Pay_error");
					break;
				case 0:
					trangThaiThanhToan = "" + _localization.Getkey("Waiting_for_refund");
					break;
				case 1:
					trangThaiThanhToan = "" + _localization.Getkey("Pay_success");
					break;
                default:
					trangThaiThanhToan = "" + _localization.Getkey("ChoThanhToan");
					break;
			}

            switch (hoaDon.TrangThaiDonHang)
            {
				case -1:
					trangThaiDonHang = "" + _localization.Getkey("Cancel_bill");
					break;
				case 0:
					trangThaiDonHang = "" + _localization.Getkey("Waiting_for_delivery");
					break;
				case 1:
					trangThaiDonHang = "" + _localization.Getkey("Delivery_in_progress");
					break;
                default:
					trangThaiDonHang = "" + _localization.Getkey("Delivery_successful");
					break;
			}

            chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == billid).ToListAsync();

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

            tenPhuongThucThanhToan = await (from a in _context.HoaDons join b in _context.ThanhToans on a.MaPttt equals b.MaPttt where a.MaHoaDon == billid select b.TenPttt).FirstOrDefaultAsync();

            if (!hoaDon.MaKm.IsNullOrEmpty())
            {
                khuyenMai = await (from a in _context.KhuyenMais
                                   join b in _context.HoaDons on a.MaKm equals b.MaKm
                                   where b.MaHoaDon == billid
                                   select a.TenKhuyenMai).FirstOrDefaultAsync();
            }
            else khuyenMai = "" + _localization.Getkey("KAD");

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
                "<td><b>" + _localization.Getkey("DSHDMaHD") + "</b></td>" +
                "<td>" + billid + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("DSHDNgayXuat") + "</b></td>" +
                "<td>" + hoaDon.NgayXuatHd + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("NgayInHD") + "</b></td>" +
                "<td>" + DateTimeVN().ToString() + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("DSHDTong") + "</b></td>" +
                "<td>" + @String.Format("{0: ### ### ### ### VNĐ}", hoaDon.TongGiaTri) + "</td>" +
                "</tr>" +
                "</tbody>" +
                "</table>" +
                "<table>" +
                "<tbody>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("DSHDCTKMPDF") + "</b></td>" +
                "<td>" + khuyenMai + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("DSHDPTTTPDF") + "</b></td>" +
                "<td>" + tenPhuongThucThanhToan + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("DSHDTTTTPDF") + "</b></td>" +
                "<td>" + trangThaiThanhToan + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td><b>" + _localization.Getkey("DSHDTTDHPDF") + "</b></td>" +
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
                htmlString += "<td>" + @String.Format("{0: ### ### ### ### VNĐ}", sanPham.GiaBan) + "</td></tr>";
                stt++;
            }

            htmlString += "</tbody></table></div></body>";

            string filename = "HD_" + billid + "_" + DateTimeVN().Ticks + ".pdf";
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

            //Console.WriteLine(filename);

            var pdf = renderer.RenderHtmlAsPdf(htmlString);

            pdf.SaveAs(filepath);

            // return file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

            return File(fileBytes, "application/pdf", filename);
        }

        public DateTime DateTimeVN()
        {
            DateTime utcTime = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Lấy thông tin về múi giờ của Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamZone); // Chuyển đổi giá trị DateTime từ múi giờ UTC sang múi giờ của Việt Nam

            return vietnamTime;
        }

    }
}
