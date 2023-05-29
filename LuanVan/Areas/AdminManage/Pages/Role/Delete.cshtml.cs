using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Role
{
    [Authorize(Roles = "Admin")]


    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization) : base(roleManager, context, notyf, localization)
        {
        }

        public IdentityRole role { get; set; }

        //public async Task<IActionResult> OnGetAsync(string roleid)
        //{
        //    if (roleid == null)
        //        return NotFound(_localization.Getkey("KhongTimThay"));

        //    role = await _roleManager.FindByIdAsync(roleid);

        //    if (role == null)
        //        return NotFound(_localization.Getkey("KhongTimThay"));

        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(string roleid) => RedirectToPage("./Index");

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            role = await _roleManager.FindByIdAsync(roleid);

            if (role == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _notyf.Success(_localization.Getkey("DeleteRole") +" " + role.Name + " "+ _localization.Getkey("Thanhcong"));

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
