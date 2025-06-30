using Microsoft.AspNetCore.Mvc;

namespace QuizMVC.Controllers
{
    public class UserController : Controller
    {
        public IActionResult ViewUsers()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult RegisterUser()
        //{
        //    return View();
        //}
    }
}
