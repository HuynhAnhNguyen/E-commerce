// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using AspNetCoreHero.ToastNotification.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LuanVan.Data;
using Microsoft.EntityFrameworkCore;
using LuanVan.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LuanVan.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly UserManager<KhachHang> _userManager;
        private readonly IUserStore<KhachHang> _userStore;
        private readonly IUserEmailStore<KhachHang> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _context;
        private readonly GoogleCaptchaService _googleCaptchaService;
        private readonly LanguageService _localization;


        public RegisterModel(
            UserManager<KhachHang> userManager,
            IUserStore<KhachHang> userStore,
            SignInManager<KhachHang> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            INotyfService notyf,
            ApplicationDbContext context,
            GoogleCaptchaService googleCaptchaService,
            LanguageService localization)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _notyf = notyf;
            _context = context;
            _googleCaptchaService = googleCaptchaService;
            _localization = localization;
        }

        [TempData]
        public string StatusMessage { get; set; }
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Họ lót là bắt buộc!")]
            public string HoKhachHang { get; set; }

            [Required(ErrorMessage = "Tên là bắt buộc!")]
            public string TenKhachHang { get; set; }

            [Required(ErrorMessage = "Username là bắt buộc!")]
            [MaxLength(10, ErrorMessage ="Chiều dài tối đa của username là 10!")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Ngày sinh là bắt buộc!")]
            public DateTime NgaySinh { get; set; }

            [Required(ErrorMessage = "Giới tính là bắt buộc!")]
            public string GioiTinh { get; set; }

            [Required(ErrorMessage = "Số điện thoại là bắt buộc!")]
            [DataType(DataType.Text)]
            [MaxLength(10, ErrorMessage = "Chiều dài tối đa của số điện thoại là 10!")]
            [Phone(ErrorMessage = "Số điện thoại không đúng định dạng!")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Địa chỉ là bắt buộc!")]
            public string DiaChi { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Email là bắt buộc!")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Token là bắt buộc!")]
            public string Token { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Mật khẩu là bắt buộc!")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Mật khẩu không đủ mạnh!")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc!")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không trùng khớp")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            foreach (var provider in ExternalLogins)
            {
                _logger.LogInformation(provider.Name);
            }

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Google Captcha
            var _googleCaptcha = _googleCaptchaService.VerifyreCaptcha(Input.Token);
            if (!_googleCaptcha.Result.success && _googleCaptcha.Result.score <= 0.5)
            {
                _notyf.Error(_localization.Getkey("MaCaptChaKhongHopLe"));
                ModelState.AddModelError(string.Empty, _localization.Getkey("MaCaptChaKhongHopLe"));
                return Page();
            }

            List<string> soDienThoais = await (from p in _context.KhachHangs select p.PhoneNumber).ToListAsync();

            DateTime dob = Input.NgaySinh; // Thay đổi ngày tháng này thành ngày tháng được nhập vào từ người dùng
            DateTime now = DateTime.Now;

            TimeSpan span = now.Subtract(dob);
            int age = (int)(span.TotalDays / 365.25);

            foreach(var soDienThoai in soDienThoais)
            {
                if(soDienThoai== Input.PhoneNumber)
                {
                    _notyf.Error(_localization.Getkey("SoDienThoaiDaTonTai"));
                    ModelState.AddModelError(string.Empty, _localization.Getkey("SoDienThoaiDaTonTai"));
                    return Page();
                }
            }

            if (age < 18)
            {
                _notyf.Error(_localization.Getkey("PhaiTren18Tuoi"));
                ModelState.AddModelError(string.Empty, _localization.Getkey("PhaiTren18Tuoi"));
                return Page();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var user = CreateUser();

                    user.HoKhachHang = Input.HoKhachHang;
                    user.TenKhachHang = Input.TenKhachHang;
                    user.NgaySinh = Input.NgaySinh;
                    user.GioiTinh = Input.GioiTinh;
                    user.DiaChi = Input.DiaChi;
                    user.DiaChi = Input.DiaChi;
                    user.PhoneNumber = Input.PhoneNumber;

                    await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _notyf.Success(_localization.Getkey("DangKyThanhCong"));
                        _notyf.Information(_localization.Getkey("VuiLongXacThucTaiKhoan"));
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, _localization.Getkey("XacNhanEmail"),
                           $"Bạn đã đăng ký tài khoản, hãy <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a> để xác nhận tài khoản.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        //_notyf.Error("Lỗi khi đăng ký tài khoản");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private KhachHang CreateUser()
        {
            try
            {
                return Activator.CreateInstance<KhachHang>();
            }
            catch
            {
                throw new InvalidOperationException($"Không thể tạo phiên bản của '{nameof(KhachHang)}'. " +
                    $"Đảm bảo rằng '{nameof(KhachHang)}' không phải là một lớp trừu tượng và có một hàm tạo không tham số, hoặc cách khác " +
                    $"ghi đè trang đăng ký trong /Ares/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<KhachHang> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Giao diện người dùng mặc định yêu cầu cửa hàng người dùng có hỗ trợ email.");
            }
            return (IUserEmailStore<KhachHang>)_userStore;
        }
    }
}
