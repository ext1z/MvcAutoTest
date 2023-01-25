using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAutoTest.Models;
using WebAutoTest.Services;

namespace WebAutoTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
       

        public HomeController(UserService userService)
        {
            _userService = userService;
        }


        public IActionResult Index()
        {
            bool isLogin = true;
            var user = _userService.GetUserFromCookie(HttpContext);

            if (user == null)
            {
                isLogin = false;
            }
            ViewBag.IsLogin = isLogin;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}