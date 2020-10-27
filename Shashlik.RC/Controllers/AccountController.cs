using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Models;
using Shashlik.RC.Utils;

namespace Shashlik.RC.Controllers
{
    public class AccountController : Controller
    {

        public AccountController(RCDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        RCDbContext dbContext { get; }
        string appId => User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier).Value;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(Roles.App))
                    return RedirectToAction("index", "app");
                else if (User.IsInRole(Roles.App))
                    return RedirectToAction("index", "admin");
            }
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Code()
        {
            var ms = new MemoryStream();
            var code = ImgHelper.BuildVerifyCode(ms, 4, 136, 32);
            HttpContext.Session.SetString("VERIFYCODE", code.ToUpper());
            return File(ms.ToArray(), "image/gif");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, [FromServices] IConfiguration configuration)
        {
            string error = null;
            if (!ModelState.IsValid)
                error = ModelState.SelectMany(r => r.Value.Errors.Select(f => f.ErrorMessage)).FirstOrDefault();
            else if (HttpContext.Session.GetString("VERIFYCODE") != loginModel.Code?.ToUpper())
                error = "验证码错误";

            if (error != null)
            {
                ViewData["Errors"] = error;
                return View();
            }

            var admin = configuration.GetSection("Admin").Get<LoginModel>();
            ClaimsIdentity claimIdentity = null;
            string role = null;
            if (loginModel.UserName == admin.UserName)
            {
                if (loginModel.Password.Md532().ToUpper() == admin.Password)
                {
                    claimIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, Roles.Admin));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, admin.UserName));
                    role = Roles.Admin;
                }
            }
            else
            {
                var pwd = loginModel.Password.Md532().ToUpper();
                var app = dbContext.Set<Apps>().FirstOrDefault(r => r.Id == loginModel.UserName && r.Password == pwd);
                if (app != null && app.Enabled)
                {
                    claimIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, app.Id));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, Roles.App));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, app.Name));
                    role = Roles.App;
                }
                else if (app != null && !app.Enabled)
                {
                    ViewData["Errors"] = "应用审核中";
                    return View();
                }
            }

            if (claimIdentity != null)
            {
                var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.Now.AddMinutes(30)
                });
                if (role == Roles.Admin)
                    return RedirectToAction("index", "admin");
                else
                    return RedirectToAction("index", "app");
            }
            else
            {
                ViewData["Errors"] = "登录凭据错误";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [Authorize(Roles = Roles.App)]
        [HttpGet]
        public IActionResult Pwd()
        {
            return View();
        }

        [Authorize(Roles = Roles.App)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pwd(ChangePasswordInput input)
        {
            var app = await dbContext.Set<Apps>().Where(r => r.Id == appId).FirstOrDefaultAsync();

            var oldpwd = input.OldPassword.Md532().ToUpper();
            if (app.Password != oldpwd)
            {
                ViewData["Errors"] = "旧密码错误";
                return View();
            }

            var newpwd = input.Password.Md532().ToUpper();
            app.Password = newpwd;
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
