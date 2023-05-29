﻿using LuanVan.Areas.Admin.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuanVan.Areas.AdminManage.Pages.Home
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class BillSuccessOrFailPdfModel : HomePageModel
    {
        public BillSuccessOrFailPdfModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, LanguageService localization) : base(context, webHostEnvironment, localization)
        {
        }

        public void OnGet()
        {
        }


        public IActionResult OnPost()
        {
            var renderer = new HtmlToPdf();
            renderer.PrintOptions.MarginTop = 20;
            renderer.PrintOptions.MarginBottom = 20;
            renderer.PrintOptions.MarginLeft = 10;
            renderer.PrintOptions.MarginRight = 10;
            renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
            renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

            var listData = GetListRevenueByMonth();

            string htmlString = "<!DOCTYPE html>" +
                "<html>" +
                "<head><meta charset=\"UTF-8\">" +
                "<style>@page {size: A4;margin: 0;}body {font-family: Arial, sans-serif;font-size: 14px;margin: 0;padding: 20px;}" +
                "h1 {text-align: center;}table {border-collapse: collapse;width: 100%;}" +
                "th, td {border: 1px solid #ddd;padding: 8px;text-align: left;}" +
                "tr:nth-child(even) {background-color: #f2f2f2;}" +
                "@media print {body * {visibility: hidden;}#print-section, #print-section * {visibility: visible;}#print-section {position: absolute;left: 0;top: 0;}}</style></head>" +
                "<body>" +
                "<h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("DSBillSuccessOrFail") + "</h1>" +
                "<div style=\"display: flex; flex-direction: row;\">" +
                "<p style=\"width: 100%; margin: 0;\"><b>" + _localization.Getkey("NgayInHD") + ": </b> " + DateTimeVN().ToString() + "</p>" +
                "</div><br><hr><br><div>" +
                "<table><thead>" +
                "<tr>" +
                "<th>" + _localization.Getkey("DSHDStt") + "</th>" +
                "<th>" + _localization.Getkey("NgayThangNam") + "</th>" +
                "<th>" + _localization.Getkey("SoLuongThanhCong") + "</th>" +
                "<th>" + _localization.Getkey("SoLuongThatBai") + "</th>" +
                "<th>" + _localization.Getkey("TongHoaDon") + "</th>" +
                "</tr></thead>" +
                "<tbody>";

            int stt = 1;
            foreach (var item in listData)
            {
                htmlString += "<tr><td>" + stt + "</td>";
                htmlString += "<td>" + item.NgayThangNam + "</td>";
                htmlString += "<td>" + item.SoLuongThanhCong + "</td>";
                htmlString += "<td>" + item.SoLuongThatBai + "</td>";
                htmlString += "<td>" + item.TongSoLuongHD + "</td></tr>";
                stt++;
            }

            htmlString += "</tbody></table></div></body></html>";

            string filename = "BillSuccessOrFail_" + DateTimeVN().Ticks + ".pdf";
            string filepath = Path.Combine(_webHostEnvironment.ContentRootPath, "Areas", "Admin", "Resource", "ExportPdf", filename);

            Console.WriteLine(filename);
            var pdf = renderer.RenderHtmlAsPdf(htmlString);

            pdf.SaveAs(filepath);
            // return file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/pdf", filename);

        }

        public List<BillSuccessOrFailModel> GetListRevenueByMonth()
        {
            var result = (from h in _context.HoaDons
                          select h)
            .AsEnumerable()
            .GroupBy(h => new {
                NgayThangNam = h.NgayXuatHd.Date
            })
            .Select(g => new BillSuccessOrFailModel
            {
                NgayThangNam = g.Key.NgayThangNam,
                SoLuongThanhCong = g.Count(x => x.TrangThaiThanhToan == 1),
                SoLuongThatBai = g.Count(x => x.TrangThaiThanhToan != 1),
                TongSoLuongHD = g.Count()
            })
            .ToList();

            return result;
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
