using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuanVan.Areas.Admin.Controllers
{
    // da dang nhap co vai tro la Admin
    //[Authorize(Roles ="Admin")]
    // da dang nhap co vai tro la Admin hoac Editor hoac Vip
    //[Authorize(Roles = "Admin, Editor, Vip")]
    //// da dang nhap thoa man ca 3 vai tro Admin, Editor va Vip
    //[Authorize(Roles = "Admin")]
    //[Authorize(Roles = "Editor")]
    //[Authorize(Roles = "Vip")]

    // chi yeu cau dang nhap
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
