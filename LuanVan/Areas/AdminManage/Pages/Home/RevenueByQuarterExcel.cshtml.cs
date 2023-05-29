using ClosedXML.Excel;
using LuanVan.Areas.Admin.Models;
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

    public class RevenueByQuarterExcelModel : HomePageModel
    {
        public RevenueByQuarterExcelModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, LanguageService localization) : base(context, webHostEnvironment, localization)
        {
        }

        public void OnGet()
        {
        }


        public IActionResult OnPost()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("" + _localization.Getkey("DTTQ"));

            ws.Cell("A1").Value = "" + _localization.Getkey("STT");
            ws.Cell("B1").Value = "" + _localization.Getkey("Quy");
            ws.Cell("C1").Value = "" + _localization.Getkey("Year");
            ws.Cell("D1").Value = "" + _localization.Getkey("TongDoanhThu");
            ws.Range("A1:D1").Style.Font.Bold = true;

            ws.Column(1).Width = 20;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 25;

            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();

            var listData = GetListRevenueByQuarter();

            int row = 2;
            int stt = 1;
            for (int i = 0; i < listData.Count(); i++)
            {
                ws.Cell("A" + row).Value = stt;
                ws.Cell("B" + row).Value = listData[i].Quy;
                ws.Cell("C" + row).Value = listData[i].Nam;
                ws.Cell("D" + row).Value = listData[i].TongDoanhThu;
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                row++;
                stt++;
            }

            string filename = "DoanhThuTheoQuy_" + DateTimeVN().Ticks + ".xlsx";
            string filepath = Path.Combine(_webHostEnvironment.ContentRootPath, "Areas", "Admin", "Resource", "ExportExcel", filename);

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

        public List<RevenueByQuarterModel> GetListRevenueByQuarter()
        {
            var result = (from h in _context.HoaDons
                          where h.TrangThaiThanhToan == 1
                          select h)
                        .AsEnumerable()
                        .GroupBy(h => new {
                            Quarter = (h.NgayXuatHd.Month - 1) / 3 + 1,
                            Year = h.NgayXuatHd.Year
                        })
                        .Select(g => new RevenueByQuarterModel
                        {
                            Quy = g.Key.Quarter,
                            Nam = g.Key.Year,
                            TongDoanhThu = g.Sum(x => x.TongGiaTri)
                        }).ToList();
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
