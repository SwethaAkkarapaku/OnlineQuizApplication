using Microsoft.AspNetCore.Mvc;

namespace QuizMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult ViewAdmins()
        {
            return View();
        }
    }
}
