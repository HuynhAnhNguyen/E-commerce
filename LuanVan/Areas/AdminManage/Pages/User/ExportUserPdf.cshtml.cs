using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using LuanVan.Areas.Admin.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class ExportUserPdfModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;
        private readonly ApplicationDbContext _context;

        public ExportUserPdfModel(
            UserManager<KhachHang> userManager,
            SignInManager<KhachHang> signInManager,
            INotyfService notyf,
            LanguageService localization,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf = notyf;
            _localization = localization;
            _context = context;
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


            var listData = await GetListUser();

            string htmlString = "<!DOCTYPE html>" +
                "<html>" +
                "<head><meta charset=\"UTF-8\">" +
                "<style>@page {size: A4;margin: 0;}body {font-family: Arial, sans-serif;font-size: 14px;margin: 0;padding: 20px;}" +
                "h1 {text-align: center;}table {border-collapse: collapse;width: 100%;}" +
                "th, td {border: 1px solid #ddd;padding: 8px;text-align: left;}" +
                "tr:nth-child(even) {background-color: #f2f2f2;}" +
                "@media print {body * {visibility: hidden;}#print-section, #print-section * {visibility: visible;}#print-section {position: absolute;left: 0;top: 0;}}</style></head>" +
                "<body>" +
                "<h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("DSUser") + "</h1>" +
                "<div style=\"display: flex; flex-direction: row;\">" +
                "<p style=\"width: 50%; margin: 0;\"><b>" + _localization.Getkey("NgayInHD") + ": </b> " + DateTimeVN().ToString() + "</p>" +
                "<p style=\"width: 50%; margin: 0;\"><b>" + _localization.Getkey("SoLuong") + ": </b> " + listData.Count() + "</p>" +
                "</div><br><hr><br><div>" +
                "<table><thead>" +
                "<tr>" +
                "<th>" + _localization.Getkey("DSHDStt") + "</th>" +
                "<th>" + _localization.Getkey("UserID") + "</th>" +
                "<th>" + _localization.Getkey("UserFullName") + "</th>" +
                "<th>" + _localization.Getkey("UserDateofbirth") + "</th>" +
                "<th>" + _localization.Getkey("UserGender") + "</th>" +
                "<th>" + _localization.Getkey("UserUsername") + "</th>" +
                "<th>" + _localization.Getkey("UserEmail") + "</th>" +
                "</tr></thead>" +
                "<tbody>";

            int stt = 1;
            foreach (var item in listData)
            {
                htmlString += "<tr><td>" + stt + "</td>";
                htmlString += "<td>" + item.ID + "</td>";
                htmlString += "<td>" + item.HoKhachHang + " " + item.TenKhachHang + "</td>";
                htmlString += "<td>" + item.NgaySinh + "</td>";
                htmlString += "<td>" + item.GioiTinh + "</td>";
                htmlString += "<td>" + item.UserName + "</td>";
                htmlString += "<td>" + item.Email + "</td></tr>";
                stt++;
            }

            htmlString += "</tbody></table></div></body></html>";

            string filename = "DSUser_" + DateTimeVN().Ticks + ".pdf";
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

            Console.WriteLine(filename);
            var pdf = renderer.RenderHtmlAsPdf(htmlString);

            pdf.SaveAs(filepath);
            // return file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/pdf", filename);
        }

        public async Task<List<UserModel>> GetListUser()
        {
            var result = (from a in _context.KhachHangs
                          select new UserModel
                          {
                              ID = a.Id,
                              HoKhachHang = a.HoKhachHang,
                              TenKhachHang = a.TenKhachHang,
                              NgaySinh = a.NgaySinh,
                              GioiTinh = a.GioiTinh,
                              DiaChi = a.DiaChi,
                              UserName = a.UserName,
                              Email = a.Email,
                              PhoneNumber = a.PhoneNumber
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
