// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LuanVan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace LuanVan.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly INotyfService _notyf;


        public LogoutModel(SignInManager<KhachHang> signInManager, ILogger<LogoutModel> logger, INotyfService notyf)
        {
            _signInManager = signInManager;
            _logger = logger;
            _notyf= notyf;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _notyf.Information("Đăng xuất thành công");
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {

                returnUrl= Url.Content("~/");
                return LocalRedirect(returnUrl);

                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                //return RedirectToPage();
            }
        }
    }
}
