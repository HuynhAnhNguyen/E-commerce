using LuanVan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace LuanVan.Areas.ManageRole.Pages.Role
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
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


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newRole = new IdentityRole(Input.Name);
            var result= await _roleManager.CreateAsync(newRole);
            if(result.Succeeded)
            {
                StatusMessage="Bạn vừa tạo role mới: "+ Input.Name+ " lúc "+ DateTime.Now;
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
