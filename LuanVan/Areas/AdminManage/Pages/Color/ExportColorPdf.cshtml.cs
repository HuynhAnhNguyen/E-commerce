using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Services;
using Microsoft.AspNetCore.Hosting;
using LuanVan.Areas.Admin.Models;

namespace LuanVan.Areas.AdminManage.Pages.Color
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class ExportColorPdfModel : ColorPageModel
    {
        public ExportColorPdfModel(ApplicationDbContext context, INotyfService notyf, ILogger<ColorPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public void OnGet()
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


            var listData = await GetListColor();

            string htmlString = "<!DOCTYPE html>" +
                "<html>" +
                "<head><meta charset=\"UTF-8\">" +
                "<style>@page {size: A4;margin: 0;}body {font-family: Arial, sans-serif;font-size: 14px;margin: 0;padding: 20px;}" +
                "h1 {text-align: center;}table {border-collapse: collapse;width: 100%;}" +
                "th, td {border: 1px solid #ddd;padding: 8px;text-align: left;}" +
                "tr:nth-child(even) {background-color: #f2f2f2;}" +
                "@media print {body * {visibility: hidden;}#print-section, #print-section * {visibility: visible;}#print-section {position: absolute;left: 0;top: 0;}}</style></head>" +
                "<body>" +
                "<h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("DSMS") + "</h1>" +
                "<div style=\"display: flex; flex-direction: row;\">" +
                "<p style=\"width: 50%; margin: 0;\"><b>" + _localization.Getkey("NgayInHD") + ": </b> " + DateTimeVN().ToString() + "</p>" +
                "<p style=\"width: 50%; margin: 0;\"><b>" + _localization.Getkey("SoLuong") + ": </b> " + listData.Count() + "</p>" +
                "</div><br><hr><br><div>" +
                "<table><thead>" +
                "<tr>" +
                "<th>" + _localization.Getkey("DSHDStt") + "</th>" +
                "<th>" + _localization.Getkey("DSMSMaMau") + "</th>" +
                "<th>" + _localization.Getkey("DSMSTenMau") + "</th>" +
                "</tr></thead>" +
                "<tbody>";

            int stt = 1;
            foreach (var item in listData)
            {
                htmlString += "<tr><td>" + stt + "</td>";
                htmlString += "<td>" + item.MaMau + "</td>";
                htmlString += "<td>" + item.TenMau + "</td></tr>";
                stt++;
            }

            htmlString += "</tbody></table></div></body></html>";


            string filename = "DSMS_" + DateTimeVN().Ticks + ".pdf";
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

            Console.WriteLine(filename);
            var pdf = renderer.RenderHtmlAsPdf(htmlString);

            pdf.SaveAs(filepath);

            // return file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/pdf", filename);

        }
        public async Task<List<ColorModel>> GetListColor()
        {
            var result = (from a in _context.MauSacs
                          select new ColorModel
                          {
                              MaMau = a.MaMau,
                              TenMau = a.TenMau
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
