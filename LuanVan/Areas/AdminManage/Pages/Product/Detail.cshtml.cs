using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Product
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class DetailModel : ProductPageModel
    {
        
        public string path = "/images/product";

        public DetailModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        public SanPham sanPham { get; set; }

        public string mauSac { get; set; }
        public string loaiSanPham { get; set; }
        public string nhaSanXuat { get; set; }

        public string trangThai { get; set; }

        public async Task<IActionResult> OnGetAsync(string productid)
        {
            if (productid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThaySP") + " " + productid, 3);
                return RedirectToPage("./Index");
            }

            sanPham =await _context.SanPhams.Where(x => x.MaSanPham == productid).FirstOrDefaultAsync();

            if (sanPham == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThaySP") + " " + productid, 3);
                return RedirectToPage("./Index");
            }

            mauSac = await (from s in _context.SanPhams join ms in _context.MauSacs on s.MaMau equals ms.MaMau where s.MaSanPham == productid select ms.TenMau).FirstOrDefaultAsync();
            loaiSanPham = await (from s in _context.SanPhams join lsp in _context.LoaiSanPhams on s.MaLoaiSp equals lsp.MaLoaiSp where s.MaSanPham == productid select lsp.TenLoaiSp).FirstOrDefaultAsync();
            nhaSanXuat = await (from s in _context.SanPhams join nsx in _context.NhaSanXuats on s.MaNsx equals nsx.MaNsx where s.MaSanPham == productid select nsx.TenNsx).FirstOrDefaultAsync();

            if (sanPham.TrangThai.Equals(-1))
            {
                trangThai = _localization.Getkey("KKD");
            } else if (sanPham.TrangThai.Equals(0)){
                trangThai = _localization.Getkey("HH");
            }
            else if (sanPham.TrangThai.Equals(1)){
                trangThai = _localization.Getkey("KM");
            } else
                trangThai = _localization.Getkey("HOT");


            return Page();

        }

        public IActionResult OnPost() => RedirectToPage();

    }
}
