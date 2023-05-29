// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Models;
using LuanVan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuanVan.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        private readonly INotyfService _notyf;
        private readonly LanguageService _localization;

        public IndexModel(
            UserManager<KhachHang> userManager,
            SignInManager<KhachHang> signInManager,
            INotyfService notyf,
            LanguageService localization)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf=notyf;
            _localization = localization;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

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
            [Phone(ErrorMessage = "Số điện thoại sai định dạng!")]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Họ lót là bắt buộc!")]
            public string HoKhachHang { get; set; }
            [Required(ErrorMessage = "Tên là bắt buộc!")]
            public string TenKhachHang { get; set; }

            [Required(ErrorMessage = "Ngày sinh là bắt buộc!")]
            public DateTime NgaySinh { get; set; }

            [Required(ErrorMessage = "Giới tính là bắt buộc!")]
            public string GioiTinh { get; set; }

            [Required(ErrorMessage = "Địa chỉ là bắt buộc!")]
            public string DiaChi { get; set; }
        }

        private async Task LoadAsync(KhachHang user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                HoKhachHang = user.HoKhachHang,
                TenKhachHang = user.TenKhachHang,
                NgaySinh = user.NgaySinh,
                GioiTinh = user.GioiTinh,
                DiaChi = user.DiaChi
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _notyf.Error(_localization.Getkey("KhongTimThayUser"));
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
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

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            DateTime dob = Input.NgaySinh; // Thay đổi ngày tháng này thành ngày tháng được nhập vào từ người dùng
            DateTime now = DateTime.Now;

            TimeSpan span = now.Subtract(dob);
            int age = (int)(span.TotalDays / 365.25);

            if (age < 18)
            {
                _notyf.Error(_localization.Getkey("PhaiTren18Tuoi"));
                ModelState.AddModelError(string.Empty, _localization.Getkey("PhaiTren18Tuoi"));
                return Page();
            }
            else
            {
                user.HoKhachHang = Input.HoKhachHang;
                user.TenKhachHang = Input.TenKhachHang;
                user.NgaySinh = Input.NgaySinh;
                user.GioiTinh = Input.GioiTinh;
                user.DiaChi = Input.DiaChi;
                user.PhoneNumber = Input.PhoneNumber;

                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);
                _notyf.Success(_localization.Getkey("ThongTinCaNhanDaDuocCapNhat"));
                //StatusMessage = _localization.Getkey("ThongTinCaNhanDaDuocCapNhat");
                return RedirectToPage();
            }

        }
    }
}
