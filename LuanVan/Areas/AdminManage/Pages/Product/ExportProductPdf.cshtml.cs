using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using LuanVan.Areas.Admin.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Product
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class ExportProductPdfModel : ProductPageModel
    {
        public ExportProductPdfModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public void OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var renderer = new HtmlToPdf();
            renderer.PrintOptions.MarginTop = 20;
            renderer.PrintOptions.MarginBottom = 20;
            renderer.PrintOptions.MarginLeft = 10;
            renderer.PrintOptions.MarginRight = 10;
            renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
            renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


            var listData = await GetListProduct();

            string htmlString = "<!DOCTYPE html>" +
                "<html>" +
                "<head><meta charset=\"UTF-8\">" +
                "<style>@page {size: A4;margin: 0;}body {font-family: Arial, sans-serif;font-size: 14px;margin: 0;padding: 20px;}" +
                "h1 {text-align: center;}table {border-collapse: collapse;width: 100%;}" +
                "th, td {border: 1px solid #ddd;padding: 8px;text-align: left;}" +
                "tr:nth-child(even) {background-color: #f2f2f2;}" +
                "@media print {body * {visibility: hidden;}#print-section, #print-section * {visibility: visible;}#print-section {position: absolute;left: 0;top: 0;}}</style></head>" +
                "<body>" +
                "<h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("DSProduct") + "</h1>" +
                "<div style=\"display: flex; flex-direction: row;\">" +
                "<p style=\"width: 50%; margin: 0;\"><b>" + _localization.Getkey("NgayInHD") + ": </b> " + DateTimeVN().ToString() + "</p>" +
                "<p style=\"width: 50%; margin: 0;\"><b>" + _localization.Getkey("SoLuong") + ": </b> " + listData.Count() + "</p>" +
                "</div><br><hr><br><div>" +
                "<table><thead>" +
                "<tr>" +
                "<th>" + _localization.Getkey("DSHDStt") + "</th>" +
                "<th>" + _localization.Getkey("DSProductMa") + "</th>" +
                "<th>" + _localization.Getkey("DSProductName") + "</th>" +
                "<th>" + _localization.Getkey("DSProductDVT") + "</th>" +
                "<th>" + _localization.Getkey("DSProductNSX") + "</th>" +
                "<th>" + _localization.Getkey("DSProductLSP") + "</th>" +
                "<th>" + _localization.Getkey("DSProductMS") + "</th>" +
                "<th>" + _localization.Getkey("DSProductGia") + "</th>" +
                "<th>" + _localization.Getkey("DSProductTT") + "</th>" +
                "</tr></thead>" +
                "<tbody>";

            int stt = 1;
            foreach (var item in listData)
            {
                htmlString += "<tr><td>" + stt + "</td>";
                htmlString += "<td>" + item.MaSanPham + "</td>";
                htmlString += "<td>" + item.TenSanPham + "</td>";
                htmlString += "<td>" + item.TenDVT + "</td>";
                htmlString += "<td>" + item.TenNSX + "</td>";
                htmlString += "<td>" + item.TenLoaiSP + "</td>";
                htmlString += "<td>" + item.TenMau + "</td>";
                htmlString += "<td>" + @String.Format("{0: ### ### ### ### VNĐ}", item.GiaBan) + "</td>";
                if (item.TrangThai == -1)
                {
                    htmlString += "<td> " + _localization.Getkey("KKD") + "</td></tr>";
                }
                else if (item.TrangThai == 0)
                {
                    htmlString += "<td> " + _localization.Getkey("HH") + "</td></tr>";
                }
                else if (item.TrangThai == 1)
                {
                    htmlString += "<td> " + _localization.Getkey("KM") + "</td></tr>";
                }
                else
                {
                    htmlString += "<td> " + _localization.Getkey("HOT") + "</td></tr>";
                }
                stt++;
            }

            htmlString += "</tbody></table></div></body></html>";

            string filename = "DSSP_" + DateTimeVN().Ticks + ".pdf";
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

            Console.WriteLine(filename);
            var pdf = renderer.RenderHtmlAsPdf(htmlString);

            pdf.SaveAs(filepath);

            // return file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/pdf", filename);
        }

        public async Task<List<ProductModel>> GetListProduct()
        {
            var result = (from a in _context.SanPhams
                          join b in _context.NhaSanXuats on a.MaNsx equals b.MaNsx into hdb
                          from b in hdb.DefaultIfEmpty()
                          join c in _context.LoaiSanPhams on a.MaLoaiSp equals c.MaLoaiSp into pttt
                          from c in pttt.DefaultIfEmpty()
                          join d in _context.MauSacs on a.MaMau equals d.MaMau into km
                          from d in km.DefaultIfEmpty()
                          select new ProductModel
                          {
                              MaSanPham = a.MaSanPham,
                              TenSanPham = a.TenSanPham,
                              TenDVT = a.TenDvt,
                              TenNSX = b.TenNsx,
                              TenLoaiSP = c.TenLoaiSp,
                              TenMau = d.TenMau,
                              GiaBan = a.GiaBan,
                              TrangThai = a.TrangThai,
                              MoTa = a.MoTa
                          }).ToListAsync();

            return await result;
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
