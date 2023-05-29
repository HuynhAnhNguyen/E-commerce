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
using System.Security.Claims;

namespace LuanVan.Areas.AdminManage.Pages.Role
{
    [Authorize(Roles = "Admin")]


    public class DeleteRoleClaimModel : RolePageModel
    {
        public DeleteRoleClaimModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization) : base(roleManager, context, notyf, localization)
        {
        }

        public IdentityRole role { get; set; }

        public IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGetAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

            if (claim == null)
                return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null)
                return NotFound("Không tìm thấy role");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");

            claim = await _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

            if (claim == null)
                return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null)
                return NotFound("Không tìm thấy role");

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            StatusMessage = "Vừa xóa claim" + claim.ClaimType + ": " + claim.ClaimValue + " thành công";
            _notyf.Success("Vừa xóa claim " + claim.ClaimType + ": " + claim.ClaimValue + " thành công");

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
