using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Bill
{
    [Authorize(Roles = "Admin, Test, Editor")]

    public class DetailModel : BillPageModel
    {
        public DetailModel(ApplicationDbContext context, INotyfService notyf, ILogger<BillPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }
        public HoaDon hoaDon { get; set; }
        public List<ChiTietHd> chiTietHoaDons { get; set; }
        public string tenPhuongThucThanhToan { get; set; }
        public string khuyenMai { get; set; }

        public string path = "/images/product";
        public async Task<IActionResult> OnGetAsync(string billid )
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

            chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == billid).ToListAsync();

            if(chiTietHoaDons == null)
            {
                _notyf.Error(_localization.Getkey("KhongThayHoaDon") + " " + billid, 3);
                return RedirectToPage("./Index");
            }

            List<SanPham> sanPhams = new List<SanPham>();
            List<GioHang> gioHangs = new List<GioHang>();
            List<LoaiSanPham> loaiSanPhams = new List<LoaiSanPham>();

            foreach (var chiTietHoaDon in chiTietHoaDons)
            {
                GioHang gioHang = await _context.GioHangs.Where(x => x.MaGioHang == chiTietHoaDon.MaGioHang).FirstOrDefaultAsync();
                SanPham sanPham = await _context.SanPhams.Where(x => x.MaSanPham == gioHang.MaSanPham).FirstOrDefaultAsync();
                LoaiSanPham loaiSanPham = await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == sanPham.MaLoaiSp).FirstOrDefaultAsync();

                sanPhams.Add(sanPham);
                gioHangs.Add(gioHang);
                loaiSanPhams.Add(loaiSanPham);
            }

            ViewData["sanPhams"] = sanPhams;
            ViewData["gioHangs"] = gioHangs;
            ViewData["loaiSanPhams"] = loaiSanPhams;

            tenPhuongThucThanhToan = await (from a in _context.HoaDons join b in _context.ThanhToans on a.MaPttt equals b.MaPttt where a.MaHoaDon == billid select b.TenPttt).FirstOrDefaultAsync();

            if (!hoaDon.MaKm.IsNullOrEmpty())
            {
                khuyenMai = await (from a in _context.KhuyenMais
                                   join b in _context.HoaDons on a.MaKm equals b.MaKm
                                   where b.MaHoaDon == billid
                                   select a.TenKhuyenMai).FirstOrDefaultAsync();
            }
            else khuyenMai = _localization.Getkey("KAD");

            return Page();

        }

        public IActionResult OnPost() => RedirectToPage();
    }
}
