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

    public class ExportProductExcelModel : ProductPageModel
    {
        public ExportProductExcelModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public void OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("" + _localization.Getkey("DSProduct"));

            ws.Cell("A1").Value = "" + _localization.Getkey("STT");
            ws.Cell("B1").Value = "" + _localization.Getkey("DSProductMa");
            ws.Cell("C1").Value = "" + _localization.Getkey("DSProductName");
            ws.Cell("D1").Value = "" + _localization.Getkey("DSProductDVT");
            ws.Cell("E1").Value = "" + _localization.Getkey("DSProductNSX");
            ws.Cell("F1").Value = "" + _localization.Getkey("DSProductLSP");
            ws.Cell("G1").Value = "" + _localization.Getkey("DSProductMS");
            ws.Cell("H1").Value = "" + _localization.Getkey("DSProductGia");
            ws.Cell("I1").Value = "" + _localization.Getkey("DSProductTT");
            ws.Range("A1:I1").Style.Font.Bold = true;

            ws.Column(1).Width = 20;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 25;
            ws.Column(5).Width = 25;
            ws.Column(6).Width = 25;
            ws.Column(7).Width = 25;
            ws.Column(8).Width = 25;
            ws.Column(9).Width = 25;

            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();

            var listData = await GetListProduct();

            int row = 2;
            int stt = 1;
            for (int i = 0; i < listData.Count(); i++)
            {
                ws.Cell("A" + row).Value = stt;
                ws.Cell("B" + row).Value = listData[i].MaSanPham;
                ws.Cell("C" + row).Value = listData[i].TenSanPham;
                ws.Cell("D" + row).Value = listData[i].TenDVT;
                ws.Cell("E" + row).Value = listData[i].TenNSX;
                ws.Cell("F" + row).Value = listData[i].TenLoaiSP;
                ws.Cell("G" + row).Value = listData[i].TenMau;
                ws.Cell("H" + row).Value = listData[i].GiaBan;

                if (listData[i].TrangThai == -1)
                {
                    ws.Cell("I" + row).Value = "" + _localization.Getkey("KKD");
                }
                else if (listData[i].TrangThai == 0)
                {
                    ws.Cell("I" + row).Value = "" + _localization.Getkey("HH");
                }
                else if (listData[i].TrangThai == 1)
                {
                    ws.Cell("I" + row).Value = "" + _localization.Getkey("KM");
                }
                else
                {
                    ws.Cell("I" + row).Value = "" + _localization.Getkey("HOT");
                }


                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                row++;
                stt++;
            }

            string filename = "DSSP_" + DateTimeVN().Ticks + ".xlsx";
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
