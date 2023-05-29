using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using LuanVan.Areas.Admin.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Bill
{
    [Authorize(Roles = "Admin, Test, Editor")]

    public class ExportHoaDonPdfFromToModel : BillPageModel
    {
        public ExportHoaDonPdfFromToModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public DateTime NgayBatDau { get; set; }
            public DateTime NgayKetThuc { get; set; }
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

            if (Input.NgayKetThuc < Input.NgayBatDau)
            {
                _notyf.Error("Ngày bắt đầu không thể trễ hơn ngày kết thúc", 3);

                return Page();
            }
            else
            {
                var listData = await GetListBillFromTo(Input.NgayBatDau, Input.NgayKetThuc);


                string htmlString = "<head><meta charset=\"UTF-8\"><style>@page {size: A4;margin: 0;}body {font-family: Arial, sans-serif;font-size: 14px;margin: 0;padding: 20px;}" +
                     "h1 {text-align: center;}table {border-collapse: collapse;width: 100%;}th, td {border: 1px solid #ddd;padding: 8px;text-align: left;}tr:nth-child(even) {" +
                     "background-color: #f2f2f2;}@media print {body * {visibility: hidden;}#print-section, #print-section * {visibility: visible;}#print-section {position: absolute;left: 0;top: 0;}}</style></head>" +
                     "<body><h1 style=\"text-align: center;text-transform: uppercase;font-weight: bold;\">" + _localization.Getkey("DSHDTitle") + "</h1>" +
                     "<div style=\"display: flex; flex-direction: row;\">" +
                     "<p style=\"width: 33%; margin: 0;\"><b>" + _localization.Getkey("NgayInHD") + ": </b> " + DateTimeVN().ToString() + "</p>" +
                     "<p style=\"width: 33%; margin: 0;\"><b>" + _localization.Getkey("StartDateHD") + ": </b> " + Input.NgayBatDau + "</p>" +
                     "<p style=\"width: 33%; margin: 0;\"><b>" + _localization.Getkey("EndDateHD") + ": </b> " + Input.NgayKetThuc + "</p>" +
                     "</div><br><hr><br>" +
                     "<div><table><thead><tr>" +
                     "<th>" + _localization.Getkey("DSHDStt") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDMaHD") + "</th>" +
                     "<th>" + _localization.Getkey("NgayBuy") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDKhachHang") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDTong") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDCTKMPDF") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDPTTTPDF") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDTTTTPDF") + "</th>" +
                     "<th>" + _localization.Getkey("DSHDTTDHPDF") + "</th>" +
                     "</tr></thead><tbody>";

                int stt = 1;
                foreach (var item in listData)
                {
                    htmlString += "<tr><td>" + stt + "</td>";
                    htmlString += "<td>" + item.MaHD + "</td>";
                    htmlString += "<td>" + item.NgayXuatHD + "</td>";
                    if (item.HoVaTenKH.Length <= 2)
                    {
                        htmlString += "<td>" + _localization.Getkey("KHVL") + "</td>";
                    }
                    else
                    {
                        htmlString += "<td>" + item.HoVaTenKH + "</td>";
                    }

                    htmlString += "<td>" + @String.Format("{0: ### ### ### ### VNĐ}", item.TongGiaTri) + "</td>";

                    if (item.TenCTKM.IsNullOrEmpty())
                    {
                        htmlString += "<td>" + _localization.Getkey("KAD") + "</td>";
                    }
                    else
                    {
                        htmlString += "<td>" + item.TenCTKM + "</td>";
                    }

                    htmlString += "<td>" + item.TenPTTT + "</td>";

                    switch (item.TrangThaiThanhToan)
                    {
                        case -1:
                            htmlString += "<td>" + _localization.Getkey("Pay_error") + "</td>";
                            break;
                        case 0:
                            htmlString += "<td>" + _localization.Getkey("Waiting_for_refund") + "</td>";
                            break;
                        case 1:
                            htmlString += "<td>" + _localization.Getkey("Pay_success") + "</td>";
                            break;
                        default:
                            htmlString += "<td>" + _localization.Getkey("ChoThanhToan") + "</td>";
                            break;
                    }

                    switch (item.TrangThaiDonHang)
                    {
                        case -1:
                            htmlString += "<td>" + _localization.Getkey("Cancel_bill") + "</td></tr>";
                            break;
                        case 0:
                            htmlString += "<td>" + _localization.Getkey("Waiting_for_delivery") + "</td></tr>";
                            break;
                        case 1:
                            htmlString += "<td>" + _localization.Getkey("Delivery_in_progress") + "</td></tr>";
                            break;
                        default:
                            htmlString += "<td>" + _localization.Getkey("Delivery_successful") + "</td></tr>";
                            break;
                    }

                    stt++;
                }

                htmlString += "</tbody></table></div></body>";

                string filename = "DSHD_" + DateTimeVN().Ticks + ".pdf";
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

                Console.WriteLine(filename);

                var pdf = renderer.RenderHtmlAsPdf(htmlString);

                pdf.SaveAs(filepath);

                // return file for download
                byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
                return File(fileBytes, "application/pdf", filename);
            }
        }

        public async Task<List<BillModel>> GetListBillFromTo(DateTime startDate, DateTime endDate)
        {
            var result = (from a in _context.HoaDons
                          join b in _context.KhachHangs on a.KhachHangId equals b.Id into hdb
                          from b in hdb.DefaultIfEmpty()
                          join c in _context.ThanhToans on a.MaPttt equals c.MaPttt into pttt
                          from c in pttt.DefaultIfEmpty()
                          join d in _context.KhuyenMais on a.MaKm equals d.MaKm into km
                          from d in km.DefaultIfEmpty()
                          where a.NgayXuatHd >= startDate && a.NgayXuatHd <= endDate
                          select new BillModel
                          {
                              MaHD = a.MaHoaDon,
                              NgayXuatHD = a.NgayXuatHd,
                              HoVaTenKH = b.HoKhachHang + " " + b.TenKhachHang,
                              TongGiaTri = a.TongGiaTri,
                              TenCTKM = d.TenKhuyenMai,
                              TenPTTT = c.TenPttt,
                              TrangThaiThanhToan = a.TrangThaiThanhToan,
                              TrangThaiDonHang = a.TrangThaiDonHang
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
