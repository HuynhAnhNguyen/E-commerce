using LuanVan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace LuanVan.Areas.ManageRole.Pages.Role
{
    public class DeleteRoleClaimModel : RolePageModel
    {
        public DeleteRoleClaimModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
        {
        }


        public IdentityRole role { get; set; }

        public IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGet (int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");


            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy role");


            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("Không tìm thấy role");

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            StatusMessage = "Vừa xóa claim "+ claim.ClaimType +": "+ claim.ClaimValue+" lúc "+ DateTime.Now;

            return RedirectToPage("./Edit", new { roleid = role.Id });

        }
    }
}
