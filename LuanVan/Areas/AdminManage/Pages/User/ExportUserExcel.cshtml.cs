using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
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

    public class ExportUserExcelModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;
        private readonly ApplicationDbContext _context;

        public ExportUserExcelModel(
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
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("" + _localization.Getkey("DSUser"));

            ws.Cell("A1").Value = "" + _localization.Getkey("STT");
            ws.Cell("B1").Value = "" + _localization.Getkey("UserID");
            ws.Cell("C1").Value = "" + _localization.Getkey("UserLastname");
            ws.Cell("D1").Value = "" + _localization.Getkey("UserFirstname");
            ws.Cell("E1").Value = "" + _localization.Getkey("UserDateofbirth");
            ws.Cell("F1").Value = "" + _localization.Getkey("UserGender");
            ws.Cell("G1").Value = "" + _localization.Getkey("UserAddress");
            ws.Cell("H1").Value = "" + _localization.Getkey("UserUsername");
            ws.Cell("I1").Value = "" + _localization.Getkey("UserEmail");
            ws.Cell("J1").Value = "" + _localization.Getkey("UserPhoneNumber");
            ws.Range("A1:J1").Style.Font.Bold = true;

            ws.Column(1).Width = 20;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;

            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();

            var listData = await GetListUser();

            int row = 2;
            int stt = 1;
            for (int i = 0; i < listData.Count(); i++)
            {
                ws.Cell("A" + row).Value = stt;
                ws.Cell("B" + row).Value = listData[i].ID;
                ws.Cell("C" + row).Value = listData[i].HoKhachHang;
                ws.Cell("D" + row).Value = listData[i].TenKhachHang;
                ws.Cell("E" + row).Value = listData[i].NgaySinh;
                ws.Cell("F" + row).Value = listData[i].GioiTinh;
                ws.Cell("G" + row).Value = listData[i].DiaChi;
                ws.Cell("H" + row).Value = listData[i].UserName;
                ws.Cell("I" + row).Value = listData[i].Email;
                ws.Cell("J" + row).Value = listData[i].PhoneNumber;

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                row++;
                stt++;
            }

            string filename = "DSUser_" + DateTimeVN().Ticks + ".xlsx";
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
