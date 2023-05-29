using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using X.PagedList;
using LuanVan.Data;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.InkML;

namespace LuanVan.Models
{
    public class Service
    {
        private ApplicationDbContext _context = new ApplicationDbContext();


        public (SanPham[] sanphams, int pages, int page) Paging(int page)
        {
            int size = 2;
            int pages = (int)Math.Ceiling((double)_context.SanPhams.Count() / size);
            var sanPhams = _context.SanPhams.Skip((page - 1) * size).Take(size).ToArray();
            return (sanPhams, pages, page);
        }

        public DateTime DateTimeVN()
        {
            DateTime utcTime = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Lấy thông tin về múi giờ của Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamZone); // Chuyển đổi giá trị DateTime từ múi giờ UTC sang múi giờ của Việt Nam

            return vietnamTime;
        }

        // Loại sản phẩm
        // Danh sách loại sản phẩm
        public IQueryable<LoaiSanPham> danhSachLoaiSP()
        {
            return _context.LoaiSanPhams.Where(x => x.MaLoaiSp != null);
        }

        // Lấy loại sản phẩm
        //public async Task<LoaiSanPham?> getLoaiSP(string maLoai)
        //{
        //    return await danhSachLoaiSP().Where(x => x.MaLoaiSp == maLoai).FirstOrDefaultAsync();
        //}

        public async Task<string> getTenLoaiSP(string maLoai)
        {
            return await _context.LoaiSanPhams.Where(x => x.MaLoaiSp == maLoai).Select(x => x.TenLoaiSp).FirstOrDefaultAsync();
        }

        // Chỉnh sửa loại sản phẩm
        //public async Task suaLoaiSanPham(ProductTypeModel loaiSP)
        //{
        //    LoaiSanPham loaiSanPham = await getLoaiSP(loaiSP.MaLoaiSP);
        //    _context.Update(loaiSanPham);
        //    loaiSanPham.TenLoaiSp = loaiSP.TenLoaiSP;
        //    _context.SaveChanges();
        //}

        // Thêm loại sản phẩm
        //public async Task themLoaiSP(ProductTypeModel loaiSP)
        //{
        //    var new_maloai = "" + DateTimeVN().ToString("ddmmyyyyHHmmss") + 1;
        //    LoaiSanPham LoaiSP = new LoaiSanPham();
        //    LoaiSP.MaLoaiSp = new_maloai;
        //    LoaiSP.TenLoaiSp = loaiSP.TenLoaiSP;
        //    await _context.LoaiSanPhams.AddAsync(LoaiSP);
        //    await _context.SaveChangesAsync();
        //}

        // Xóa loại sản phẩm
        //public async Task<int> xoaLoaiSP(string maLoaiSP)
        //{
        //    var khoaChinh = await _context.SanPhams.Where(x => x.MaLoaiSp == maLoaiSP).ToListAsync();
        //    if (khoaChinh.Any())
        //    {
        //        return 0;
        //    }
        //    _context.LoaiSanPhams.Remove(await _context.LoaiSanPhams.FindAsync(maLoaiSP));
        //    await _context.SaveChangesAsync();
        //    return 1;
        //}

        // Giỏ hàng
        // Danh sách giỏ hàng
        //public IQueryable<GioHang> danhSachGioHang(int tt = 2, string? maKhachHang = null)
        //{
        //    var rs = _context.GioHangs.Where(x => x.KhachHangId == maKhachHang);
        //    if (tt == 2)
        //    {
        //        return _context.GioHangs.Where(x => x.MaGioHang != null).Include(x => x.SanPham);
        //    }

        //    return rs.Where(x => x.TrangThai == tt).Include(x => x.SanPham);
        //}


        public IQueryable<GioHang> danhSachGioHang(int tt = 2, string? maKhachHang = null)
        {
            var rs = _context.GioHangs.Where(x => x.KhachHangId == maKhachHang && x.SanPham.TrangThai != -1 && x.SanPham.TrangThai != 0 && x.SanPham.SoLuongTon > 0);
            if (tt == 2)
            {
                return rs.Where(x => x.MaGioHang != null).Include(x => x.SanPham);
            }

            return rs.Where(x => x.TrangThai == tt).Include(x => x.SanPham);
        }



        // Lấy giỏ hàng
        public async Task<GioHang> getGioHang(string maGioHang)
        {
            if (!string.IsNullOrEmpty(maGioHang))
            {
                return await _context.GioHangs.Where(x => x.MaGioHang == maGioHang).Include(x => x.SanPham).FirstOrDefaultAsync();
            }
            return null;
        }


        public async Task<string> themGioHang(string maSP, string? maKH = null)
        {
            string maGH = "GH" + DateTimeVN().ToString("ddMMyyyyHHmmss") + 3;
            var rs = danhSachGioHang(0, maKH).Where(x => x.MaSanPham == maSP);
            if (rs.Any())
            {
                var model = rs.FirstOrDefault();
                _context.Update(model);
                model.SoLuongDat = model.SoLuongDat + 1;
                await _context.SaveChangesAsync();
                return model.MaGioHang;
            }

            GioHang gh = new GioHang();
            gh.MaGioHang = maGH;
            gh.KhachHangId = maKH;
            if (maKH == null)
            {
                gh.TrangThai = 2;
            }
            else
            {
                gh.TrangThai = 0;
            }
            gh.SoLuongDat = 1;
            gh.MaSanPham = maSP;
            _context.GioHangs.Add(gh);
            await _context.SaveChangesAsync();
            return gh.MaGioHang;
        }

        // Xóa đơn đặt
        public async Task xoaGioHang(string maGH)
        {
            if (!string.IsNullOrEmpty(maGH))
            {
                var model = _context.GioHangs.Find(maGH);
                _context.Update(model);
                if (model.TrangThai == 0 || model.TrangThai == 2)
                {
                    model.TrangThai = -1;
                }
                await _context.SaveChangesAsync();
            }
        }

        // Sản phẩm
        // Danh sách sản phẩm
        public IQueryable<SanPham> danhSachSanPham()
        {
            //return (from s in _context.SanPhams where s.TrangThai != -1 || s.TrangThai != 0 select s);
            //return _context.SanPhams.Where(x => x.MaSanPham != null).Where(x=> x.TrangThai==1).Where(x=> x.TrangThai==2);
            //return _context.SanPhams.Where(x => x.TrangThai == 1 || x.TrangThai == 2);
            //return _context.SanPhams.Where(s => s.TrangThai != -1 && s.TrangThai != 0);
            return (from a in _context.SanPhams
                    where (a.TrangThai == 1 || a.TrangThai == 2) && a.SoLuongTon > 0
                    orderby a.TenSanPham ascending
                    select a);

        }

        public IQueryable<SanPham> danhSachSanPhamHot()
        {
            return _context.SanPhams.Where(x => x.MaSanPham != null).Where(x => x.TrangThai == 2).Where(x => x.SoLuongTon > 0);
        }

        public IQueryable<SanPham> danhSachSanPhamKhuyenMai()
        {
            return _context.SanPhams.Where(x => x.MaSanPham != null).Where(x => x.TrangThai == 1).Where(x => x.SoLuongTon > 0);
        }

        public IQueryable<SanPham> danhSachSanPhamBanChay()
        {
            var result = (_context.SanPhams
                            .Where(x => x.SoLuongDaBan > 0 && x.TrangThai != -1)
                            .Where(x => x.SoLuongTon > 0)
                            .OrderByDescending(sp => sp.SoLuongDaBan));
            return result;
        }


        // Danh sách sản phẩm theo loại
        //public IQueryable<SanPham> danhSachSanPham(string maLoai = null)
        //{
        //    return (from s in _context.SanPhams where s.TrangThai == 1 && s.MaLoaiSp == maLoai || s.TrangThai == 2 && s.MaLoaiSp == maLoai select s);

        //    //return _context.SanPhams.Where(x => x.MaSanPham != null).Where(x => x.MaLoaiSp == maLoai);
        //}

        public IQueryable<SanPham> danhSachSanPham(string maLoai)
        {
            return _context.SanPhams.Where(x => (x.TrangThai == 1 && x.MaLoaiSp == maLoai) ||
                                                 (x.TrangThai == 2 && x.MaLoaiSp == maLoai));
        }


        // Lấy sản phẩm
        public async Task<SanPham?> getSanPham(string maSanPham)
        {
            return await danhSachSanPham().Where(x => x.MaSanPham == maSanPham).FirstOrDefaultAsync();
        }

        // Lấy sản phẩm
        public async Task<SanPham?> GetSanPham(string maSanPham)
        {
            return await _context.SanPhams.FindAsync(maSanPham);
        }

        // Xóa sản phẩm
        //public async Task xoaSanPham(string maSP)
        //{
        //    if (!string.IsNullOrEmpty(maSP))
        //    {
        //        var model = _context.SanPhams.Find(maSP);
        //        _context.SanPhams.Remove(model);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        // Sửa sản phẩm
        //public async Task suaSanPham(SanPham sanPham)
        //{
        //    var sP = getSanPham(sanPham.MaSanPham);
        //    _context.Update(sP);
        //    await _context.SaveChangesAsync();
        //}

        // Thêm sản phẩm
        //public void themSanPham(SanPham sanPham)
        //{
        //    _context.SanPhams.Add(sanPham);
        //    _context.SaveChanges();
        //}

        //public async Task themSanPham(ProductModel model)
        //{
        //    string maSP = "" + DateTimeVN().ToString("ddMMyyyyHHmmss") + 1;
        //    SanPham sanPham = new SanPham();
        //    sanPham.MaSanPham = maSP;
        //    sanPham.TenSanPham = model.TenSanPham;
        //    sanPham.TenDvt = model.TenDvt;
        //    sanPham.MaNsx = model.MaNsx;
        //    sanPham.MaLoaiSp = model.MaLoaiSp;
        //    sanPham.MaMau = model.MaMau;
        //    sanPham.HinhAnh = model.HinhAnh;
        //    sanPham.GiaBan = model.GiaBan;
        //    sanPham.SoLuongTon = model.SoLuongTon;
        //    sanPham.TrangThai= model.TrangThai;
        //    sanPham.MoTa = model.MoTa;
        //    _context.SanPhams.Add(sanPham);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task<string?> UploadImage(IFormFile image, string? path = null)
        //{

        //    string[] permittedExtensions = { ".jpg", ".png" };
        //    var ext = Path.GetExtension("\\" + image.FileName).ToLowerInvariant();
        //    if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
        //    {
        //        return null;
        //    }

        //    string fileName = image.FileName;

        //    if (path == null)
        //    {
        //        path = Path.Combine("wwwroot\\images\\product", fileName);
        //    }
        //    else { path = Path.Combine(path, fileName); }

        //    using (var stream = System.IO.File.Create(path))
        //    {
        //        await image.CopyToAsync(stream);
        //    }
        //    return fileName;
        //}

        //public async Task suaSanPham(SanPham sanPham, IFormFile file)
        //{
        //    var sp = await getSanPham(sanPham.MaSanPham);
        //    _context.Update(sp);
        //    if (file != null)
        //    {
        //        var path = getDataPath(file.FileName);
        //        using var stream = new FileStream(path, FileMode.Create);
        //        file.CopyTo(stream);
        //        sp.HinhAnh = file.FileName;
        //    }

        //    await _context.SaveChangesAsync();
        //}

        // Tìm kiếm sản phẩm
        public IQueryable<SanPham> timKiem(string key)
        {
            return danhSachSanPham().Where(x => x.TenSanPham.Contains(key));
        }

        // Nhà sản xuất
        // Danh sách nhà sản xuất
        //public IQueryable<NhaSanXuat> danhSachNSX()
        //{
        //    return _context.NhaSanXuats.Where(x => x.MaNsx != null);
        //}

        // Lấy nhà sản xuất
        //public async Task<NhaSanXuat> getNSX(string maNSX)
        //{
        //    return await danhSachNSX().Where(x => x.MaNsx == maNSX).FirstOrDefaultAsync();
        //}

        // Màu sắc 
        //public IQueryable<MauSac> danhsachMauSac()
        //{
        //    return _context.MauSacs.Where(x => x.MaMau != null);
        //}

        // Lấy màu sắc
        //public async Task<MauSac> getMauSac(string maMau)
        //{
        //    return await danhsachMauSac().Where(x => x.MaMau == maMau).FirstOrDefaultAsync();
        //}

        //Khách hàng
        //Danh sách khách hàng
        //public IQueryable<KhachHang> danhSachKH()
        //{
        //    return _context.KhachHangs;
        //}


        ////Lấy khách hàng
        public async Task<KhachHang> getKH(string maKH)
        {
            return await _context.KhachHangs.FindAsync(maKH);
        }


        ////Lấy khách hàng
        //public KhachHang GetKH(string maKH)
        //{
        //    return _context.Users.Find(maKH);
        //}


        // Thêm khách hàng ( Register)
        //public string themKH(RegisterModel model)
        //{
        //    string maKH = "" + (DateTimeVN().ToString("ddMMyyyyHHmmss"));
        //    Console.WriteLine(maKH);
        //    KhachHang kh = new KhachHang();
        //    kh.MaKhachHang = maKH;
        //    kh.HoKhachHang = model.HoKhachHang;
        //    kh.TenKhachHang = model.TenKhachHang;
        //    kh.NgaySinh = model.NgaySinh;
        //    kh.GioiTinh = model.GioiTinh;
        //    kh.SoDienThoai = model.SoDienThoai;
        //    kh.Email = model.Email;
        //    kh.DiaChi = model.DiaChi;
        //    kh.MatKhau = getMD5(model.MatKhau);
        //    kh.TrangThai = 1;
        //    _context.KhachHangs.Add(kh);
        //    _context.SaveChanges();
        //    return maKH;
        //}

        // Sửa khách hàng
        //public void suaKhachHang(KhachHang khachHang)
        //{
        //    var kH = getKH(khachHang.MaKhachHang);
        //    _context.Update(kH);
        //    kH.HoKhachHang = khachHang.HoKhachHang;
        //    kH.TenKhachHang = khachHang.TenKhachHang;
        //    kH.NgaySinh = khachHang.NgaySinh;
        //    kH.GioiTinh = khachHang.GioiTinh;
        //    kH.SoDienThoai = khachHang.SoDienThoai;
        //    kH.DiaChi = khachHang.DiaChi;
        //    kH.Email = khachHang.Email;
        //    kH.MatKhau = getMD5(khachHang.MatKhau);
        //    kH.TrangThai = khachHang.TrangThai;

        //    _context.SaveChanges();

        //}

        // Sửa trạng thái khách hàng
        //public void suaTTKhachHang(string maKH, string trangThai)
        //{
        //    var kH = GetKH(maKH);
        //    _context.Update(kH);
        //    kH.TrangThai = int.Parse(trangThai);
        //    _context.SaveChanges();

        //}

        //public void updateMatKhau(string maKH, string password)
        //{
        //    var kH = GetKH(maKH);
        //    _context.Update(kH);
        //    kH.MatKhau = getMD5(password);
        //    _context.SaveChanges();

        //}
        //// Khách hàng login
        //public KhachHang? loginKH(string input, string pwd)
        //{
        //    if (IsValidEmail(input))
        //    {
        //        return danhSachKH().Where(x => x.Email == input).Where(x => x.MatKhau == getMD5(pwd)).FirstOrDefault();
        //    }
        //    else
        //        return danhSachKH().Where(x => x.SoDienThoai == input).Where(x => x.MatKhau == getMD5(pwd)).FirstOrDefault();
        //}

        //public bool IsValidPhoneNumber(string input)
        //{
        //    return Regex.IsMatch(input, @"^\d{10,11}$");
        //}

        //public bool IsValidEmail(string input)
        //{
        //    return Regex.IsMatch(input, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
        //}

        // lấy mã phương thức thanh toán của hóa đơn
        public async Task<string> getMaPTTT(string maHD)
        {
            var result = _context.HoaDons
                .Where(hd => hd.MaHoaDon == maHD)
                .Select(hd => hd.MaPttt)
                .FirstOrDefaultAsync();
            return await result;
        }


        //Sửa trạng thái thanh toán
        public async Task suaTrangThaiThanhToan(string maHD, int trangThai)
        {
            var hoaDon = await getHoaDon(maHD);
            _context.Update(hoaDon);
            hoaDon.TrangThaiThanhToan = trangThai;
            await _context.SaveChangesAsync();
        }

        public async Task huyDonHang(string maHD)
        {
            var hoaDon = await getHoaDon(maHD);
            _context.Update(hoaDon);
            hoaDon.TrangThaiDonHang = -1;
            await _context.SaveChangesAsync();
        }

        public async Task suaTrangThaiDonHang(string maHD, int trangThai)
        {
            var hoaDon = await getHoaDon(maHD);
            _context.Update(hoaDon);
            hoaDon.TrangThaiDonHang = trangThai;
            await _context.SaveChangesAsync();
        }


        //public int getTrangThaiHD(string maHD)
        //{
        //    return 
        //}

        // Lấy khách hàng by email
        //public string GetKHByEmail(string input)
        //{
        //    var result = (from kh in _context.KhachHangs
        //              where kh.Email == input
        //              select kh.MaKhachHang).FirstOrDefault();
        //    return result;
        //}

        // Lấy khách hàng by phone
        //public string GetKHByPhone(string input)
        //{
        //    var result = (from kh in _context.KhachHangs
        //                  where kh.SoDienThoai == input
        //                  select kh.MaKhachHang).FirstOrDefault();
        //    return result;
        //}


        // Hóa dơn
        // Danh sách hóa đơn
        public IQueryable<HoaDon> danhSachHoaDon(string maKH = null)
        {
            if (maKH == null)
            {
                return _context.HoaDons.Where(x => x.MaHoaDon != null).Include(x => x.ChiTietHds);
            }
            return _context.HoaDons.Where(x => x.KhachHangId == maKH).Include(x => x.ChiTietHds);
        }

        // Lấy hóa đơn
        public async Task<HoaDon> getHoaDon(string maHD)
        {
            return await danhSachHoaDon().Where(x => x.MaHoaDon == maHD).FirstOrDefaultAsync();
        }

        public async Task<string> GetMaKM(string mahd)
        {
            var maKM = await _context.HoaDons
                .Where(a => a.MaHoaDon == mahd)
                .Select(a => a.MaKm)
                .FirstOrDefaultAsync();

            return maKM;
        }

        public async Task<KhuyenMai> GetKhuyenMai(string makm)
        {
            var query = await _context.KhuyenMais
                .Where(a => a.MaKm == makm)
                .FirstOrDefaultAsync();
            return query;
        }

        public async Task TangHangTon(string orderId)
        {
            var chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == orderId).ToListAsync();

            foreach (var chiTietHoaDon in chiTietHoaDons)
            {
                var gioHang = await _context.GioHangs.FirstOrDefaultAsync(x => x.MaGioHang == chiTietHoaDon.MaGioHang);
                var sanPham = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == gioHang.MaSanPham);

                if (sanPham != null && gioHang != null)
                {
                    sanPham.SoLuongTon += gioHang.SoLuongDat;
                    _context.Update(sanPham);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task GiamHangTon(string orderId)
        {
            var chiTietHoaDons = await _context.ChiTietHds.Where(x => x.MaHoaDon == orderId).ToListAsync();

            foreach (var chiTietHoaDon in chiTietHoaDons)
            {
                var gioHang = await _context.GioHangs.FirstOrDefaultAsync(x => x.MaGioHang == chiTietHoaDon.MaGioHang);
                var sanPham = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == gioHang.MaSanPham);

                if (sanPham != null && gioHang != null)
                {
                    sanPham.SoLuongTon -= gioHang.SoLuongDat;
                    _context.Update(sanPham);
                }
            }

            await _context.SaveChangesAsync();
        }

        // Thêm hóa đơn
        //public string themHoaDon(string maKH = "", string thanhtoan = "")
        //{
        //    HoaDon hoaDon = new HoaDon();
        //    hoaDon.MaHoaDon = "" + DateTimeVN().Ticks.ToString();
        //    if (maKH == "")
        //    {
        //        hoaDon.KhachHangId = null;
        //    }
        //    else
        //    {
        //        hoaDon.KhachHangId = maKH;
        //    }
        //    switch (thanhtoan)
        //    {
        //        //case "cod":
        //        //    {
        //        //        hoaDon.MaPttt = "cod";
        //        //        break;
        //        //    }
        //        case "vnpay":
        //            {
        //                hoaDon.MaPttt = "vnpay";
        //                break;
        //            }
        //        //default: hoaDon.MaPttt = "cod"; break;
        //    }
        //    hoaDon.NgayXuatHd = DateTimeVN();
        //    _context.HoaDons.Add(hoaDon);
        //    _context.SaveChanges();
        //    return hoaDon.MaHoaDon;
        //}

        // Chi tiết hóa đơn
        // Danh sách chi tiết hóa đơn
        //public IQueryable<ChiTietHd> danhSachChiTietHD(string maHD = null)
        //{
        //    if (maHD == null)
        //    {
        //        return _context.ChiTietHds.Where(x => x.MaChiTietHd != null);
        //    }
        //    return _context.ChiTietHds.Where(x => x.MaHoaDon == maHD);
        //}

        // Thêm chi tiết hóa đơn
        public async Task<string> themChiTietHD(string maHD, string maGH)
        {
            ChiTietHd chiTietHD = new ChiTietHd();
            chiTietHD.MaChiTietHd = "" + DateTimeVN().Ticks.ToString() + 3;
            chiTietHD.MaGioHang = maGH;
            var gioHang = await getGioHang(maGH);
            gioHang.TrangThai = 1;
            //await getGioHang(maGH).TrangThai = 1;
            chiTietHD.MaHoaDon = maHD;
            _context.ChiTietHds.Add(chiTietHD);
            await _context.SaveChangesAsync();
            return chiTietHD.MaChiTietHd;
        }

        // Tăng số lượng đơn đặt
        public async Task increase(string maGH, int soLuong)
        {
            var gioHang = await getGioHang(maGH);
            _context.Update(gioHang);
            gioHang.SoLuongDat = soLuong;

            await _context.SaveChangesAsync();
        }

        // Nhân viên
        // Danh sách nhân viên
        //public IQueryable<NhanVien> danhSachNhanVien()
        //{
        //    return _context.NhanViens.Where(x => x.MaNhanVien != null);
        //}

        //// Lấy nhân viên
        //public NhanVien? getNV(string maNV)
        //{
        //    return danhSachNhanVien().Where(x => x.MaNhanVien == maNV).FirstOrDefault();
        //}

        //// Lấy nhân viên
        //public NhanVien getNhanVien(string maNV)
        //{
        //    return _context.NhanViens.Find(maNV);
        //}

        //// Thêm nhân viên
        //public void themNV(string hoNV, string tenNV, DateTime ngaySinh, string gioiTinh, string soDT, string diaChi, string email, string matKhau)
        //{

        //    NhanVien nV = new NhanVien();
        //    string maNV = "" + (DateTimeVN().ToString("ddMMyyyyHHmmss") + 2);
        //    nV.MaNhanVien = maNV;
        //    nV.HoNhanVien = hoNV;
        //    nV.TenNhanVien = tenNV;
        //    nV.NgaySinh = ngaySinh;
        //    nV.GioiTinh = gioiTinh;
        //    nV.SoDienThoai = soDT;
        //    nV.DiaChi = diaChi;
        //    nV.Email = email;
        //    nV.MatKhau = getMD5(matKhau);
        //    nV.TrangThai = 1;
        //    nV.MaRole = "1";
        //    _context.NhanViens.Add(nV);
        //    _context.SaveChanges(true);
        //}

        //// Xóa nhân viên
        //public void xoaNhanVien(string maNV)
        //{
        //    if (!string.IsNullOrEmpty(maNV))
        //    {
        //        var model = _context.NhanViens.Find(maNV);
        //        _context.NhanViens.Remove(model);
        //        _context.SaveChanges();
        //    }
        //}

        //// Sửa nhân viên
        //public void suaNhanVien(NhanVien nhanVien)
        //{
        //    var nV = getNV(nhanVien.MaNhanVien);
        //    _context.Update(nV);
        //    nV.HoNhanVien = nhanVien.HoNhanVien;
        //    nV.TenNhanVien = nhanVien.TenNhanVien;
        //    nV.NgaySinh = nhanVien.NgaySinh;
        //    nV.GioiTinh = nhanVien.GioiTinh;
        //    nV.SoDienThoai = nhanVien.SoDienThoai;
        //    nV.DiaChi = nhanVien.DiaChi;
        //    nV.Email = nhanVien.Email;
        //    nV.MatKhau = getMD5(nhanVien.MatKhau);
        //    nV.TrangThai = nhanVien.TrangThai;
        //    nV.MaRole = nhanVien.MaRole;

        //    _context.SaveChanges();

        //}

        //// Nhân viên (Đăng nhập)
        //public NhanVien? loginNV(string input, string matKhau)
        //{
        //    if (IsValidEmail(input))
        //    {
        //        return danhSachNhanVien().Where(x => x.Email == input).Where(x => x.MatKhau == getMD5(matKhau)).FirstOrDefault();
        //    }
        //    else
        //        return danhSachNhanVien().Where(x => x.SoDienThoai == input).Where(x => x.MatKhau == getMD5(matKhau)).FirstOrDefault();
        //}

        // Lấy đường dẫn
        //public string getDataPath(string file) => $"wwwroot\\images\\product\\{file}";


        //// Tổng giá trị hóa đơn
        //public long tongGiaTri(string maHD)
        //{
        //    long sum = 0;
        //    var dds = danhSachChiTietHD(maHD).ToList();
        //    foreach (var dd in dds)
        //    {
        //        sum += dd.GioHang.SoLuongDat * dd.GioHang.SanPham.GiaBan;
        //    }
        //    return sum;
        //}

        //public async Task<string> tenPTTT(string maHD)
        //{
        //    var result = (from a in _context.ThanhToans
        //                  join b in _context.HoaDons on a.MaPttt equals b.MaPttt
        //                  where b.MaHoaDon == maHD
        //                  select a.TenPttt).FirstOrDefaultAsync();
        //    return await result;
        //}
        //// Mã hóa mật khẩu MD5
        //public static string getMD5(string password)
        //{
        //    MD5 mD5 = new MD5CryptoServiceProvider();
        //    byte[] fromData = Encoding.UTF8.GetBytes(password);
        //    byte[] targetData = mD5.ComputeHash(fromData);

        //    string byte2String = null;
        //    for (int i = 0; i < targetData.Length; i++)
        //    {
        //        byte2String += targetData[i].ToString("x2");
        //    }
        //    return byte2String;
        //}


        //private static string _otp;
        //public string createOTP()
        //{
        //    // Generate OTP
        //    var random = new Random();
        //    _otp = random.Next(100000, 1000000).ToString();

        //    //Start timer for OTP expiration

        //    return _otp;
        //}


        //public int getTrangThai(string maKH)
        //{
        //    //var result = (from c in KhachHang
        //    //              where c.Email == email
        //    //              select c.TrangThai).FirstOrDefault();
        //    //return result;

        //    var trangThai = (from kh in _context.KhachHangs
        //                     where kh.MaKhachHang == maKH
        //                     select kh.TrangThai).FirstOrDefault();

        //    return trangThai;

        //}


        public async Task<string?> getNhaSXBySanPham(string maSP)
        {


            var result = (from a in _context.SanPhams
                          join b in _context.NhaSanXuats on a.MaNsx equals b.MaNsx
                          where a.MaSanPham == maSP
                          select b.TenNsx).FirstOrDefaultAsync();


            return await result;

        }



        public async Task<string?> getMauSacBySanPham(string maSP)
        {


            var result = (from a in _context.SanPhams
                          join b in _context.MauSacs on a.MaMau equals b.MaMau
                          where a.MaSanPham == maSP
                          select b.TenMau).FirstOrDefaultAsync();


            return await result;

        }

        public async Task<string?> getMotaBySanPham(string maSP)
        {


            var result = (from a in _context.SanPhams
                          where a.MaSanPham == maSP
                          select a.MoTa).FirstOrDefaultAsync();
            return await result;

        }

        public async Task<IPagedList<SanPham>> PagingSortProductByName(string input, bool isAscending, int page, int size)
        {
            var result = (from a in _context.SanPhams
                          where a.MaLoaiSp == input
                          orderby a.TenSanPham ascending
                          select a).ToPagedListAsync(page, size);
            if (!isAscending) // Nếu isAscending là false thì sắp xếp giảm dần
            {
                result = (from a in _context.SanPhams
                          where a.MaLoaiSp == input
                          orderby a.TenSanPham descending
                          select a).ToPagedListAsync(page, size);
            }
            return await result;
        }


        public async Task<IPagedList<SanPham>> PagingProductByLoaiSP(string maloai, int page, int size)
        {
            Task<IPagedList<SanPham>> result;
            Console.WriteLine("MALOAI:"+ maloai);
            if (maloai.Equals(""))
            {
                result = danhSachSanPham().ToPagedListAsync(page, size);
            }
            else
            {
                result = danhSachSanPham(maloai).ToPagedListAsync(page, size);

            }

            return await result;
        }

        //public async Task<IPagedList<SanPham>> PagingProduct(int page, int size, string maloai= null)
        //{
        //    var result = danhSachSanPham().ToPagedListAsync(page, size);

        //    return await result;
        //}

        public async Task<IPagedList<SanPham>> PagingSortProductByPrice(string input, bool isAscending, int page, int size)
        {

            var result = (from a in _context.SanPhams
                          where a.MaLoaiSp == input
                          orderby a.GiaBan ascending
                          select a).ToPagedListAsync(page, size);
            if (!isAscending) // Nếu isAscending là false thì sắp xếp giảm dần
            {
                result = (from a in _context.SanPhams
                          where a.MaLoaiSp == input
                          orderby a.GiaBan descending
                          select a).ToPagedListAsync(page, size);
            }
            return await result;
        }

        //public async Task<List<string>> GetEmailListFromDB()
        //{
        //    using (var db = new ApplicationDbContext())
        //    {
        //        return  await db.KhachHangs
        //                 .Select(c => c.Email)
        //                 .ToListAsync();
        //    }
        //}

        //public async Task<List<string>> GetSdtListFromDB()
        //{
        //    using (var db = new ApplicationDbContext())
        //    {
        //        return await db.KhachHangs
        //                 .Select(c => c.PhoneNumber)
        //                 .ToListAsync();
        //    }
        //}

        //public async Task<List<string>> GetUsernameListFromDB()
        //{
        //    using (var db = new ApplicationDbContext())
        //    {
        //        return await db.KhachHangs
        //            .Select(c => c.UserName)
        //                 .ToListAsync();
        //    }
        //}

        //public async Task<string> GetUsernameByEmail(string email)
        //{
        //    var username = (from khachhang in _context.KhachHangs
        //                    where khachhang.Email == email
        //                    select khachhang.UserName).FirstOrDefaultAsync();
        //    return await username;

        //}

        //public async Task<string> GetUsernameByPhone(string phone)
        //{
        //    var username = (from khachhang in _context.KhachHangs
        //                    where khachhang.PhoneNumber == phone
        //                    select khachhang.UserName).FirstOrDefaultAsync();
        //    return await username;

        //}

        //public async Task<KhachHang> GetKhachHangByEmail(string email)
        //{
        //    return await _context.KhachHangs.Where(x => x.Email == email).FirstOrDefaultAsync();
        //}

        //public void sendMail(string noiDung, string mailTo, string tieuDe)
        //{
        //    //gui email

        //    MailMessage mailMessage = new MailMessage();
        //    mailMessage.From = new MailAddress("anhB1910186@student.ctu.edu.vn");

        //    //var kh = _service.getKH(model.MaKhachHang);
        //    mailMessage.To.Add(new MailAddress(mailTo));

        //    mailMessage.Subject = tieuDe;

        //    mailMessage.IsBodyHtml = true;
        //    mailMessage.Body = noiDung;
        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Port = 587; // 25 465
        //    smtp.EnableSsl = true;
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Host = "smtp.gmail.com";
        //    smtp.Credentials = new System.Net.NetworkCredential("anhB1910186@student.ctu.edu.vn", "huynhanh18+");
        //    smtp.Send(mailMessage);
        //}


        public async Task<IPagedList<SanPham>> PagingSanPhams(int page, int size)
        {
            return await danhSachSanPham().ToPagedListAsync(page, size);
        }

        public async Task<IPagedList<SanPham>> PagingSanPhamsByLoaiSP(string loaisp, int page, int size)
        {
            return await danhSachSanPham(loaisp).ToPagedListAsync(page, size);
        }

        public async Task<IPagedList<SanPham>> PagingSanPhamsByKey(string key, int page, int size)
        {
            return await timKiem(key).ToPagedListAsync(page, size);
        }


        //public async Task<KhachHang> FindByPhoneNumberAsync(string phoneNumber)
        //{
        //    var users = await _context.KhachHangs.ToListAsync();
        //    return users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
        //}

        //public async Task<KhachHang> GetKhachHangByPhone(string input)
        //{
        //    return await _context.KhachHangs.Where(x => x.PhoneNumber == input).FirstOrDefaultAsync();
        //}

        //public List<KhuyenMai> getDanhSachGiamGia()
        //{
        //    var result= _context.KhuyenMais.Where(km => (km.NgayKetThuc - DateTimeVN()).TotalDays > 0).ToList();
        //    return result;
        //}


        //public IQueryable<KhachHang> listKhachHang()
        //{
        //    var listKH = _context.KhachHangs.Where(x => x.Id != null);
        //    return listKH;
        //}

        //public IQueryable<LoaiSanPham> listLoaiSP()
        //{
        //    return _context.LoaiSanPhams.Where(x => x.MaLoaiSp != null);
        //}


        //public IQueryable<NhaSanXuat> listNhaSanXuat()
        //{
        //    return _context.NhaSanXuats.Where(x => x.MaNsx != null);
        //}

        //public NhaSanXuat? getNhaSanXuat(string id)
        //{
        //    return listNhaSanXuat().Where(x => x.MaNsx == id).FirstOrDefault();
        //}


        //public int xoaNhaSanXuat(string id)
        //{
        //    var khoaChinh = _context.SanPhams.Where(x => x.MaNsx == id).ToList();
        //    if (khoaChinh.Any())
        //    {
        //        return 0;
        //    }
        //    _context.NhaSanXuats.Remove(_context.NhaSanXuats.Find(id));
        //    _context.SaveChanges();
        //    return 1;
        //}

        //public void themNhaSanXuat(ProducerModel nsx)
        //{
        //    var new_maNSX = "" + DateTimeVN().ToString("ddMMyyyyHhmmss") + 1;
        //    NhaSanXuat nhaSanXuat = new NhaSanXuat();
        //    nhaSanXuat.MaNsx = new_maNSX;
        //    nhaSanXuat.TenNsx = nsx.TenNSX;
        //    _context.NhaSanXuats.Add(nhaSanXuat);
        //    _context.SaveChanges();
        //}


        //public void suaNhaSanXuat(ProducerModel nsx)
        //{
        //    NhaSanXuat nhaSanXuat = getNhaSanXuat(nsx.MaNSX);
        //    _context.Update(nhaSanXuat);
        //    nhaSanXuat.TenNsx = nsx.TenNSX;
        //    _context.SaveChanges();
        //}


    }
}