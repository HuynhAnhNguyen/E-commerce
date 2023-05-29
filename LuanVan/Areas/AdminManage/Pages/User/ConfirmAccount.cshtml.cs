using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Encodings.Web;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class ConfirmAccountModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly LanguageService _localization;


        public ConfirmAccountModel(
            UserManager<KhachHang> userManager,
            SignInManager<KhachHang> signInManager,
            INotyfService notyf,
            ApplicationDbContext context,
            IEmailSender emailSender, LanguageService localization)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf= notyf;
            _context = context;
            _emailSender= emailSender;
            _localization= localization;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string? KhachHangID { get; set; }
            public bool Confirm { get; set; }
        }

        public KhachHang user { get; set; }
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
            else
            {
                Input = new InputModel()
                {
                    KhachHangID= user.Id,
                    Confirm = user.EmailConfirmed
                };
                return Page();
            }
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

            var oldDisableAccount= user.EmailConfirmed;
            if(oldDisableAccount != Input.Confirm)
            {
                _context.Update(user);
                user.EmailConfirmed = Input.Confirm;
                await _context.SaveChangesAsync();

                //StatusMessage = _localization.Getkey("CapNhatXacThuc") +"" + DateTimeVN();
                _notyf.Success(_localization.Getkey("CapNhatXacThuc2") +"", 3);
            }
            else
            {
                //StatusMessage = _localization.Getkey("Sttnotchange");
                _notyf.Information(_localization.Getkey("Sttnotchange"), 3);
            }

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
