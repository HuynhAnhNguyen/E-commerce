
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

namespace LuanVan.Areas.AdminManage.Pages.Promotion
{
    [Authorize(Roles = "Admin, Editor, Test")]


    public class ExportDiscountExcelModel : PromotionPageModel
    {
        public ExportDiscountExcelModel(ApplicationDbContext context, INotyfService notyf, ILogger<PromotionPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public void OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("" + _localization.Getkey("DSKM"));

            ws.Cell("A1").Value = "" + _localization.Getkey("STT");
            ws.Cell("B1").Value = "" + _localization.Getkey("MaKM");
            ws.Cell("C1").Value = "" + _localization.Getkey("KMName");
            ws.Cell("D1").Value = "" + _localization.Getkey("GiaTriKM");
            ws.Cell("E1").Value = "" + _localization.Getkey("DateStart");
            ws.Cell("F1").Value = "" + _localization.Getkey("DateEnd");
            ws.Cell("G1").Value = "" + _localization.Getkey("SoLuongConLai");
            ws.Range("A1:G1").Style.Font.Bold = true;

            ws.Column(1).Width = 20;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 20;
            ws.Column(5).Width = 25;
            ws.Column(6).Width = 25;
            ws.Column(7).Width = 25;

            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();

            var listData = await GetListDiscount();

            int row = 2;
            int stt = 1;
            for (int i = 0; i < listData.Count(); i++)
            {
                ws.Cell("A" + row).Value = stt;
                ws.Cell("B" + row).Value = listData[i].MaCTKM;
                ws.Cell("C" + row).Value = listData[i].TenCTKM;
                ws.Cell("D" + row).Value = listData[i].GiaTriKM;
                ws.Cell("E" + row).Value = listData[i].NgayBatDau;
                ws.Cell("F" + row).Value = listData[i].NgayKetThuc;
                ws.Cell("G" + row).Value = listData[i].SoLuongConLai;

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                row++;
                stt++;
            }

            string filename = "DSKM_" + DateTimeVN().Ticks + ".xlsx";
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

            //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
            //Console.WriteLine(fileNamePath);

            wb.SaveAs(filepath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                if (!Directory.Exists(Path.GetDirectoryName(filepath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                }
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        public async Task<List<DiscountModel>> GetListDiscount()
        {
            var result = (from a in _context.KhuyenMais
                          select new DiscountModel
                          {
                              MaCTKM = a.MaKm,
                              TenCTKM = a.TenKhuyenMai,
                              GiaTriKM = a.GiaTriKm,
                              NgayBatDau = a.NgayBatDau,
                              NgayKetThuc = a.NgayKetThuc,
                              SoLuongConLai= a.SoLuongConLai
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
