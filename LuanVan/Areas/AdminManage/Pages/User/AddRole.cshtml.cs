using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin")]


    public class AddRoleModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;
        public AddRoleModel(
            UserManager<KhachHang> userManager,
            SignInManager<KhachHang> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            INotyfService notyf,
            LanguageService localization)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _notyf=notyf;
            _localization= localization;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>


        public KhachHang user { get; set; }

        [BindProperty]
        public string[] RoleNames { get; set; }

        public List<string> allRoles { get; set; }

        public List<IdentityRoleClaim<string>> claimsInRole { get; set; }

        public List<IdentityUserClaim<string>> claimsInUserClaim { get; set; }


        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }


            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleName = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

            allRoles= roleName.ToList();

            await GetClaims(id);


            return Page();
        }


        async Task GetClaims(string id)
        {
            var listRole = from r in _context.Roles
                           join ur in _context.UserRoles on r.Id equals ur.RoleId
                           where ur.UserId == id
                           select r;


            var _claimsInRole = from c in _context.RoleClaims
                                join r in listRole on c.RoleId equals r.Id
                                select c;

            claimsInRole = await _claimsInRole.ToListAsync();

            claimsInUserClaim = await (from c in _context.UserClaims
                                       where c.UserId == id
                                       select c).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            await GetClaims(id);

            var OldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            var deleteRoles = OldRoleNames.Where(r => !RoleNames.Contains(r));

            var addRoles = RoleNames.Where(r => !OldRoleNames.Contains(r));

            List<string> roleName = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

            allRoles = roleName.ToList();

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                });
                return Page();
            }


            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                });
                return Page();
            }

            //await _signInManager.RefreshSignInAsync(user);
            _notyf.Success(_localization.Getkey("UpdateRoleForUser")+ " " + user.UserName + " "+ _localization.Getkey("Thanhcong"));
            //StatusMessage = _localization.Getkey("UpdateRoleForUser") + " " + user.UserName+ " "+ _localization.Getkey("When") + " "+DateTimeVN();

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
