using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QuizMVC.Captcha;
using QuizMVC.DTO;
using System.Security.Claims;
using System.Text.Json;

namespace QuizMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _client;

        public AccountController(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("http://localhost:5180/api/");
        }
        [HttpGet]
        public IActionResult AccountLogin()
        {
            string code = CaptchaGenerator.GenerateCaptchaCode(5);
            HttpContext.Session.SetString("Captchacode", code);

            var model = new RegisterViewModeldto
            {
                CaptchaOutput = code
            };

            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AccountLogin(RegisterViewModeldto model)
        {
            if (!ModelState.IsValid)
            {
                model = ResetCaptcha(model);
                return View(model);
            }

            string sessionCaptcha = HttpContext.Session.GetString("Captchacode");

            if (model.CaptchaInput != sessionCaptcha)
            {
                ModelState.AddModelError("CaptchaInput", "Incorrect Captcha");
                model = ResetCaptcha(model);
                return View(model);
            }

            // Now authenticate from API
            GetAccountData users = await GetAccounts();
            var matchedUser = users?.data.FirstOrDefault(u =>
                u.email == model.Email && u.password == model.Password);

            if (matchedUser == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                model = ResetCaptcha(model);
                return View(model);
            }

            await SignInUser(model.Email, matchedUser.role);

            // Redirect based on role
            return matchedUser.role switch
            {
                "Admin" => RedirectToAction("ViewAdmins", "Admin"),
                "User" => RedirectToAction("ViewUsers", "User")
                //_ => RedirectToAction("AccountLogin", "Account")
            };
        }
        private RegisterViewModeldto ResetCaptcha(RegisterViewModeldto model)
        {
            string newCaptcha = CaptchaGenerator.GenerateCaptchaCode(5);
            HttpContext.Session.SetString("Captchacode", newCaptcha);
            model.CaptchaOutput = newCaptcha;
            model.CaptchaInput = string.Empty;
            return model;
        }

        private async Task SignInUser(string email, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }







        [HttpGet]
        public async Task<IActionResult> ViewAccounts()
        {
            var accounts = await GetAccounts();
            return View(accounts);
        }
        public async Task<GetAccountData> GetAccounts()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("Account");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetAccountData>(content);
            }
            catch
            {
                return null;
            }
        }
    }
}