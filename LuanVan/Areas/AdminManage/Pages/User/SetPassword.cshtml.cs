using AspNetCoreHero.ToastNotification.Abstractions;
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
using System.Text;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin")]
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;
        private readonly IEmailSender _emailSender;


        public SetPasswordModel(
            UserManager<KhachHang> userManager,
            SignInManager<KhachHang> signInManager,
            INotyfService notyf,
            LanguageService localization,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf= notyf;
            _localization = localization;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Mật khẩu là bắt buộc!")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Mật khẩu không đủ mạnh!")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc!")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không chính xác!")]
            public string ConfirmPassword { get; set; }
        }

        public KhachHang user { get; set; }
        //public async Task<IActionResult> OnGetAsync(string id)
        //{
        //    Console.WriteLine(GenerateRandomString());
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
        //        return RedirectToPage("./Index");
        //    }

        //    user = await _userManager.FindByIdAsync(id);

        //    if (user == null)
        //    {
        //        _notyf.Error(_localization.Getkey("KhongTimThay"), 3);
        //        return RedirectToPage("./Index");
        //    }

        //    return Page();
        //}

        public async Task<IActionResult> OnGetAsync(string id) => RedirectToPage("./Index");

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

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            await _userManager.RemovePasswordAsync(user);
            string newPass = GenerateRandomString();
            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPass);

            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            //await _signInManager.RefreshSignInAsync(user);
            //StatusMessage = _localization.Getkey("SetPasswordUser") + " " + user.UserName + " "+ _localization.Getkey("When")+" " + DateTimeVN();
            _notyf.Success(_localization.Getkey("SetPasswordUser") +" " + user.UserName + " "+ _localization.Getkey("Thanhcong"));

            await _emailSender.SendEmailAsync(user.Email, "Mật khẩu đã được cập nhật",
                           "Mật khẩu mới của bạn là "+ "<strong>"+ newPass+ "</strong>");

            return RedirectToPage("./Index");
        }

        public DateTime DateTimeVN()
        {
            DateTime utcTime = DateTime.UtcNow; // Lấy thời gian hiện tại theo giờ UTC
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Lấy thông tin về múi giờ của Việt Nam
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamZone); // Chuyển đổi giá trị DateTime từ múi giờ UTC sang múi giờ của Việt Nam

            return vietnamTime;
        }

        public string GenerateRandomString()
        {
            string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            string specialCharacters = "!@#$%^&*()";

            Random random = new Random();

            StringBuilder builder = new StringBuilder();

            // Chọn ngẫu nhiên một chữ cái viết hoa
            char uppercaseChar = uppercaseLetters[random.Next(uppercaseLetters.Length)];
            builder.Append(uppercaseChar);

            // Chọn ngẫu nhiên một chữ cái viết thường
            char lowercaseChar = lowercaseLetters[random.Next(lowercaseLetters.Length)];
            builder.Append(lowercaseChar);

            // Chọn ngẫu nhiên một kí tự đặc biệt
            char specialChar = specialCharacters[random.Next(specialCharacters.Length)];
            builder.Append(specialChar);

            // Chọn ngẫu nhiên 5 kí tự bất kỳ
            for (int i = 0; i < 5; i++)
            {
                char randomChar = (char)random.Next(33, 127); // Mã ASCII từ 33 đến 126 đại diện cho các kí tự in được
                builder.Append(randomChar);
            }

            return builder.ToString();
        }
    }
}
