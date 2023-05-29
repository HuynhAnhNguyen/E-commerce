// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Services;

namespace LuanVan.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;


        public ForgotPasswordModel(UserManager<KhachHang> userManager, IEmailSender emailSender, INotyfService notyf, LanguageService localization)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _notyf = notyf;
            _localization = localization;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Email là bắt buộc!")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    _notyf.Error(_localization.Getkey("KhongTimThayTaiKhoan") +" "+ Input.Email);
                    return Page();
                }

                if(!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _notyf.Success(_localization.Getkey("ThucHienTheoYeuCau"));
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                _notyf.Success(_localization.Getkey("GuiEmailKhoiPhucMKThanhCong"));
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    _localization.Getkey("DatLaiMatKhau"),
                    $"Để đặt lại mật khẩu. Vui lòng <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
