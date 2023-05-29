// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using LuanVan.Services;

namespace LuanVan.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly INotyfService _notyf;
        private readonly IEmailSender _emailSender;
        private readonly LanguageService _localization;



        public ConfirmEmailModel(UserManager<KhachHang> userManager, INotyfService notyf, IEmailSender emailSender, LanguageService localization)
        {
            _userManager = userManager;
            _notyf= notyf;
            _emailSender = emailSender;
            _localization = localization;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _notyf.Error(_localization.Getkey("KhongTheXacNhanEmail"));
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            string emailAddress = user.Email;
            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayUser"));
                return NotFound(_localization.Getkey("KhongTimThayUser"));
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                //StatusMessage = _localization.Getkey("XacNhanEmailThanhCong");
                _notyf.Success(_localization.Getkey("XacNhanEmailThanhCong"));
                await _emailSender.SendEmailAsync(emailAddress, _localization.Getkey("ChaoMungKhachHangMoi"),
                            _localization.Getkey("ChaoMungKHMoiContent"));
            }
            else
            {
                //StatusMessage = _localization.Getkey("LoLoiXacNhanEmaiLoiXacNhanEmail");
                _notyf.Error(_localization.Getkey("LoLoiXacNhanEmaiLoiXacNhanEmail"));
            }
            //StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return Page();
        }
    }
}
