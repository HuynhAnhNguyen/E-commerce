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


    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization) : base(roleManager, context, notyf, localization)
        {
        }

        public class InputModel
        {
            public string? RoleID { get; set; }

            [Required(ErrorMessage = "Tên (vai trò) role là bắt buộc!")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên (vai trò) role phải dài từ 3 đến 256 ký tự!")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleid)
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
            else
            {
                Input = new InputModel()
                {
                    RoleID= role.Id,
                    Name = role.Name
                };
                Claims = await _context.RoleClaims.Where(rc => rc.RoleId == roleid).ToListAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            role = await _roleManager.FindByIdAsync(roleid);

            var oldRoleName = role.Name;

            if(oldRoleName.Equals(Input.Name)) {

                _notyf.Information(_localization.Getkey("Role")+" " + oldRoleName + " "+ _localization.Getkey("NotChange"));
                //StatusMessage = _localization.Getkey("Role") + " " + oldRoleName + " " + _localization.Getkey("NotChange");
               
                return RedirectToPage("./Index");
            }

            if (role == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
                return RedirectToPage("./Index");
            }

            Claims = await _context.RoleClaims.Where(rc => rc.RoleId == roleid).ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {

                //StatusMessage = _localization.Getkey("UpdateRoleName") + " " + oldRoleName + " " + _localization.Getkey("Thanh") + " " + Input.Name + " " + _localization.Getkey("ThanhCongLuc") + " " + DateTimeVN();
                _notyf.Success(_localization.Getkey("UpdateRoleName") + " " + oldRoleName + " " + _localization.Getkey("Thanh") + " " + Input.Name + " " + _localization.Getkey("Thanhcong"), 3);
                
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

        public DateTime DateTimeVN()
        {
            DateTime utcTime = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Lấy thông tin về múi giờ của Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamZone); // Chuyển đổi giá trị DateTime từ múi giờ UTC sang múi giờ của Việt Nam

            return vietnamTime;
        }
    }
}
