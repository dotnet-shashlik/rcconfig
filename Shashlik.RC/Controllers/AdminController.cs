using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Models;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller
    {
        public AdminController(RCDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model =
                DbContext.Set<Apps>()
                    .OrderByDescending(r => r.CreateTime)
                    .Select(r => new AppModel
                    {
                        AppId = r.Id,
                        Desc = r.Desc,
                        Name = r.Name,
                        Enabled = r.Enabled
                    })
                    .ToList();

            return View(model);
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id">appid</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPut]
        public IActionResult Enabled(string id)
        {
            var app = DbContext.Set<Apps>().FirstOrDefault(r => r.Id == id);

            if (app == null)
                return NotFound();

            app.Enabled = true;
            DbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id">appid</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPut]
        public IActionResult Disabled(string id)
        {
            var app = DbContext.Set<Apps>().FirstOrDefault(r => r.Id == id);

            if (app == null)
                return NotFound();

            app.Enabled = false;
            DbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateApp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApp(AppAddModel model)
        {
            string error = null;
            if (!ModelState.IsValid)
                error = ModelState.SelectMany(r => r.Value.Errors.Select(f => f.ErrorMessage)).FirstOrDefault();

            if (error != null)
            {
                ViewData["Errors"] = error;
                return View();
            }

            var app =
                new Apps
                {
                    Id = Guid16().ToUpper(),
                    Name = model.Name,
                    Password = HashHelper.MD5(model.Password).ToUpperInvariant(),
                    Desc = model.Desc,
                    CreateTime = DateTime.Now
                };
            DbContext.Add(app);

            await DbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private static string Guid16()
        {
            long i = Guid.NewGuid()
                .ToByteArray()
                .Aggregate<byte, long>(1, (current, b) => current * (b + 1));
            return $"{i - DateTime.Now.Ticks:x}";
        }
    }
}