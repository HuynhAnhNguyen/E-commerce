using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Role
{
    [Authorize(Roles = "Admin, Editor, Test")]


    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization) : base(roleManager, context, notyf, localization)
        {
        }
        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }

        public List<IdentityRole> Roles { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPage { get; set; }
        public List<RoleModel> roles { get; set; }
        public List<IdentityRole> soLuongRole { get; set; }
        public async Task OnGetAsync(string Search)
        {
            soLuongRole = await _context.Roles.ToListAsync();
            if(soLuongRole.Count()> 0)
            {
                int totalRole = await _context.Roles.CountAsync();
                countPage = (int)Math.Ceiling((double)totalRole / ITEMS_PER_PAGE);

                if (currentPage < 1)
                    currentPage = 1;
                if (currentPage > countPage)
                    currentPage = countPage;


                var qr = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
                roles = new List<RoleModel>();

                if (!string.IsNullOrEmpty(Search))
                {
                    Roles = qr.Where(x => x.Name.Contains(Search)).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToList();
                }
                else
                {
                    Roles = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToList();

                }

                foreach (var _r in Roles)
                {
                    var claims = await _roleManager.GetClaimsAsync(_r);
                    var claimsString = claims.Select(c => c.Type + "=" + c.Value);
                    var rm = new RoleModel()
                    {
                        Name = _r.Name,
                        Id = _r.Id,
                        Claims = claimsString.ToArray()
                    };
                    roles.Add(rm);
                }
            }

        }

        public void OnPost() => RedirectToPage();
    }
}
