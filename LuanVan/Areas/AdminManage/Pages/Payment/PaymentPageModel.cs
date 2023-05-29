using AspNetCoreHero.ToastNotification.Abstractions;
using LuanVan.Data;
using LuanVan.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LuanVan.Areas.AdminManage.Pages.Payment
{
    public class PaymentPageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly INotyfService _notyf;
        protected readonly ILogger<PaymentPageModel> _logger;
        protected readonly LanguageService _localization;

        [TempData]
        public string StatusMessage { get; set; }
        public PaymentPageModel(ApplicationDbContext context, INotyfService notyf, ILogger<PaymentPageModel> logger, LanguageService localization)
        {
            _context = context;
            _notyf = notyf;
            _logger = logger;
            _localization = localization;
        }
    }
}
