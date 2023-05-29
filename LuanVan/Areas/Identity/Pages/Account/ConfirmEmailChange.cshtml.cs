﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Services;

namespace LuanVan.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;


        public ConfirmEmailChangeModel(UserManager<KhachHang> userManager, SignInManager<KhachHang> signInManager, INotyfService notyf, LanguageService localization)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf= notyf;
            _localization = localization;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                _notyf.Error(_localization.Getkey("KhongTheXacNhanEmail"));
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayUser"));
                return NotFound(_localization.Getkey("KhongTimThayUser"));
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var oldEmail = user.Email;

            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                _notyf.Error(_localization.Getkey("KhongTheThayDoiEmail"));
                //StatusMessage = _localization.Getkey("KhongTheThayDoiEmail");
                return Page();
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            if(user.UserName == oldEmail)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
                if (!setUserNameResult.Succeeded)
                {
                    //StatusMessage = _localization.Getkey("KhongTheThayDoiUsername");
                    _notyf.Error(_localization.Getkey("KhongTheThayDoiUsername"));
                    return Page();
                }
            }
            

            await _signInManager.RefreshSignInAsync(user);
            //StatusMessage = _localization.Getkey("ThayDoiEmailThanhCong");
            _notyf.Success(_localization.Getkey("ThayDoiEmailThanhCong"));
            return Page();
        }
    }
}
