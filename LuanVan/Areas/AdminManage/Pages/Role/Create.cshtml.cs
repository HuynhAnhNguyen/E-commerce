using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace LuanVan.Areas.AdminManage.Pages.Role
{
    [Authorize(Roles = "Admin")]

    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, INotyfService notyf, LanguageService localization) : base(roleManager, context, notyf, localization)
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

            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                _notyf.Success("Tạo role mới " + Input.Name + " thành công");
                //StatusMessage = "Bạn vừa tạo role mới: " + Input.Name + " lúc " + DateTimeVN();
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
