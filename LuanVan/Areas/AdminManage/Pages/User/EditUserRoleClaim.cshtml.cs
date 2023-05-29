using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace LuanVan.Areas.AdminManage.Pages.User
{
    [Authorize(Roles = "Admin")]

    public class EditUserRoleClaimModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<KhachHang> _userManager;
        private readonly INotyfService _notyf;


        public EditUserRoleClaimModel(ApplicationDbContext context, UserManager<KhachHang> userManager, INotyfService notyf)
        {
            _context = context;
            _userManager = userManager;
            _notyf= notyf;
        }


        [TempData]
        public string StatusMessage { get; set; }

        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập");

        public class InputModel
        {
            [Required(ErrorMessage = "Kiểu (tên) claim là bắc buộc")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên (vai trò) role phải dài từ 3 đến 256 ký tự!")]
            public string ClaimType { get; set; }

            [Required(ErrorMessage = "Giá trị của claim là bắt buộc!")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tên (vai trò) role phải dài từ 3 đến 256 ký tự!")]
            public string ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public KhachHang user { get; set; }

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);

            if (user == null)
                return NotFound("Không tìm thấy user");

            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);

            if (user == null)
                return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid)
                return Page();

            var claims = _context.UserClaims.Where(c => c.UserId == userid);

            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                _notyf.Error("Claim " + Input.ClaimType + " :" + Input.ClaimValue + " đã tồn tại");
                ModelState.AddModelError(string.Empty, "Claim này đã tồn tại");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm claim " + Input.ClaimType + " :" + Input.ClaimValue + " thành công cho user "+ user.UserName;
            _notyf.Success("Đã thêm claim " + Input.ClaimType + " :" + Input.ClaimValue + " thành công cho user "+ user.UserName + " lúc "+ DateTimeVN());

            return RedirectToPage("./AddRole", new { Id = user.Id });

        }

        public IdentityUserClaim<string> userClaim { get; set; }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound("Không tìm thấy claim");

            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if (user == null)
                return NotFound("Không tìm thấy user");

            Input = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue,
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound("Không tìm thấy claim");

            userClaim = await _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if (user == null)
                return NotFound("Không tìm thấy user");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.UserClaims.Any(c => c.UserId == user.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != userClaim.Id))
            {
                _notyf.Error("Claim " + Input.ClaimType + " :" + Input.ClaimValue + " đã tồn tại");
                ModelState.AddModelError(string.Empty, "Claim này đã tồn tại");
                return Page();
            }


            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            _notyf.Success("Cập nhật claim riêng cho user " + user.UserName + " thành công");
            StatusMessage = "Cập nhật claim riêng cho user " + user.UserName + " thành công lúc " + DateTimeVN() ;

            return RedirectToPage("./AddRole", new { Id = user.Id });

        }

        public async Task<IActionResult> OnPostDeleteClaimAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound("Không tìm thấy claim");

            userClaim = await _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefaultAsync();

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if (user == null)
                return NotFound("Không tìm thấy user");

            await _userManager.RemoveClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));

            _notyf.Success("Xóa claim riêng của user " + user.UserName + " thành công");
            StatusMessage = "Xóa claim riêng của user " + user.UserName + " thành công lúc " + DateTimeVN();

            return RedirectToPage("./AddRole", new { Id = user.Id });

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
