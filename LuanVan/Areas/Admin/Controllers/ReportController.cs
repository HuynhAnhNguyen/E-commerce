using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using LuanVan.Areas.Admin.Models;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Composition;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Helpers;
using System.Web.WebPages;


namespace LuanVan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        public readonly ApplicationDbContext _context;
        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public string connectionString = "Server=tcp:server-ct554-luanvan.database.windows.net,1433;Initial Catalog=LuanVan;Persist Security Info=False;User ID=HuynhAnhAdmin;Password=Huynhanh18+;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public string connectionString = "Data Source=DESKTOP-VCL1NL6;Initial Catalog=LuanVan;TrustServerCertificate=True; Integrated Security=True";
        //Test
        //public string connectionString = "Data Source=DESKTOP-VCL1NL6;Initial Catalog=LuanVan_Test;TrustServerCertificate=True; Integrated Security=True";

        [HttpGet]
        public IActionResult RevenueByDate()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 NgayThangNam, DoanhThu\r\nFROM (\r\n\t\tSELECT TOP 5 CONVERT(date, NgayXuatHd) AS NgayThangNam ,\r\n\t\t\tSUM(tonggiatri) AS DoanhThu\r\n\t\tFROM HoaDon\r\n\t\tWHERE TrangThaiThanhToan = 1 AND TrangThaiDonHang =2\r\n\t\tGROUP BY CONVERT(date, NgayXuatHd)\r\n\t\tORDER BY YEAR(CONVERT(date, NgayXuatHd)) DESC,\r\n\t\tMONTH(CONVERT(date, NgayXuatHd)) DESC,\r\n\t\tDAY(CONVERT(date, NgayXuatHd)) DESC)\r\nAS subquery\r\nORDER BY NgayThangNam ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult RevenueByWeek()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 N'Tuần ' + CAST(Tuan AS VARCHAR(2)) + N' Năm ' + CAST(Nam AS VARCHAR(4)) AS Tuan, DoanhThu\r\nFROM (\r\n    SELECT TOP 5 DATEPART(WEEK, NgayXuatHd) AS Tuan, YEAR(NgayXuatHd) AS Nam,\r\n\tSUM(tonggiatri) AS DoanhThu\r\n\tFROM HoaDon\r\n\tWHERE TrangThaiThanhToan = 1 AND TrangThaiDonHang =2\r\n\tGROUP BY DATEPART(WEEK, NgayXuatHd), YEAR(NgayXuatHd)\r\n\tORDER BY YEAR(NgayXuatHd) * 52 + DATEPART(WEEK, NgayXuatHd) DESC\r\n) AS subquery\r\nORDER BY Nam * 52 + Tuan ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult RevenueByMonth()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 N'Tháng ' + CAST(Thang AS VARCHAR(2)) + N' Năm ' + CAST(Nam AS VARCHAR(4)) AS Thang, DoanhThu\r\nFROM (\r\n\tSELECT TOP 5 MONTH(NgayXuatHd) AS Thang, YEAR(NgayXuatHd)AS Nam, \r\n\t\tSUM(tonggiatri) AS DoanhThu\r\n\tFROM HoaDon\r\n\tWHERE TrangThaiThanhToan = 1 AND TrangThaiDonHang =2\r\n\tGROUP BY MONTH(NgayXuatHd), YEAR(NgayXuatHd)\r\n\tORDER BY YEAR(NgayXuatHd) DESC,\r\n\t\t\tMONTH(NgayXuatHd) DESC) \r\nAS subquery\r\nORDER BY Nam ASC, Thang ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult RevenueByQuarter()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 N'Quý ' + CAST(Quy AS VARCHAR(2)) + N' Năm ' + CAST(Nam AS VARCHAR(4)) AS Quy, DoanhThu\r\nFROM (\r\n\tSELECT TOP 5 DATEPART(QUARTER, NgayXuatHd) AS Quy, + YEAR(NgayXuatHd) AS Nam, \r\n\t\tSUM(tonggiatri) AS DoanhThu\r\n\tFROM HoaDon\r\n\tWHERE TrangThaiThanhToan = 1 AND TrangThaiDonHang =2\r\n\tGROUP BY DATEPART(QUARTER, NgayXuatHd), YEAR(NgayXuatHd)\r\n\tORDER BY YEAR(NgayXuatHd) DESC, \r\n\t\tDATEPART(QUARTER, NgayXuatHd) DESC\r\n) \r\nAS subquery\r\nORDER BY Nam ASC, Quy ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult RevenueByYear()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 Nam, DoanhThu\r\nFROM (\r\n\tSELECT TOP 5 YEAR(NgayXuatHd) AS Nam, SUM(tonggiatri) AS DoanhThu\r\n\tFROM HoaDon\r\n\tWHERE TrangThaiThanhToan = 1 AND TrangThaiDonHang =2\r\n\tGROUP BY YEAR(NgayXuatHd)\r\n\tORDER BY YEAR(NgayXuatHd) DESC\r\n) \r\nAS subquery\r\nORDER BY Nam ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult InvoiceSuccessOrFailure()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 NgayThangNam, SoLuongThanhCong, SoLuongThatBai, TongHoaDon\r\nFROM (\r\n    SELECT TOP 5 CONVERT(date, NgayXuatHd) AS NgayThangNam,\r\n\t\tSUM(CASE WHEN TrangThaiThanhToan = 1 THEN 1 ELSE 0 END) AS SoLuongThanhCong,\r\n\t\tSUM(CASE WHEN TrangThaiThanhToan != 1 THEN 1 ELSE 0 END) AS SoLuongThatBai,\r\n\t\tCOUNT(MaHoaDon) AS TongHoaDon\r\n\tFROM HoaDon\r\n\tGROUP BY CONVERT(date, NgayXuatHd)\r\n\tORDER BY \r\n\t\tYEAR(CONVERT(date, NgayXuatHd)) DESC,\r\n\t\tMONTH(CONVERT(date, NgayXuatHd)) DESC,\r\n\t\tDAY(CONVERT(date, NgayXuatHd)) DESC\r\n) AS subquery\r\nORDER BY NgayThangNam ASC;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult GetPayByUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 5 NgayThangNam, ThanhToanBoiKH, ThanhToanBoiGuest, TongHoaDon\r\nFROM (\r\n    SELECT TOP 5 CONVERT(date, NgayXuatHd) AS NgayThangNam, \r\n        SUM(CASE WHEN KhachHangId IS NOT NULL THEN 1 ELSE 0 END) AS ThanhToanBoiKH,\r\n        SUM(CASE WHEN KhachHangId IS NULL THEN 1 ELSE 0 END) AS ThanhToanBoiGuest,\r\n        COUNT(MaHoaDon) AS TongHoaDon\r\n    FROM HoaDon\r\n    GROUP BY CONVERT(date, NgayXuatHd)\r\n\tORDER BY \r\n\tYEAR(CONVERT(date, NgayXuatHd)) DESC,\r\n\tMONTH(CONVERT(date, NgayXuatHd)) DESC,\r\n\tDAY(CONVERT(date, NgayXuatHd)) DESC\r\n) AS subquery\r\nORDER BY NgayThangNam ASC;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult GetAgeUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT DATEDIFF(YEAR, NgaySinh, GETDATE()) AS DoTuoi, COUNT(NgaySinh) AS SoLuong\r\nFROM KhachHang\r\nGROUP BY DATEDIFF(YEAR, NgaySinh, GETDATE())";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult GetUsersByGender()
        {
            // Kết nối cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT GioiTinh, COUNT(GioiTinh) AS SoLuong\r\nFROM KhachHang\r\nGROUP BY GioiTinh";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult GetProductByProducer()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT a.TenNsx, COUNT(b.TenSanPham) AS SoLuong\r\nFROM NhaSanXuat a LEFT JOIN SanPham b ON a.MaNsx= b.MaNsx\r\nGROUP BY a.TenNsx\r\nORDER BY a.TenNsx";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult GetProductByType()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT c.TenLoaiSp, COUNT(p.TenSanPham) AS SoLuong\r\nFROM LoaiSanPham c\r\nLEFT JOIN SanPham p ON c.MaLoaiSp = p.MaLoaiSp\r\nGROUP BY c.TenLoaiSp\r\nORDER BY c.TenLoaiSp";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult PercentByPayment()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT a.TenPttt AS TenPTTT, ROUND((COUNT(hd.MaHoaDon)* 100.0 / (SELECT COUNT(*) FROM HoaDon)),2) as PhanTram\r\nFROM ThanhToan a\r\nJOIN HoaDon hd ON a.MaPttt = hd.MaPttt\r\nGROUP BY a.TenPttt";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Tạo mảng chứa dữ liệu
                        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            data.Add(row);
                        }

                        // Trả về dữ liệu dưới dạng JSON
                        return Json(data);
                    }
                }
            }
        }

        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(_localization.Getkey("DSHDTitle"));

        //    ws.Cell("A1").Value = (""+_localization.Getkey("DSHDStt"));
        //    ws.Cell("B1").Value = ""+ _localization.Getkey("DSHDMaHD");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("DSHDNgayXuat");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("DSHDKhachHang");
        //    ws.Cell("E1").Value = "" + _localization.Getkey("DSHDTong");
        //    ws.Cell("F1").Value = "" + _localization.Getkey("DSHDCTKM");
        //    ws.Cell("G1").Value = "" + _localization.Getkey("DSHDPTTT");
        //    ws.Cell("H1").Value = "" + _localization.Getkey("DSHDTTTT");
        //    ws.Cell("I1").Value = "" + _localization.Getkey("DSHDTTDH");
        //    ws.Range("A1:I1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 20;
        //    ws.Column(5).Width = 20;
        //    ws.Column(6).Width = 25;
        //    ws.Column(7).Width = 20;
        //    ws.Column(8).Width = 20;
        //    ws.Column(9).Width = 20;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listHD =await GetListHoaDon();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listHD.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listHD[i].MaHD;
        //        ws.Cell("C" + row).Value = listHD[i].NgayXuatHD;
        //        if (listHD[i].HoVaTenKH.Length<= 2)
        //        {
        //            ws.Cell("D" + row).Value = "" + _localization.Getkey("KHVL");
        //        }
        //        else
        //        {
        //            ws.Cell("D" + row).Value = listHD[i].HoVaTenKH;
        //        }
        //        ws.Cell("E" + row).Value = listHD[i].TongGiaTri;
        //        if (listHD[i].TenCTKM.IsNullOrEmpty())
        //        {
        //            ws.Cell("F" + row).Value = "" + _localization.Getkey("KAD");
        //        }
        //        else
        //        {
        //            ws.Cell("F" + row).Value = listHD[i].TenCTKM;
        //        }
        //        ws.Cell("G" + row).Value = listHD[i].TenPTTT;

        //        if (listHD[i].TrangThaiThanhToan== -1)
        //        {
        //            ws.Cell("H" + row).Value = "" + _localization.Getkey("Pay_error");
        //        }
        //        else if (listHD[i].TrangThaiThanhToan == 0)
        //        {
        //            ws.Cell("H" + row).Value = "" + _localization.Getkey("Waiting_for_refund");
        //        }
        //        else
        //        {
        //            ws.Cell("H" + row).Value = "" + _localization.Getkey("Pay_success");
        //        }

        //        if (listHD[i].TrangThaiDonHang == -1)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Cancel_bill");
        //        }
        //        else if (listHD[i].TrangThaiDonHang == 0)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Waiting_for_delivery");
        //        }
        //        else if (listHD[i].TrangThaiDonHang == 1)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Delivery_in_progress");
        //        }
        //        else 
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Delivery_successful");
        //        }


        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSHD_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListHoaDon();

        //    string htmlString = "<div     style=\"text-align: center;\r\n    text-transform: uppercase;\r\n    font-weight: bold;\">"+ _localization.Getkey("DSHDTitle") + "</div> <hr>";
        //    htmlString += "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDStt") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDMaHD") + "</th>";
        //    //htmlString += "<th>Ngày xuất hóa đơn</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDKhachHang") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDTong") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDCTKMPDF") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDPTTTPDF") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDTTTTPDF") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDTTDHPDF") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaHD + "</td>";
        //        //htmlString += "<td>" + item.NgayXuatHD + "</td>";
        //        if (item.HoVaTenKH.Length <= 2)
        //        {
        //            htmlString += "<td>"+ _localization.Getkey("KHVL") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>"+ item.HoVaTenKH+ "</td>";
        //        }

        //        htmlString += "<td>" + item.TongGiaTri + "</td>";

        //        if (item.TenCTKM.IsNullOrEmpty())
        //        {
        //            htmlString += "<td>" + _localization.Getkey("KAD") + "</td>"; 
        //        }
        //        else
        //        {
        //            htmlString += "<td>" + item.TenCTKM + "</td>";
        //        }

        //        htmlString += "<td>" + item.TenPTTT + "</td>";

        //        if (item.TrangThaiThanhToan == -1)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Pay_error") + "</td>";
        //        }
        //        else if (item.TrangThaiThanhToan == 0)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Waiting_for_refund") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Pay_success") + "</td>";
        //        }

        //        if (item.TrangThaiDonHang == -1)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Cancel_bill") + "</td>";
        //        }
        //        else if (item.TrangThaiDonHang == 0)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Waiting_for_delivery") + "</td>";
        //        }
        //        else if (item.TrangThaiDonHang == 1)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Delivery_in_progress") + "</td>";
        //        }else 
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Delivery_successful") + "</td>";
        //        }

        //        stt++;
        //    }

        //    htmlString += "</tr></tbody></table>";



        //    string filename = "DSHD_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);

        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpPost]
        //public ActionResult PrintBill(string maHoaDon)
        //{
        //    HttpContext.Session.SetString("maHoaDon", maHoaDon);

        //    return Json(new { success = true, message = "MaHoaDon đã được lưu." });
        //}

        //public async Task<List<BillModel>> GetListHoaDon()
        //{
        //    var result = (from a in _context.HoaDons
        //                  join b in _context.KhachHangs on a.KhachHangId equals b.Id into hdb
        //                  from b in hdb.DefaultIfEmpty()
        //                  join c in _context.ThanhToans on a.MaPttt equals c.MaPttt into pttt
        //                  from c in pttt.DefaultIfEmpty()
        //                  join d in _context.KhuyenMais on a.MaKm equals d.MaKm into km
        //                  from d in km.DefaultIfEmpty()
        //                  select new BillModel
        //                  {
        //                      MaHD = a.MaHoaDon,
        //                      NgayXuatHD = a.NgayXuatHd,
        //                      HoVaTenKH = b.HoKhachHang + " " + b.TenKhachHang,
        //                      TongGiaTri = a.TongGiaTri,
        //                      TenCTKM = d.TenKhuyenMai,
        //                      TenPTTT = c.TenPttt,
        //                      TrangThaiThanhToan = a.TrangThaiThanhToan,
        //                      TrangThaiDonHang= a.TrangThaiDonHang
        //                  }).ToListAsync();

        //    return await result;
        //}


        //[HttpPost]
        //public async Task<ActionResult> PrintExcelBillFromTo(DateTime startDate, DateTime endDate)
        //{
        //    HttpContext.Session.SetString("startDate", startDate.ToString());
        //    HttpContext.Session.SetString("endDate", endDate.ToString());


        //    //await ExportHoaDonExcelFromTo(await result);
        //    // Xử lý tại đây, in danh sách hóa đơn theo định dạng Excel
        //    // startDate và endDate lúc này đã chứa giá trị được gửi từ AJAX
        //    // và có kiểu dữ liệu là DateTime
        //    // Sau khi in xong, trả về một ActionResult tùy theo logic của bạn, ví dụ:
        //    return Json(new { success = true });
        //}

        //[HttpPost]
        //public async Task<ActionResult> PrintPdfBillFromTo(DateTime startDate, DateTime endDate)
        //{
        //    HttpContext.Session.SetString("startDate", startDate.ToString());
        //    HttpContext.Session.SetString("endDate", endDate.ToString());


        //    //await ExportHoaDonExcelFromTo(await result);
        //    // Xử lý tại đây, in danh sách hóa đơn theo định dạng Excel
        //    // startDate và endDate lúc này đã chứa giá trị được gửi từ AJAX
        //    // và có kiểu dữ liệu là DateTime
        //    // Sau khi in xong, trả về một ActionResult tùy theo logic của bạn, ví dụ:
        //    return Json(new { success = true });
        //}

        //public async Task<List<BillModel>> GetListBillFromTo()
        //{
        //    DateTime startDate = DateTime.Parse(HttpContext.Session.GetString("startDate"));
        //    DateTime endDate = DateTime.Parse(HttpContext.Session.GetString("endDate"));

        //    var result = (from a in _context.HoaDons
        //                  join b in _context.KhachHangs on a.KhachHangId equals b.Id into hdb
        //                  from b in hdb.DefaultIfEmpty()
        //                  join c in _context.ThanhToans on a.MaPttt equals c.MaPttt into pttt
        //                  from c in pttt.DefaultIfEmpty()
        //                  join d in _context.KhuyenMais on a.MaKm equals d.MaKm into km
        //                  from d in km.DefaultIfEmpty()
        //                  where a.NgayXuatHd >= startDate && a.NgayXuatHd <= endDate
        //                  select new BillModel
        //                  {
        //                      MaHD = a.MaHoaDon,
        //                      NgayXuatHD = a.NgayXuatHd,
        //                      HoVaTenKH = b.HoKhachHang + " " + b.TenKhachHang,
        //                      TongGiaTri = a.TongGiaTri,
        //                      TenCTKM = d.TenKhuyenMai,
        //                      TenPTTT = c.TenPttt,
        //                      TrangThaiThanhToan = a.TrangThaiThanhToan,
        //                      TrangThaiDonHang = a.TrangThaiDonHang
        //                  }).ToListAsync();

        //    return await result;
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonExcelFromTo() {
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(_localization.Getkey("DSHDTitle"));

        //    ws.Cell("A1").Value = ("" + _localization.Getkey("DSHDStt"));
        //    ws.Cell("B1").Value = "" + _localization.Getkey("DSHDMaHD");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("DSHDNgayXuat");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("DSHDKhachHang");
        //    ws.Cell("E1").Value = "" + _localization.Getkey("DSHDTong");
        //    ws.Cell("F1").Value = "" + _localization.Getkey("DSHDCTKM");
        //    ws.Cell("G1").Value = "" + _localization.Getkey("DSHDPTTT");
        //    ws.Cell("H1").Value = "" + _localization.Getkey("DSHDTTTT");
        //    ws.Cell("I1").Value = "" + _localization.Getkey("DSHDTTDH");

        //    ws.Range("A1:I1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 20;
        //    ws.Column(5).Width = 20;
        //    ws.Column(6).Width = 25;
        //    ws.Column(7).Width = 20;
        //    ws.Column(8).Width = 20;
        //    ws.Column(9).Width = 20;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListBillFromTo();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].MaHD;
        //        ws.Cell("C" + row).Value = listData[i].NgayXuatHD;
        //        if (listData[i].HoVaTenKH.Length <= 2)
        //        {
        //            ws.Cell("D" + row).Value = "" + _localization.Getkey("KHVL");
        //        }
        //        else
        //        {
        //            ws.Cell("D" + row).Value = listData[i].HoVaTenKH;
        //        }
        //        ws.Cell("E" + row).Value = listData[i].TongGiaTri;
        //        if (listData[i].TenCTKM.IsNullOrEmpty())
        //        {
        //            ws.Cell("F" + row).Value = "" + _localization.Getkey("KAD");
        //        }
        //        else
        //        {
        //            ws.Cell("F" + row).Value = listData[i].TenCTKM;
        //        }
        //        ws.Cell("G" + row).Value = listData[i].TenPTTT;

        //        if (listData[i].TrangThaiThanhToan == -1)
        //        {
        //            ws.Cell("H" + row).Value = "" + _localization.Getkey("Pay_error");
        //        }
        //        else if (listData[i].TrangThaiThanhToan == 0)
        //        {
        //            ws.Cell("H" + row).Value = "" + _localization.Getkey("Waiting_for_refund");
        //        }
        //        else
        //        {
        //            ws.Cell("H" + row).Value = "" + _localization.Getkey("Pay_success");
        //        }

        //        if (listData[i].TrangThaiDonHang == -1)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Cancel_bill");
        //        }
        //        else if (listData[i].TrangThaiDonHang == 0)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Waiting_for_delivery");
        //        }
        //        else if (listData[i].TrangThaiDonHang == 1)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Delivery_in_progress");
        //        }
        //        else
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("Delivery_successful");
        //        }


        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSHD_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonPdfFromTo()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListBillFromTo();

        //    string htmlString = "<div     style=\"text-align: center;\r\n    text-transform: uppercase;\r\n    font-weight: bold;\">" + _localization.Getkey("DSHDTitle") + "</div> <hr>";
        //    htmlString += "<table><thead><tr>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDStt") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDMaHD") + "</th>";
        //    //htmlString += "<th>Ngày xuất hóa đơn</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDKhachHang") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDTong") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDCTKMPDF") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDPTTTPDF") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDTTTTPDF") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("DSHDTTDHPDF") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaHD + "</td>";
        //        //htmlString += "<td>" + item.NgayXuatHD + "</td>";
        //        if (item.HoVaTenKH.Length <= 2)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("KHVL") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>" + item.HoVaTenKH + "</td>";
        //        }

        //        htmlString += "<td>" + item.TongGiaTri + "</td>";

        //        if (item.TenCTKM.IsNullOrEmpty())
        //        {
        //            htmlString += "<td>" + _localization.Getkey("KAD") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>" + item.TenCTKM + "</td>";
        //        }

        //        htmlString += "<td>" + item.TenPTTT + "</td>";

        //        if (item.TrangThaiThanhToan == -1)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Pay_error") + "</td>";
        //        }
        //        else if (item.TrangThaiThanhToan == 0)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Waiting_for_refund") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Pay_success") + "</td>";
        //        }

        //        if (item.TrangThaiDonHang == -1)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Cancel_bill") + "</td>";
        //        }
        //        else if (item.TrangThaiDonHang == 0)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Waiting_for_delivery") + "</td>";
        //        }
        //        else if (item.TrangThaiDonHang == 1)
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Delivery_in_progress") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>" + _localization.Getkey("Delivery_successful") + "</td>";
        //        }

        //        stt++;
        //    }

        //    htmlString += "</tr></tbody></table>";

        //    string filename = "DSHD_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);

        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}


        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonPdfById()
        //{
        //    string maHoaDon = HttpContext.Session.GetString("maHoaDon");

        //    Console.WriteLine(maHoaDon);

        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    HoaDon hoaDon;
        //    List<ChiTietHd> chiTietHoaDons;
        //    string tenPhuongThucThanhToan;
        //    string khuyenMai;
        //    string trangThaiThanhToan;
        //    string trangThaiDonHang;

        //    if (maHoaDon.IsNullOrEmpty())
        //    {
        //        return Json("Error. Try again");
        //    }
        //    else
        //    {
        //        hoaDon = await _context.HoaDons.Where(x => x.MaHoaDon == maHoaDon).FirstOrDefaultAsync();

        //        if (hoaDon.TrangThaiThanhToan == -1)
        //        {
        //            trangThaiThanhToan = "" + _localization.Getkey("Pay_error");
        //        }
        //        else if (hoaDon.TrangThaiThanhToan == 0)
        //        {
        //            trangThaiThanhToan = "" + _localization.Getkey("Waiting_for_refund");
        //        }
        //        else
        //        {
        //            trangThaiThanhToan = "" + _localization.Getkey("Pay_success");
        //        }

        //        if (hoaDon.TrangThaiDonHang == -1)
        //        {
        //            trangThaiDonHang = "" + _localization.Getkey("Cancel_bill");
        //        }
        //        else if (hoaDon.TrangThaiDonHang == 0)
        //        {
        //            trangThaiDonHang = "" + _localization.Getkey("Waiting_for_delivery");
        //        }
        //        else if (hoaDon.TrangThaiDonHang == 1)
        //        {
        //            trangThaiDonHang = "" + _localization.Getkey("Delivery_in_progress");
        //        }
        //        else
        //        {
        //            trangThaiDonHang = "" + _localization.Getkey("Delivery_successful");
        //        }

        //        chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == maHoaDon).ToListAsync();

        //        List<SanPham> sanPhams = new List<SanPham>();
        //        List<GioHang> gioHangs = new List<GioHang>();
        //        List<LoaiSanPham> loaiSanPhams = new List<LoaiSanPham>();

        //        foreach (var chiTietHoaDon in chiTietHoaDons)
        //        {
        //            GioHang gioHang = await _context.GioHangs.Where(x => x.MaGioHang == chiTietHoaDon.MaGioHang).FirstOrDefaultAsync();
        //            SanPham sanPham = await _context.SanPhams.Where(x => x.MaSanPham == gioHang.MaSanPham).FirstOrDefaultAsync();
        //            LoaiSanPham loaiSanPham = await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == sanPham.MaLoaiSp).FirstOrDefaultAsync();

        //            sanPhams.Add(sanPham);
        //            gioHangs.Add(gioHang);
        //            loaiSanPhams.Add(loaiSanPham);
        //        }

        //        ViewData["sanPhams"] = sanPhams;
        //        ViewData["gioHangs"] = gioHangs;
        //        ViewData["loaiSanPhams"] = loaiSanPhams;

        //        tenPhuongThucThanhToan = await (from a in _context.HoaDons join b in _context.ThanhToans on a.MaPttt equals b.MaPttt where a.MaHoaDon == maHoaDon select b.TenPttt).FirstOrDefaultAsync();


        //        if (!hoaDon.MaKm.IsNullOrEmpty())
        //        {
        //            khuyenMai = await (from a in _context.KhuyenMais
        //                               join b in _context.HoaDons on a.MaKm equals b.MaKm
        //                               where b.MaHoaDon == maHoaDon
        //                               select a.TenKhuyenMai).FirstOrDefaultAsync();
        //        }
        //        else khuyenMai = "" + _localization.Getkey("KAD");

        //        //string htmlString = "<div     style=\"text-align: center;\r\n    text-transform: uppercase;\r\n    font-weight: bold;\"> Danh sách hóa đơn</div> <hr>";

        //        string htmlString = $"<div class=\"product-area section pt-4\">" +
        //            "<div class=\"container col-7\">" +
        //            "<div class=\"row\">" +
        //            "<div class=\"col-12\">" +
        //            "<div class=\"section-title p-0\">" +
        //            "<h2>"+ _localization.Getkey("TTHD") + " " + maHoaDon + "</h2>" +
        //            "</div>" +
        //            "</div>" +
        //            "</div>" +
        //            "<div class=\"row\">" +
        //            "<div class=\"col-12\">" +
        //            "<div class=\"card\">" +
        //            "<div class=\"card-body p-2\">" +
        //            "<table class=\"table v-middle table-hover\">" +
        //            "<tr><th>" + _localization.Getkey("DSHDMaHD") + "</th><td>" + maHoaDon + "</td></tr>" +
        //            "<tr><th>" + _localization.Getkey("DSHDNgayXuat") + "</th><td>" + hoaDon.NgayXuatHd + "</td></tr>" +
        //            "<tr><th>" + _localization.Getkey("DSHDTong") + "</th><td>" + hoaDon.TongGiaTri + "</td></tr>" +
        //            "<tr><th>" + _localization.Getkey("DSHDCTKMPDF") + "</th><td>" + khuyenMai + "</td></tr>" +
        //            "<tr><th>" + _localization.Getkey("DSHDPTTTPDF") + "</th><td>" + tenPhuongThucThanhToan + "</td></tr>" +
        //            "<tr><th>" + _localization.Getkey("DSHDTTTTPDF") + "</th><td>" + trangThaiThanhToan + "</td></tr>" +
        //            "<tr><th>" + _localization.Getkey("DSHDTTDHPDF") + "</th><td>" + trangThaiDonHang + "</td></tr>" +
        //            "</table></div></div></div><br /><br />" +
        //            "<div></div>";
        //        htmlString += "<div class=\"col-12\">" +
        //        "<div class=\"card p-1\">" +
        //        "<div class=\"card-body p-2\">" +
        //        "<table class=\"table v-middle table-hover\">" +
        //        "<thead><tr>" +
        //        "<th>"+ _localization.Getkey("Product_name") + "</th>\t" +
        //        "<th>" + _localization.Getkey("Product_Type") + "</th>\t" +
        //        "<th>" + _localization.Getkey("Quantity") + "</th>\t" +
        //        "<th>" + _localization.Getkey("Price") + "</th>\t" +
        //        "</tr></thead>" +
        //        "<tbody><tr>";

        //        var sanPhamss = ViewData["sanPhams"] as IEnumerable<SanPham>;
        //        var gioHangss = ViewData["gioHangs"] as IEnumerable<GioHang>;
        //        var loaiSanPhamss = ViewData["loaiSanPhams"] as IEnumerable<LoaiSanPham>;

        //        for (int i = 0; i < chiTietHoaDons.Count(); i++)
        //        {
        //            var sanPham = sanPhamss.ElementAt(i);
        //            var gioHang = gioHangss.ElementAt(i);
        //            var loaiSanPham = loaiSanPhamss.ElementAt(i);

        //            htmlString += "<td>" + sanPham.TenSanPham + "</td>";
        //            htmlString += "<td>" + loaiSanPham.TenLoaiSp + "</td>";
        //            htmlString += "<td>" + gioHang.SoLuongDat + "</td>";
        //            htmlString += "<td> " + sanPham.GiaBan + "</td>";
        //        }

        //        htmlString += "</tr></tbody></table></div></div></div></div></div>";



        //        string filename = "HD_" + maHoaDon + "_" + DateTime.Now.Ticks + ".pdf";
        //        string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //        Console.WriteLine(filename);

        //        var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //        pdf.SaveAs(filepath);

        //        // return file for download
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);

        //        HttpContext.Session.Clear();

        //        return File(fileBytes, "application/pdf", filename);
        //    }
        //}

        //public async Task<List<ColorModel>> GetListColor()
        //{
        //    var result = (from a in _context.MauSacs
        //                  select new ColorModel
        //                  {
        //                      MaMau = a.MaMau,
        //                      TenMau = a.TenMau
        //                  }).ToListAsync();

        //    return await result;
        //}

        //public async Task<List<DiscountModel>> GetListDiscount()
        //{
        //    var result = (from a in _context.KhuyenMais
        //                  select new DiscountModel
        //                  {
        //                      MaCTKM = a.MaKm,
        //                      TenCTKM = a.TenKhuyenMai,
        //                      GiaTriKM= a.GiaTriKm,
        //                      NgayBatDau= a.NgayBatDau,
        //                      NgayKetThuc= a.NgayKetThuc
        //                  }).ToListAsync();

        //    return await result;
        //}

        //public async Task<List<PaymentModel>> GetListPayment()
        //{
        //    var result = (from a in _context.ThanhToans
        //                  select new PaymentModel
        //                  {
        //                      MaPTTT= a.MaPttt,
        //                      TenPTTT= a.TenPttt
        //                  }).ToListAsync();

        //    return await result;
        //}

        //public async Task<List<ProducerModel>> GetListProducer()
        //{
        //    var result = (from a in _context.NhaSanXuats
        //                  select new ProducerModel
        //                  {
        //                      MaNSX= a.MaNsx,
        //                      TenNSX= a.TenNsx
        //                  }).ToListAsync();

        //    return await result;
        //}

        //public async Task<List<ProductTypeModel>> GetListProductType()
        //{
        //    var result = (from a in _context.LoaiSanPhams
        //                  select new ProductTypeModel
        //                  {
        //                      MaLoaiSP= a.MaLoaiSp,
        //                      TenLoaiSP= a.TenLoaiSp
        //                  }).ToListAsync();

        //    return await result;
        //}


        //public async Task<List<RoleModel>> GetListRole()
        //{
        //    var result = (from a in _context.Roles
        //                  select new RoleModel
        //                  {
        //                      ID = a.Id,
        //                      TenRole = a.Name
        //                  }).ToListAsync();

        //    return await result;
        //}

        //public async Task<List<UserModel>> GetListUser()
        //{
        //    var result = (from a in _context.KhachHangs
        //                  select new UserModel
        //                  {
        //                      ID = a.Id,
        //                      HoKhachHang= a.HoKhachHang,
        //                      TenKhachHang= a.TenKhachHang,
        //                      NgaySinh= a.NgaySinh,
        //                      GioiTinh= a.GioiTinh,
        //                      DiaChi= a.DiaChi,
        //                      UserName= a.UserName,
        //                      Email= a.Email,
        //                      PhoneNumber= a.PhoneNumber
        //                  }).ToListAsync();

        //    return await result;
        //}

        //public async Task<List<ProductModel>> GetListProduct()
        //{
        //    var result = (from a in _context.SanPhams
        //                  join b in _context.NhaSanXuats on a.MaNsx equals b.MaNsx into hdb
        //                  from b in hdb.DefaultIfEmpty()
        //                  join c in _context.LoaiSanPhams on a.MaLoaiSp equals c.MaLoaiSp into pttt
        //                  from c in pttt.DefaultIfEmpty()
        //                  join d in _context.MauSacs on a.MaMau equals d.MaMau into km
        //                  from d in km.DefaultIfEmpty()
        //                  select new ProductModel
        //                  {
        //                      MaSanPham= a.MaSanPham,
        //                      TenSanPham= a.TenSanPham,
        //                      TenDVT= a.TenDvt,
        //                      TenNSX= b.TenNsx,
        //                      TenLoaiSP= c.TenLoaiSp,
        //                      TenMau= d.TenMau,
        //                      GiaBan= a.GiaBan,
        //                      TrangThai= a.TrangThai,
        //                      MoTa= a.MoTa
        //                  }).ToListAsync();

        //    return await result;
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportColorExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(_localization.Getkey("DSMS"));

        //    ws.Cell("A1").Value = ""+ _localization.Getkey("DSHDStt");
        //    ws.Cell("B1").Value = ""+ _localization.Getkey("DSMSMaMau");
        //    ws.Cell("C1").Value = ""+ _localization.Getkey("DSMSTenMau");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listColor = await GetListColor();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listColor.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listColor[i].MaMau;
        //        ws.Cell("C" + row).Value = listColor[i].TenMau;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSMS_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportColorPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListColor();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDStt") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSMSMaMau") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSMSTenMau") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaMau + "</td>";
        //        htmlString += "<td>" + item.TenMau + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";


        //    string filename = "DSMS_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportPaymentExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DSPTTT"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("DSHDStt");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("DSPTTTMa");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("DSPTTTTen");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listPayment = await GetListPayment();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listPayment.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listPayment[i].MaPTTT;
        //        ws.Cell("C" + row).Value = listPayment[i].TenPTTT;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSPTTT_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportPaymentPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListPayment();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("DSHDStt") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSPTTTMa") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSPTTTTen") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaPTTT + "</td>";
        //        htmlString += "<td>" + item.TenPTTT + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DSPTTT_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);

        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportProducerExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DSProducer"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("DSProducerMa");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("DSProducerName");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListProducer();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].MaNSX;
        //        ws.Cell("C" + row).Value = listData[i].TenNSX;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSNSX_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportProducerPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListProducer();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProducerMa") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProducerName") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaNSX + "</td>";
        //        htmlString += "<td>" + item.TenNSX + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";



        //    string filename = "DSNSX_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportProductExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+_localization.Getkey("DSProduct"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("DSProductMa");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("DSProductName");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("DSProductDVT");
        //    ws.Cell("E1").Value = "" + _localization.Getkey("DSProductNSX");
        //    ws.Cell("F1").Value = "" + _localization.Getkey("DSProductLSP");
        //    ws.Cell("G1").Value = "" + _localization.Getkey("DSProductMS");
        //    ws.Cell("H1").Value = "" + _localization.Getkey("DSProductGia");
        //    ws.Cell("I1").Value = "" + _localization.Getkey("DSProductTT");
        //    ws.Range("A1:I1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 25;
        //    ws.Column(5).Width = 25;
        //    ws.Column(6).Width = 25;
        //    ws.Column(7).Width = 25;
        //    ws.Column(8).Width = 25;
        //    ws.Column(9).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListProduct();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].MaSanPham;
        //        ws.Cell("C" + row).Value = listData[i].TenSanPham;
        //        ws.Cell("D" + row).Value = listData[i].TenDVT;
        //        ws.Cell("E" + row).Value = listData[i].TenNSX;
        //        ws.Cell("F" + row).Value = listData[i].TenLoaiSP;
        //        ws.Cell("G" + row).Value = listData[i].TenMau;
        //        ws.Cell("H" + row).Value = listData[i].GiaBan;

        //        if (listData[i].TrangThai == -1)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("KKD");
        //        }
        //        else if (listData[i].TrangThai == 0)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("HH");
        //        }
        //        else if (listData[i].TrangThai == 1)
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("KM");
        //        }
        //        else
        //        {
        //            ws.Cell("I" + row).Value = "" + _localization.Getkey("HOT");
        //        }


        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSSP_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportProductPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListProduct();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductMa") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductName") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductDVT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductNSX") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductLSP") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductMS") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductGia") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSProductTT") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaSanPham + "</td>";
        //        htmlString += "<td>" + item.TenSanPham + "</td>";
        //        htmlString += "<td>" + item.TenDVT + "</td>";
        //        htmlString += "<td>" + item.TenNSX + "</td>";
        //        htmlString += "<td>" + item.TenLoaiSP + "</td>";
        //        htmlString += "<td>" + item.TenMau + "</td>";
        //        htmlString += "<td>" + item.GiaBan + "</td>";
        //        if (item.TrangThai == -1)
        //        {
        //            htmlString += "<td> "+ _localization.Getkey("KKD") + "</td>";
        //        }
        //        else if (item.TrangThai == 0)
        //        {
        //            htmlString += "<td> "+ _localization.Getkey("HH") + "</td>";
        //        }
        //        else if (item.TrangThai == 1)
        //        {
        //            htmlString += "<td> "+ _localization.Getkey("KM") + "</td>";
        //        }
        //        else
        //        {
        //            htmlString += "<td> "+ _localization.Getkey("HOT") + "</td>";
        //        }


        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";



        //    string filename = "DSSP_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportProductTypeExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+_localization.Getkey("DSLSP"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("DSLSPMa");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("DSLSPName");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListProductType();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].MaLoaiSP;
        //        ws.Cell("C" + row).Value = listData[i].TenLoaiSP;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSLSP_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportProductTypePdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListProductType();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSLSPMa") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DSLSPName") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaLoaiSP + "</td>";
        //        htmlString += "<td>" + item.TenLoaiSP + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DSLSP_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportDiscountExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DSKM"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("MaKM");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("KMName");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("GiaTriKM");
        //    ws.Cell("E1").Value = "" + _localization.Getkey("DateStart");
        //    ws.Cell("F1").Value = "" + _localization.Getkey("DateEnd");
        //    ws.Range("A1:F1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 20;
        //    ws.Column(5).Width = 25;
        //    ws.Column(6).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListDiscount();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].MaCTKM;
        //        ws.Cell("C" + row).Value = listData[i].TenCTKM;
        //        ws.Cell("D" + row).Value = listData[i].GiaTriKM;
        //        ws.Cell("E" + row).Value = listData[i].NgayBatDau;
        //        ws.Cell("F" + row).Value = listData[i].NgayKetThuc;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSKM_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportDiscountPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = await GetListDiscount();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("MaKM") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("KMName") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("GiaTriKM") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DateStart") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DateEnd") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaCTKM + "</td>";
        //        htmlString += "<td>" + item.TenCTKM + "</td>";
        //        htmlString += "<td>" + item.GiaTriKM + "</td>";
        //        htmlString += "<td>" + item.NgayBatDau + "</td>";
        //        htmlString += "<td>" + item.NgayKetThuc + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DSKM_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportRoleExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DSRole"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("IDRole");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("RoleName");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListRole();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].ID;
        //        ws.Cell("C" + row).Value = listData[i].TenRole;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSRole_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportRolePdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = await GetListRole();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("IDRole") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("RoleName") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.ID + "</td>";
        //        htmlString += "<td>" + item.TenRole + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";


        //    string filename = "DSRole_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportUserExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DSUser"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("UserID");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("UserLastname");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("UserFirstname");
        //    ws.Cell("E1").Value = "" + _localization.Getkey("UserDateofbirth");
        //    ws.Cell("F1").Value = "" + _localization.Getkey("UserGender");
        //    ws.Cell("G1").Value = "" + _localization.Getkey("UserAddress");
        //    ws.Cell("H1").Value = "" + _localization.Getkey("UserUsername");
        //    ws.Cell("I1").Value = "" + _localization.Getkey("UserEmail");
        //    ws.Cell("J1").Value = "" + _localization.Getkey("UserPhoneNumber");
        //    ws.Range("A1:J1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = await GetListUser();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].ID;
        //        ws.Cell("C" + row).Value = listData[i].HoKhachHang;
        //        ws.Cell("D" + row).Value = listData[i].TenKhachHang;
        //        ws.Cell("E" + row).Value = listData[i].NgaySinh;
        //        ws.Cell("F" + row).Value = listData[i].GioiTinh;
        //        ws.Cell("G" + row).Value = listData[i].DiaChi;
        //        ws.Cell("H" + row).Value = listData[i].UserName;
        //        ws.Cell("I" + row).Value = listData[i].Email;
        //        ws.Cell("J" + row).Value = listData[i].PhoneNumber;

        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSUser_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportUserPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListUser();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("UserID") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("UserFullName") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("UserDateofbirth") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("UserGender") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("UserUsername") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("UserEmail") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.ID + "</td>";
        //        htmlString += "<td>" + item.HoKhachHang+ " "+ item.TenKhachHang + "</td>";
        //        htmlString += "<td>" + item.NgaySinh + "</td>";
        //        htmlString += "<td>" + item.GioiTinh + "</td>";
        //        htmlString += "<td>" + item.UserName + "</td>";
        //        htmlString += "<td>" + item.Email + "</td>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DSUser_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //public List<RevenueByDateModel> GetListRevenueByDate()
        //{
        //    var result = (from h in _context.HoaDons
        //                        where h.TrangThaiThanhToan == 1
        //                        select h)
        //            .AsEnumerable()
        //            .GroupBy(h => h.NgayXuatHd.ToString("dd-MM-yyyy"))
        //            .Select(g => new RevenueByDateModel
        //            {
        //                NgayThangNam = g.Key,
        //                TongDoanhThu = g.Sum(x => x.TongGiaTri)
        //            }).ToList();

        //    return result;
        //}

        //[HttpGet]
        //public ActionResult RevenueByDateExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add("" + _localization.Getkey("DTTN"));

        //    ws.Cell("A1").Value = ""+_localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("DTTN_Ngay");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("TongDoanhThu");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = GetListRevenueByDate();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].NgayThangNam;
        //        ws.Cell("C" + row).Value = listData[i].TongDoanhThu;
        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DoanhThuTheoNgay_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public ActionResult RevenueByDatePdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = GetListRevenueByDate();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("DTTN_Ngay") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("TongDoanhThu") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.NgayThangNam + "</td>";
        //        htmlString += "<td>" + item.TongDoanhThu + "</td></tr>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DoanhThuTheoNgay_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}


        //public List<RevenueByWeekModel> GetListRevenueByWeek()
        //{

        //    var result = (from h in _context.HoaDons
        //                  where h.TrangThaiThanhToan == 1
        //                  select h)
        //    .AsEnumerable()
        //    .GroupBy(h => new {
        //        Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
        //                   h.NgayXuatHd,
        //                   CalendarWeekRule.FirstFourDayWeek,
        //                   DayOfWeek.Monday),
        //        Year = h.NgayXuatHd.Year
        //    })
        //    .Select(g => new RevenueByWeekModel
        //    {
        //        Tuan = g.Key.Week,
        //        Nam= g.Key.Year,
        //        TongDoanhThu = g.Sum(x => x.TongGiaTri)
        //    }).ToList();

        //    return result;
        //}


        //[HttpGet]
        //public ActionResult RevenueByWeekExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+_localization.Getkey("DTTT"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("Week");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("Year");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("TongDoanhThu");
        //    ws.Range("A1:D1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = GetListRevenueByWeek();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].Tuan;
        //        ws.Cell("C" + row).Value = listData[i].Nam;
        //        ws.Cell("D" + row).Value = listData[i].TongDoanhThu;
        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DoanhThuTheoTuan_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public ActionResult RevenueByWeekPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = GetListRevenueByWeek();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>" + _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("Week") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("Year") + "</th>";
        //    htmlString += "<th>" + _localization.Getkey("TongDoanhThu") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.Tuan + "</td>";
        //        htmlString += "<td>" + item.Nam + "</td>";
        //        htmlString += "<td>" + item.TongDoanhThu + "</td></tr>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DoanhThuTheoTuan_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}


        //public List<RevenueByMonthModel> GetListRevenueByMonth()
        //{
        //    var result = (from h in _context.HoaDons
        //                  where h.TrangThaiThanhToan == 1
        //                  select h)
        //    .AsEnumerable()
        //    .GroupBy(h => new {
        //        Thang = h.NgayXuatHd.Month,
        //        Nam = h.NgayXuatHd.Year
        //    })
        //    .Select(g => new RevenueByMonthModel
        //    {
        //        Thang = g.Key.Thang,
        //        Nam = g.Key.Nam,
        //        TongDoanhThu = g.Sum(x => x.TongGiaTri)
        //    })
        //    .ToList();

        //    return result;
        //}

        //[HttpGet]
        //public ActionResult RevenueByMonthExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DTTMonth"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("Month");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("Year");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("TongDoanhThu");
        //    ws.Range("A1:D1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = GetListRevenueByMonth();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].Thang;
        //        ws.Cell("C" + row).Value = listData[i].Nam;
        //        ws.Cell("D" + row).Value = listData[i].TongDoanhThu;
        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DoanhThuTheoThang_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public ActionResult RevenueByMonthPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = GetListRevenueByMonth();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("Month") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("Year") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("TongDoanhThu") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.Thang+ "</td>";
        //        htmlString += "<td>" + item.Nam + "</td>";
        //        htmlString += "<td>" + item.TongDoanhThu + "</td></tr>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DoanhThuTheoThang_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //public List<RevenueByQuarterModel> GetListRevenueByQuarter()
        //{
        //    var result = (from h in _context.HoaDons
        //                  where h.TrangThaiThanhToan == 1
        //                  select h)
        //                .AsEnumerable()
        //                .GroupBy(h => new {
        //                    Quarter = (h.NgayXuatHd.Month - 1) / 3 + 1,
        //                    Year = h.NgayXuatHd.Year
        //                })
        //                .Select(g => new RevenueByQuarterModel
        //                {
        //                    Quy = g.Key.Quarter,
        //                    Nam = g.Key.Year,
        //                    TongDoanhThu = g.Sum(x => x.TongGiaTri)
        //                }).ToList();
        //    return result;
        //}

        //[HttpGet]
        //public ActionResult RevenueByQuarterExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add("" + _localization.Getkey("DTTQ"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("Quy");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("Year");
        //    ws.Cell("D1").Value = "" + _localization.Getkey("TongDoanhThu");
        //    ws.Range("A1:D1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = GetListRevenueByQuarter();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].Quy;
        //        ws.Cell("C" + row).Value = listData[i].Nam;
        //        ws.Cell("D" + row).Value = listData[i].TongDoanhThu;
        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DoanhThuTheoQuy_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public ActionResult RevenueByQuarterPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = GetListRevenueByQuarter();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("Quy") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("Year") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("TongDoanhThu") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.Quy+ "</td>";
        //        htmlString += "<td>" + item.Nam + "</td>";
        //        htmlString += "<td>" + item.TongDoanhThu + "</td></tr>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DoanhThuTheoQuy_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}


        //public List<RevenueByYearModel> GetListRevenueByYear()
        //{
        //    var result = (from h in _context.HoaDons
        //                  where h.TrangThaiThanhToan == 1
        //                  select h)
        //                .AsEnumerable()
        //                .GroupBy(h => h.NgayXuatHd.Year)
        //                .Select(g => new RevenueByYearModel
        //                {
        //                    Nam = g.Key,
        //                    TongDoanhThu = g.Sum(x => x.TongGiaTri)
        //                }).ToList();
        //    return result;
        //}

        //[HttpGet]
        //public ActionResult RevenueByYearExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add(""+ _localization.Getkey("DTTYear"));

        //    ws.Cell("A1").Value = "" + _localization.Getkey("STT");
        //    ws.Cell("B1").Value = "" + _localization.Getkey("Year");
        //    ws.Cell("C1").Value = "" + _localization.Getkey("TongDoanhThu");
        //    ws.Range("A1:C1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listData = GetListRevenueByYear();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listData.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listData[i].Nam;
        //        ws.Cell("C" + row).Value = listData[i].TongDoanhThu;
        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DoanhThuTheoNam_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public ActionResult RevenueByYearPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //    renderer.PrintOptions.MarginTop = 20;
        //    renderer.PrintOptions.MarginBottom = 20;
        //    renderer.PrintOptions.MarginLeft = 10;
        //    renderer.PrintOptions.MarginRight = 10;
        //    renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //    renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;

        //    var listData = GetListRevenueByYear();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>"+ _localization.Getkey("STT") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("Year") + "</th>";
        //    htmlString += "<th>"+ _localization.Getkey("TongDoanhThu") + "</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.Nam + "</td>";
        //        htmlString += "<td>" + item.TongDoanhThu + "</td></tr>";
        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DoanhThuTheoNam_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //    var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //    pdf.SaveAs(filepath);
        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}


        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonByKhachHangIdExcel()
        //{
        //    var wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add("Danh sách hóa đơn");

        //    ws.Cell("A1").Value = "STT";
        //    ws.Cell("B1").Value = "Mã hóa đơn";
        //    ws.Cell("C1").Value = "Ngày xuất hóa đơn";
        //    ws.Cell("D1").Value = "Ho và tên khách hàng";
        //    ws.Cell("E1").Value = "Tổng giá trị";
        //    ws.Cell("F1").Value = "Tên chương trình khuyến mãi";
        //    ws.Cell("G1").Value = "Tên phương thức thanh toán";
        //    ws.Cell("H1").Value = "Trạng thái thanh toán";
        //    ws.Cell("I1").Value = "Trạng thái đơn hàng";
        //    ws.Range("A1:I1").Style.Font.Bold = true;

        //    ws.Column(1).Width = 20;
        //    ws.Column(2).Width = 25;
        //    ws.Column(3).Width = 25;
        //    ws.Column(4).Width = 20;
        //    ws.Column(5).Width = 20;
        //    ws.Column(6).Width = 25;
        //    ws.Column(7).Width = 20;
        //    ws.Column(8).Width = 20;
        //    ws.Column(9).Width = 20;

        //    ws.Columns().AdjustToContents();
        //    ws.Rows().AdjustToContents();

        //    var listHD = await GetListHoaDon2();

        //    int row = 2;
        //    int stt = 1;
        //    for (int i = 0; i < listHD.Count(); i++)
        //    {
        //        ws.Cell("A" + row).Value = stt;
        //        ws.Cell("B" + row).Value = listHD[i].MaHD;
        //        ws.Cell("C" + row).Value = listHD[i].NgayXuatHD;
        //        ws.Cell("D" + row).Value = listHD[i].HoVaTenKH;
        //        ws.Cell("E" + row).Value = listHD[i].TongGiaTri;
        //        ws.Cell("F" + row).Value = listHD[i].TenCTKM;
        //        ws.Cell("G" + row).Value = listHD[i].TenPTTT;
        //        if (listHD[i].TrangThaiThanhToan == 1)
        //        {
        //            ws.Cell("H" + row).Value = "Thành công";
        //        }
        //        else
        //        {
        //            ws.Cell("H" + row).Value = "Thất bại";
        //        }

        //        if (listHD[i].TrangThaiDonHang == -1)
        //        {
        //            ws.Cell("I" + row).Value = "Đã hủy";
        //        }
        //        else if (listHD[i].TrangThaiDonHang == 0)
        //        {
        //            ws.Cell("I" + row).Value = "Đang chờ lấy hàng";
        //        }
        //        else if (listHD[i].TrangThaiDonHang == 1)
        //        {
        //            ws.Cell("I" + row).Value = "Đang giao hàng";
        //        }
        //        else
        //        {
        //            ws.Cell("I" + row).Value = "Giao hàng thành công";
        //        }


        //        ws.Columns().AdjustToContents();
        //        ws.Rows().AdjustToContents();

        //        row++;
        //        stt++;
        //    }

        //    string filename = "DSHD_" + DateTime.Now.Ticks + ".xlsx";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportExcel", filename);

        //    //string fileNamePath = Path.Combine(Directory.GetCurrentDirectory()); // tuong duong filepath
        //    //Console.WriteLine(fileNamePath);

        //    wb.SaveAs(filepath);

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filepath, FileMode.Open))
        //    {
        //        if (!Directory.Exists(Path.GetDirectoryName(filepath)))
        //        {
        //            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
        //        }
        //        stream.CopyTo(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        //}

        //[HttpGet]
        //public async Task<ActionResult> ExportHoaDonByKhachHangIdPdf()
        //{
        //    var renderer = new HtmlToPdf();
        //renderer.PrintOptions.MarginTop = 20;
        //renderer.PrintOptions.MarginBottom = 20;
        //renderer.PrintOptions.MarginLeft = 10;
        //renderer.PrintOptions.MarginRight = 10;
        //renderer.PrintOptions.PaperSize = IronPdf.PdfPrintOptions.PdfPaperSize.A4;
        //renderer.PrintOptions.PaperOrientation = IronPdf.Rendering.PdfPaperOrientation.Portrait;


        //    var listData = await GetListHoaDon2();

        //    string htmlString = "<table><thead><tr>";
        //    htmlString += "<th>STT</th>";
        //    htmlString += "<th>Mã hóa đơn</th>";
        //    htmlString += "<th>Ngày xuất hóa đơn</th>";
        //    htmlString += "<th>Họ và tên khách hàng</th>";
        //    htmlString += "<th>Tổng giá trị</th>";
        //    htmlString += "<th>Tên chương trình khuyến mãi</th>";
        //    htmlString += "<th>Tên phương thức thanh toán</th>";
        //    htmlString += "<th>Trạng thái thanh toán</th>";
        //    htmlString += "<th>Trạng thái đơn hàng</th>";
        //    htmlString += "</tr></thead><tbody>";
        //    int stt = 1;
        //    foreach (var item in listData)
        //    {
        //        htmlString += "<tr><td>" + stt + "</td>";
        //        htmlString += "<td>" + item.MaHD + "</td>";
        //        htmlString += "<td>" + item.NgayXuatHD + "</td>";
        //        htmlString += "<td>" + item.HoVaTenKH + "</td>";
        //        htmlString += "<td>" + item.TongGiaTri + "</td>";
        //        htmlString += "<td>" + item.TenCTKM + "</td>";
        //        htmlString += "<td>" + item.TenPTTT + "</td>";
        //        if (item.TrangThaiThanhToan == 1)
        //        {
        //            htmlString += "<td>Thành công</td></tr>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>Thất bại</td></tr>";
        //        }

        //        if (item.TrangThaiDonHang == -1)
        //        {
        //            htmlString += "<td>Đã hủy</td></tr>";
        //        }
        //        else if (item.TrangThaiDonHang == 0)
        //        {
        //            htmlString += "<td>Đang chờ lấy hàng</td></tr>";
        //        }
        //        else if (item.TrangThaiDonHang == 1)
        //        {
        //            htmlString += "<td>Đang giao hàng</td></tr>";
        //        }
        //        else
        //        {
        //            htmlString += "<td>Giao hàng thành công</td></tr>";
        //        }

        //        stt++;
        //    }

        //    htmlString += "</tbody></table>";

        //    string filename = "DSHD_" + DateTime.Now.Ticks + ".pdf";
        //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "Resource", "ExportPdf", filename);

        //    Console.WriteLine(filename);
        //var pdf = renderer.RenderHtmlAsPdf(htmlString);

        //pdf.SaveAs(filepath);

        //    // return file for download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
        //    return File(fileBytes, "application/pdf", filename);
        //}

        //public async Task<List<BillModel>> GetListHoaDon2()
        //{
        //    var result = (from a in _context.HoaDons
        //                  join b in _context.KhachHangs on a.KhachHangId equals b.Id into hdb
        //                  from b in hdb.DefaultIfEmpty()
        //                  join c in _context.ThanhToans on a.MaPttt equals c.MaPttt into pttt
        //                  from c in pttt.DefaultIfEmpty()
        //                  join d in _context.KhuyenMais on a.MaKm equals d.MaKm into km
        //                  from d in km.DefaultIfEmpty()
        //                  select new BillModel
        //                  {
        //                      MaHD = a.MaHoaDon,
        //                      NgayXuatHD = a.NgayXuatHd,
        //                      HoVaTenKH = b.HoKhachHang + " " + b.TenKhachHang,
        //                      TongGiaTri = a.TongGiaTri,
        //                      TenCTKM = d.TenKhuyenMai,
        //                      TenPTTT = c.TenPttt,
        //                      TrangThaiThanhToan = a.TrangThaiThanhToan,
        //                      TrangThaiDonHang = a.TrangThaiDonHang
        //                  }).ToListAsync();

        //    return await result;
        //}

    }
}
