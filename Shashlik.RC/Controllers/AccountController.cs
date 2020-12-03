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

// ReSharper disable StringLiteralTypo

namespace Shashlik.RC.Controllers
{
    public class AccountController : Controller
    {
        public AccountController(RCDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        private string AppId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
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
            HttpContext.Session.SetString("VERIFYCODE", code.ToUpperInvariant());
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
            else if (HttpContext.Session.GetString("VERIFYCODE") != loginModel.Code?.ToUpperInvariant())
                error = "验证码错误";

            if (error != null)
            {
                ViewData["Errors"] = error;
                return View();
            }

            var adminUser = configuration.GetValue<string>("Admin:UserName") ?? Environment.GetEnvironmentVariable("ADMIN_USER");
            var adminPassword = configuration.GetValue<string>("Admin:Password") ?? Environment.GetEnvironmentVariable("ADMIN_PASS");

            ClaimsIdentity claimIdentity = null;
            string role = null;
            if (loginModel.UserName == adminUser)
            {
                if (DbContext.IsLockout(adminUser))
                {
                    ViewData["Errors"] = "账户已锁定";
                    return View();
                }

                if (loginModel.Password == adminPassword)
                {
                    claimIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, Roles.Admin));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, adminUser!));
                    role = Roles.Admin;
                    DbContext.ResetLockEnd(adminUser);
                }
                else
                    DbContext.LoginFailed(adminUser);
            }
            else
            {
                var app = DbContext.Set<Apps>().FirstOrDefault(r => r.Id == loginModel.UserName);
                if (app is null)
                {
                    ViewData["Errors"] = "用户名或密码错误";
                    return View();
                }

                if (DbContext.IsLockout(loginModel.UserName))
                {
                    ViewData["Errors"] = "账户已锁定";
                    return View();
                }

                if (!PasswordUtils.Verify(app.Password, loginModel.Password))
                {
                    DbContext.LoginFailed(loginModel.UserName);
                    ViewData["Errors"] = "用户名或密码错误";
                    return View();
                }

                if (app.Enabled)
                {
                    claimIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, app.Id));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, Roles.App));
                    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, app.Name));
                    role = Roles.App;
                    DbContext.ResetLockEnd(loginModel.UserName);
                }
                else if (!app.Enabled)
                {
                    ViewData["Errors"] = "账户已禁用";
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
            var app = await DbContext.Set<Apps>().Where(r => r.Id == AppId).FirstOrDefaultAsync();

            if (!PasswordUtils.Verify(app.Password, input.OldPassword))
            {
                ViewData["Errors"] = "旧密码错误";
                return View();
            }

            app.Password = PasswordUtils.HashPassword(input.Password);
            await DbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}