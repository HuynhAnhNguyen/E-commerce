// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace LuanVan.Areas.Identity.Pages.Account.Manage
{
    [Authorize]

    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;


        public PersonalDataModel(
            UserManager<KhachHang> userManager,
            ILogger<PersonalDataModel> logger,
            INotyfService notyf,
            LanguageService localization)
        {
            _userManager = userManager;
            _logger = logger;
            _notyf = notyf;
            _localization= localization;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayUser"));
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
