using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Bill
{
    [Authorize(Roles = "Admin, Test")]

    public class EditModel : BillPageModel
    {
        public EditModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string? MaHD { get; set; }
            public DateTime NgayXuatHD { get; set; }
            public string? KhachHangID { get; set; }
            public double TongGiaTri { get; set; }
            public string? MaKM { get; set; }
            public string? MaPTTT { get; set; }

            [Required(ErrorMessage = "Trạng thái thanh toán là bắt buộc!")]
            public int TrangThaiThanhToan { get; set; }

            [Required(ErrorMessage = "Trạng thái đơn hàng là bắt buộc!")]
            public int TrangThaiDonHang { get; set; }
        }
        public HoaDon hoaDon { get; set; }
        public List<ChiTietHd> chiTietHoaDons { get; set; }
        public async Task<IActionResult> OnGetAsync(string billid)
        {
            if (billid == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }

            hoaDon = await _context.HoaDons.Where(x => x.MaHoaDon == billid).FirstOrDefaultAsync();

            if (hoaDon == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }
            else
            {
                Input = new InputModel()
                {
                    MaHD= hoaDon.MaHoaDon,
                    NgayXuatHD= hoaDon.NgayXuatHd,
                    KhachHangID= hoaDon.KhachHangId,
                    TongGiaTri= (double)hoaDon.TongGiaTri,
                    MaKM= hoaDon.MaKm,
                    MaPTTT= hoaDon.MaPttt,
                    TrangThaiThanhToan = hoaDon.TrangThaiThanhToan,
                    TrangThaiDonHang= hoaDon.TrangThaiDonHang
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string billid)
        {
            if (billid == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }

            hoaDon =await _context.HoaDons.Where(x => x.MaHoaDon == billid).FirstOrDefaultAsync();

            if (hoaDon == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }

            var oldTTDH = hoaDon.TrangThaiDonHang;
            var oldTTTT = hoaDon.TrangThaiThanhToan;
            if (oldTTDH == Input.TrangThaiDonHang && oldTTTT == Input.TrangThaiThanhToan)
            {
                //StatusMessage = ""+ _localization.Getkey("StatusNotChange");
                _notyf.Information(_localization.Getkey("StatusNotChange"), 3);
            }
            else
            {
                _context.Update(hoaDon);
                hoaDon.TrangThaiThanhToan = Input.TrangThaiThanhToan;
                hoaDon.TrangThaiDonHang = Input.TrangThaiDonHang;
                await _context.SaveChangesAsync();

                //StatusMessage = _localization.Getkey("UpdateSuccessWhen") + " " + DateTimeVN();
                _notyf.Success(_localization.Getkey("UpdateBillSuccess"), 3);

                if (Input.TrangThaiThanhToan == 1 && Input.TrangThaiDonHang == 2)
                {
                    var chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == billid).ToListAsync();

                    foreach (var chiTietHoaDon in chiTietHoaDons)
                    {
                        var gioHang = await _context.GioHangs
                            .Where(x => x.MaGioHang == chiTietHoaDon.MaGioHang)
                            .FirstOrDefaultAsync();

                        var sanPham = await _context.SanPhams
                            .Where(x => x.MaSanPham == gioHang.MaSanPham)
                            .FirstOrDefaultAsync();

                        sanPham.SoLuongDaBan += gioHang.SoLuongDat;
                        //sanPham.SoLuongTon -= gioHang.SoLuongDat;
                        _context.Update(sanPham);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToPage("./Index");
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
