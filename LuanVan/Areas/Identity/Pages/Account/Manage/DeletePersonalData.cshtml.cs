// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LuanVan.Areas.Identity.Pages.Account.Manage
{
    [Authorize]

    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;
        private readonly ApplicationDbContext _context;


        public DeletePersonalDataModel(
            UserManager<KhachHang> userManager,
            SignInManager<KhachHang> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            INotyfService notyf,
            LanguageService localization,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyf= notyf;
            _localization = localization;
            _context= context;
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
            [Required(ErrorMessage = "Mật khẩu là bắt buộc!")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public List<HoaDon> hoaDons { get; set; }

        public List<GioHang> gioHangs { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayUser"));

                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayUser"));

                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    _notyf.Error(_localization.Getkey("MatKhauSai"));
                    //ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            hoaDons = await _context.HoaDons.Where(x => x.KhachHangId == user.Id).ToListAsync();
            gioHangs = await _context.GioHangs.Where(x => x.KhachHangId == user.Id).ToListAsync();

            if (gioHangs.Count() > 0)
            {
                foreach (var gioHang in gioHangs)
                {
                    gioHang.KhachHangId = null;
                    await _context.SaveChangesAsync();
                }
            }

            if (hoaDons.Count() > 0)
            {
                foreach (var hoaDon in hoaDons)
                {
                    hoaDon.KhachHangId = null;
                    await _context.SaveChangesAsync();
                }
            }

            try
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return Page();
                    throw new InvalidOperationException($"Unexpected error occurred deleting user.");
                }
            }
            catch(Exception ex)
            {
                _notyf.Error(_localization.Getkey("KhongTheXoaTK"));
            }

            var userId = await _userManager.GetUserIdAsync(user);
            

            await _signInManager.SignOutAsync();

            _notyf.Success(_localization.Getkey("XoaTKThanhCong"));
            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
