using LuanVan.Data;
using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LuanVan.Areas.ManageRole.Pages.User
{
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
            public string RoleNames { get; set; }
        }

        public List<UserAndRole> users { get; set; }

        public async Task OnGet()
        {
            ////users= await _userManager.Users.OrderBy(u=> u.UserName).Select(u=> new UserAndRole()
            ////{
            ////    Id= u.Id,
            ////    UserName= u.UserName,
            ////}).ToListAsync();
            //var qr=  _userManager.Users.OrderBy(u => u.UserName);
            //int totalUser= await qr.CountAsync();
            //countPage = (int)Math.Ceiling((double)totalUser / ITEMS_PER_PAGE);

            //if (currentPage < 1)
            //    currentPage = 1;
            //if(currentPage> countPage)
            //    currentPage= countPage;

            //var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).Select(u=> new UserAndRole()
            //{
            //    Id= u.Id,
            //    UserName=u.UserName,
            //});


            users = await _userManager.Users.OrderBy(u => u.UserName).Select(u => new UserAndRole()
            {
                Id = u.Id,
                UserName = u.UserName,
            }).ToListAsync();

            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames= string.Join(",", roles);
            }

        }

            public void OnPost() => RedirectToPage();
    }
}
