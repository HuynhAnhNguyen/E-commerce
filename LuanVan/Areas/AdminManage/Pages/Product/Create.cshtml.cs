using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Product
{
    [Authorize(Roles = "Admin, Editor, Test")]

    public class CreateModel : ProductPageModel
    {
        public CreateModel(ApplicationDbContext context, INotyfService notyf, ILogger<ProductPageModel> logger, LanguageService localization) : base(context, notyf, logger, localization)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Tên sản phẩm là bắt buộc!")]
            [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải có độ dài từ 2 đến 255 ký tự!")]
            public string? TenSanPham { get; set; }

            [Required(ErrorMessage = "Tên đơn vị tính là bắt buộc!")]
            public string? TenDvt { get; set; }

            [Required(ErrorMessage = "Tên nhà sản xuất là bắt buộc!")]
            public string? MaNsx { get; set; }

            [Required(ErrorMessage = "Tên loại sản phẩm là bắt buộc!")]
            public string? MaLoaiSp { get; set; }

            [Required(ErrorMessage = "Màu sắc của sản phẩm là bắt buộc!")]
            public string? MaMau { get; set; }

            //[Required(ErrorMessage = "Hình ảnh là bắt buộc!")]
            public string? HinhAnh { get; set; }

            [Required(ErrorMessage = "Giá bán là bắt buộc!")]
            public long GiaBan { get; set; }

            [Required(ErrorMessage = "Trạng thái sản phẩm là bắt buộc!")]
            public int TrangThai { get; set; }

            [Required(ErrorMessage = "Số lượng tồn là bắt buộc!")]
            public int SoLuongTon { get; set; }

            [Required(ErrorMessage = "Mô tả của sản phẩm là bắt buộc!")]
            public string? MoTa { get; set; }

        }

        public List<LoaiSanPham> loaiSanPhams { get; set; }
        public List<MauSac> mauSacs { get; set; }
        public List<NhaSanXuat> nhaSanXuats { get; set; }
        public async Task OnGetAsync()
        {
            loaiSanPhams = await _context.LoaiSanPhams.ToListAsync();
            mauSacs = await _context.MauSacs.ToListAsync();
            nhaSanXuats = await _context.NhaSanXuats.ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            loaiSanPhams = await _context.LoaiSanPhams.ToListAsync();
            mauSacs = await _context.MauSacs.ToListAsync();
            nhaSanXuats = await _context.NhaSanXuats.ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var new_maSP = "" + DateTimeVN().ToString("ddMMyyyyHhmmss") + 1;
            SanPham sanPham = new SanPham();
            sanPham.MaSanPham = new_maSP;
            sanPham.TenSanPham = Input.TenSanPham;
            sanPham.TenDvt = Input.TenDvt;
            sanPham.MaNsx = Input.MaNsx;
            sanPham.MaLoaiSp = Input.MaLoaiSp;
            sanPham.MaMau = Input.MaMau;
            if (file != null && file.Length > 0)
            {
                sanPham.HinhAnh = await UploadImage(file);
            }
            //sanPham.HinhAnh = Input.HinhAnh;
            sanPham.GiaBan = Input.GiaBan;
            sanPham.SoLuongTon= Input.SoLuongTon;
            sanPham.TrangThai = Input.TrangThai;
            sanPham.MoTa = Input.MoTa;
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();


            _notyf.Success(_localization.Getkey("ThemSP") + " " + Input.TenSanPham + " " + _localization.Getkey("Thanhcong"), 3);
            //StatusMessage = _localization.Getkey("ThemSP") + " " + Input.TenSanPham + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();

            return RedirectToPage("./Index");

        }

        public async Task<string?> UploadImage(IFormFile image, string? path = null)
        {

            string[] permittedExtensions = { ".jpg", ".png" };
            var ext = Path.GetExtension("\\" + image.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return null;
            }

            string fileName = image.FileName;

            if (path == null)
            {
                path = Path.Combine("wwwroot\\images\\product", fileName);
            }
            else { path = Path.Combine(path, fileName); }

            using (var stream = System.IO.File.Create(path))
            {
                await image.CopyToAsync(stream);
            }
            return fileName;
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
