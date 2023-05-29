using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Role
{
    [Authorize(Roles = "Admin")]


    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization) : base(roleManager, context, notyf, localization)
        {
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Kiểu (tên) claim là bắc buộc")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên (vai trò) role phải dài từ 3 đến 256 ký tự!")]
            public string ClaimType { get; set; }

            [Required(ErrorMessage = "Giá trị của claim là bắt buộc!")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên (vai trò) role phải dài từ 3 đến 256 ký tự!")]
            public string ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole role { get; set; }

        public IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound(_localization.Getkey("KhongTimThay"));

            claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

            if (claim == null)
                return NotFound(_localization.Getkey("KhongTimThay"));

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null)
                return NotFound(_localization.Getkey("KhongTimThay"));

            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound(_localization.Getkey("KhongTimThay"));

            claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

            if (claim == null)
                return NotFound(_localization.Getkey("KhongTimThay"));

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null)
                return NotFound(_localization.Getkey("KhongTimThay"));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                _notyf.Error("Claim " + Input.ClaimType + ": " + Input.ClaimValue + " đã có trong role");
                ModelState.AddModelError(string.Empty, "Claim " + Input.ClaimType + ": " + Input.ClaimValue +" đã có trong role");
                return Page();
            }

            //if ( _context.RoleClaims.Any(c => c.RoleId==role.Id &&  c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            //{
            //    ModelState.AddModelError(string.Empty, "Claim nay da co trong role");
            //    return Page();
            //}

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            _notyf.Success("Claim " + Input.ClaimType + ": " + Input.ClaimValue + " vừa cập nhật");
            StatusMessage = "Claim " + Input.ClaimType + ": " + Input.ClaimValue + " vừa cập nhật";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
