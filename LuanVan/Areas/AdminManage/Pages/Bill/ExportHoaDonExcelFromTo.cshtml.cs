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

    public class ExportHoaDonExcelFromToModel : BillPageModel
    {
        public ExportHoaDonExcelFromToModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
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
            if (Input.NgayKetThuc < Input.NgayBatDau)
            {
                _notyf.Error("Ngày bắt đầu không thể trễ hơn ngày kết thúc", 3);

                return Page();
            }
            else
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_localization.Getkey("DSHDTitle"));

                ws.Cell("A1").Value = ("" + _localization.Getkey("DSHDStt"));
                ws.Cell("B1").Value = "" + _localization.Getkey("DSHDMaHD");
                ws.Cell("C1").Value = "" + _localization.Getkey("DSHDNgayXuat");
                ws.Cell("D1").Value = "" + _localization.Getkey("DSHDKhachHang");
                ws.Cell("E1").Value = "" + _localization.Getkey("DSHDTong");
                ws.Cell("F1").Value = "" + _localization.Getkey("DSHDCTKM");
                ws.Cell("G1").Value = "" + _localization.Getkey("DSHDPTTT");
                ws.Cell("H1").Value = "" + _localization.Getkey("DSHDTTTT");
                ws.Cell("I1").Value = "" + _localization.Getkey("DSHDTTDH");

                ws.Range("A1:I1").Style.Font.Bold = true;

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 25;
                ws.Column(3).Width = 25;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 25;
                ws.Column(7).Width = 20;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 20;

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                var listData = await GetListBillFromTo(Input.NgayBatDau, Input.NgayKetThuc);

                int row = 2;
                int stt = 1;
                for (int i = 0; i < listData.Count(); i++)
                {
                    ws.Cell("A" + row).Value = stt;
                    ws.Cell("B" + row).Value = listData[i].MaHD;
                    ws.Cell("C" + row).Value = listData[i].NgayXuatHD;

                    if (listData[i].HoVaTenKH.Length <= 2)
                    {
                        ws.Cell("D" + row).Value = "" + _localization.Getkey("KHVL");
                    }
                    else
                    {
                        ws.Cell("D" + row).Value = listData[i].HoVaTenKH;
                    }

                    ws.Cell("E" + row).Value = listData[i].TongGiaTri;

                    if (listData[i].TenCTKM.IsNullOrEmpty())
                    {
                        ws.Cell("F" + row).Value = "" + _localization.Getkey("KAD");
                    }
                    else
                    {
                        ws.Cell("F" + row).Value = listData[i].TenCTKM;
                    }
                    ws.Cell("G" + row).Value = listData[i].TenPTTT;

                    switch (listData[i].TrangThaiThanhToan)
                    {
                        case -1:
                            ws.Cell("H" + row).Value = "" + _localization.Getkey("Pay_error");
                            break;
                        case 0:
                            ws.Cell("H" + row).Value = "" + _localization.Getkey("Waiting_for_refund");
                            break;
                        case 1:
                            ws.Cell("H" + row).Value = "" + _localization.Getkey("Pay_success");
                            break;
                        default:
                            ws.Cell("H" + row).Value = "" + _localization.Getkey("ChoThanhToan");
                            break;
                    }

                    switch (listData[i].TrangThaiDonHang)
                    {
                        case -1:
                            ws.Cell("I" + row).Value = "" + _localization.Getkey("Cancel_bill");
                            break;
                        case 0:
                            ws.Cell("I" + row).Value = "" + _localization.Getkey("Waiting_for_delivery");
                            break;
                        case 1:
                            ws.Cell("I" + row).Value = "" + _localization.Getkey("Delivery_in_progress");
                            break;
                        default:
                            ws.Cell("I" + row).Value = "" + _localization.Getkey("Delivery_successful");
                            break;
                    }

                    ws.Columns().AdjustToContents();
                    ws.Rows().AdjustToContents();

                    row++;
                    stt++;
                }

                string filename = "DSHD_" + DateTimeVN().Ticks + ".xlsx";
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

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
