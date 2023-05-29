using DocumentFormat.OpenXml.Office2010.Excel;
using LuanVan.Data;
using LuanVan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin, Editor, Test")]


    public class IndexModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<KhachHang> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [TempData]
        public string StatusMessage { get; set; }

        public class UserAndRole : KhachHang
        {
            public int statusAccount { get; set; }
            public string RoleNames { get; set; }
            public int cancelBillNumber { get; set; }
            public int successBillNumber { get; set; }
        }

        public List<KhachHang> soLuongUser { get; set; }

        public List<UserAndRole> users { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }

        public bool IsValidEmail(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
        }

        public async Task OnGetAsync(string Search)
        {
            soLuongUser = await _context.KhachHangs.ToListAsync();
            if(soLuongUser.Count() > 0)
            {
                int totalUser = await _context.KhachHangs.CountAsync();
                countPage = (int)Math.Ceiling((double)totalUser / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;


                var qr = await _userManager.Users.OrderBy(u => u.UserName).Select(u => new UserAndRole()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email= u.Email,
                    EmailConfirmed= u.EmailConfirmed,
                }).ToListAsync();

                if (!string.IsNullOrEmpty(Search))
                {
                    if(IsValidEmail(Search)) {
                        users = qr.Where(x => x.Email.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToList();
                    }
                    else
                    {
                        users = qr.Where(x => x.UserName.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToList();
                    }
                }
                else
                {
                    users = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToList();

                }
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var numberBillCancel= await _context.HoaDons.Where(a => a.KhachHangId == user.Id && a.TrangThaiDonHang==-1).CountAsync();
                    var numberBillSuccess= await _context.HoaDons.Where(a => a.KhachHangId == user.Id && a.TrangThaiDonHang != -1).CountAsync();
                    var statusAccount = await _context.KhachHangs.Where(a=> a.Id== user.Id).Select(a => a.DisableAccount).FirstOrDefaultAsync();
                    user.statusAccount = statusAccount;
                    user.successBillNumber= numberBillSuccess;
                    user.cancelBillNumber = numberBillCancel;
                    user.RoleNames = string.Join(", ", roles);
                }
            }

        }

        public void OnPost() => RedirectToPage();
    }
}
