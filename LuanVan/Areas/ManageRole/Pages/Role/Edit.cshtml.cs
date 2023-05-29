using LuanVan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LuanVan.Areas.ManageRole.Pages.Role
{
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
        {
        }
        public class InputModel
        {
            [Required(ErrorMessage = "Tên (vai trò) role là bắt buộc!")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên (vai trò) role phải dài từ 3 đến 256 ký tự!")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }


        public List<IdentityRoleClaim<string>> Claims { get; set; }


        public IdentityRole role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy");

            role = await _roleManager.FindByIdAsync(roleid);

            if(role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                Claims = await _context.RoleClaims.Where(rc => rc.RoleId == roleid).ToListAsync();
                return Page();
            }
            return NotFound("Không tìm thấy");
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy");


            role = await _roleManager.FindByIdAsync(roleid);
            if(role== null) return NotFound("Không tìm thấy");
            Claims = await _context.RoleClaims.Where(rc => rc.RoleId == roleid).ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            role.Name= Input.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = "Bạn vừa đổi tên role: " + role.Name +" thành : "+ Input.Name+ " lúc "+ DateTime.Now;
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                );
            }


            return Page();
        }
    }
}
