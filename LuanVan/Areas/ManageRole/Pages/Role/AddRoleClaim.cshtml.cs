using LuanVan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace LuanVan.Areas.ManageRole.Pages.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
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

        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            role= await _roleManager.FindByIdAsync(roleid);
            if(role== null)
                return NotFound("Không tìm thấy role");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound("Không tìm thấy role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if((await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role "+ role.Name);
                return Page();
            }

            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result= await _roleManager.AddClaimAsync(role, newClaim); 
            if(!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = "Vừa thêm đặc tính mới (claim) "+ Input.ClaimType+ ": "+ Input.ClaimValue+" cho role: "+ role.Name+ " lúc "+ DateTime.Now;

            return RedirectToPage("./Edit", new { roleid = roleid });

        }
    }
}
