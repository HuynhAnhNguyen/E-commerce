using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LuanVan.Data
{
    public class ApplicationDbContext : IdentityDbContext<KhachHang>
    {
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<ChiTietHd> ChiTietHds { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<NhaSanXuat> NhaSanXuats { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer("Server=tcp:server-ct554-luanvan.database.windows.net,1433;Initial Catalog=LuanVan;Persist Security Info=False;User ID=HuynhAnhAdmin;Password=Huynhanh18+;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-VCL1NL6;Initial Catalog=LuanVan;TrustServerCertificate=True; Integrated Security=True");
            // Vesion Test
            //optionsBuilder.UseSqlServer("Data Source=DESKTOP-VCL1NL6;Initial Catalog=LuanVan_Test;TrustServerCertificate=True; Integrated Security=True");
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            builder.Entity<ChiTietHd>(entity =>
            {
                entity.HasKey(e => e.MaChiTietHd);

                entity.ToTable("ChiTietHD");

                entity.HasOne(d => d.GioHang).WithMany(p => p.ChiTietHds)
                    .HasForeignKey(d => d.MaGioHang);

                entity.HasOne(d => d.HoaDon).WithMany(p => p.ChiTietHds)
                    .HasForeignKey(d => d.MaHoaDon);
            });

            builder.Entity<GioHang>(entity =>
            {
                entity.HasKey(e => e.MaGioHang);

                entity.ToTable("GioHang");

                entity.HasOne(d => d.KhachHang).WithMany(p => p.GioHangs)
                    .HasForeignKey(d => d.KhachHangId);

                entity.HasOne(d => d.SanPham).WithMany(p => p.GioHangs)
                    .HasForeignKey(d => d.MaSanPham);
            });

            builder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHoaDon);

                entity.ToTable("HoaDon");

                entity.HasOne(d => d.KhachHang).WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.KhachHangId);

                entity.HasOne(d => d.KhuyenMai).WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaKm);

                entity.HasOne(d => d.ThanhToan).WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaPttt);
            });
            //builder.Entity<GioHang>(option =>
            //{
            //    option.ToTable("GioHang");

            //    option.HasKey(e => e.MaGioHang);


            //    //entity.ToTable("ChiTietHD");

            //    //entity.Property(e => e.MaChiTietHd)
            //    //    .HasMaxLength(20)
            //    //    .IsUnicode(false)
            //    //    .HasColumnName("MaChiTietHD");
            //    //entity.Property(e => e.MaGioHang)
            //    //    .HasMaxLength(20)
            //    //    .IsUnicode(false);
            //    //entity.Property(e => e.MaHoaDon)
            //    //    .HasMaxLength(20)
            //    //    .IsUnicode(false);

            //    //entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.ChiTietHds)
            //    //    .HasForeignKey(d => d.MaGioHang)
            //    //    .HasConstraintName("FK__ChiTietHD__MaGio__5BE2A6F2");

            //    //entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHds)
            //    //    .HasForeignKey(d => d.MaHoaDon)
            //    //    .HasConstraintName("FK__ChiTietHD__MaHoa__5AEE82B9");
            //});

            builder.Entity<KhachHang>(entity =>
            {
                entity.ToTable("KhachHang");

            });

            //builder.Entity<KhachHang>(option =>
            //{
            //    option.ToTable("KhachHang");

            //    option.HasMany<GioHang>(kh => kh.GioHangs).WithOne(gh => gh.KhachHang).HasForeignKey(gh => gh.KhachHangId);
            //    //entity.HasKey(e => e.MaChiTietHd).HasName("PK__ChiTietH__651E49EB03AF8B52");

            //    //entity.ToTable("ChiTietHD");

            //    //entity.Property(e => e.MaChiTietHd)
            //    //    .HasMaxLength(20)
            //    //    .IsUnicode(false)
            //    //    .HasColumnName("MaChiTietHD");
            //    //entity.Property(e => e.MaGioHang)
            //    //    .HasMaxLength(20)
            //    //    .IsUnicode(false);
            //    //entity.Property(e => e.MaHoaDon)
            //    //    .HasMaxLength(20)
            //    //    .IsUnicode(false);

            //    //entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.ChiTietHds)
            //    //    .HasForeignKey(d => d.MaGioHang)
            //    //    .HasConstraintName("FK__ChiTietHD__MaGio__5BE2A6F2");

            //    //entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHds)
            //    //    .HasForeignKey(d => d.MaHoaDon)
            //    //    .HasConstraintName("FK__ChiTietHD__MaHoa__5AEE82B9");
            //});




            builder.Entity<KhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaKm);

                entity.ToTable("KhuyenMai");
            });

            builder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.MaLoaiSp);

                entity.ToTable("LoaiSanPham");

            });

            builder.Entity<MauSac>(entity =>
            {
                entity.HasKey(e => e.MaMau);

                entity.ToTable("MauSac");

            });

            builder.Entity<NhaSanXuat>(entity =>
            {
                entity.HasKey(e => e.MaNsx);

                entity.ToTable("NhaSanXuat");

            });


            builder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.MaSanPham);

                entity.ToTable("SanPham");

                entity.HasOne(d => d.LoaiSp).WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaLoaiSp);

                entity.HasOne(d => d.MauSac).WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaMau);

                entity.HasOne(d => d.Nsx).WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaNsx);
            });

            builder.Entity<ThanhToan>(entity =>
            {
                entity.HasKey(e => e.MaPttt);

                entity.ToTable("ThanhToan");

            });
        }

    }
}