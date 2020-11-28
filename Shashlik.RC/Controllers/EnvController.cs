using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Models;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Controllers
{
    [Authorize(Roles = Roles.App)]
    public class EnvController : Controller
    {
        public EnvController(RCDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }
        private string AppId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>
        /// 新增/编辑环境 变量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Roles.App)]
        public IActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                ViewData.Model =
                    DbContext.Set<Envs>()
                        .Where(r => r.Id == id && r.AppId == AppId)
                        .Select(r => new EnvModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            IpWhites = r.IpWhites.Select(f => f.Ip).ToList(),
                            Desc = r.Desc,
                            Key = r.Key,
                            Configs = r.Configs.Select(f => new ConfigSimpleModel
                            {
                                Desc = f.Desc,
                                Id = f.Id,
                                Name = f.Name,
                                Type = f.Type,
                                Enabled = f.Enabled
                            }).ToList()
                        })
                        .FirstOrDefault();
                if (ViewData.Model == null)
                    return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(EnvAddOrUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Errors"] = ModelState.SelectMany(r => r.Value.Errors.Select(f => f.ErrorMessage)).FirstOrDefault();
                return View();
            }

            model.IpWhites ??= "";
            var ipWhites =
                model.IpWhites.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                    .Where(r => !r.IsNullOrWhiteSpace())
                    .Distinct()
                    .Select(r => new IpWhites {Ip = r.Trim()})
                    .ToList();

            if (model.Id.HasValue)
            {
                var env = DbContext
                    .Set<Envs>()
                    .Include(r => r.IpWhites)
                    .FirstOrDefault(r => r.Id == model.Id && r.AppId == AppId);
                if (env == null)
                    return NotFound();

                DbContext.RemoveRange(env.IpWhites);
                env.Name = model.Name;
                env.Desc = model.Desc;
                if (!ipWhites.IsNullOrEmpty())
                    env.IpWhites = ipWhites.ToList();
            }
            else
            {
                DbContext.Add(new Envs
                {
                    Name = model.Name,
                    AppId = AppId,
                    Desc = model.Desc,
                    IpWhites = ipWhites.ToList(),
                    Key = Guid.NewGuid().ToString("n").ToUpperInvariant()
                });
            }

            DbContext.SaveChanges();
            return RedirectToAction("index", "app");
        }

        /// <summary>
        /// 复制环境
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public IActionResult Copy(int id)
        {
            var env =
                DbContext.Set<Envs>()
                    .Where(r => r.Id == id && r.AppId == AppId)
                    .Include(r => r.IpWhites)
                    .Include(r => r.Configs)
                    .FirstOrDefault();

            if (env == null)
                return NotFound();

            var copyEnv = new Envs
            {
                AppId = AppId,
                Name = env.Name + "_copy",
                IpWhites = env.IpWhites.Select(r => new IpWhites {Ip = r.Ip}).ToList(),
                Key = Guid.NewGuid().ToString("n").ToUpperInvariant(),
                Configs = env.Configs.Select(r => new Configs
                {
                    Content = r.Content,
                    Desc = r.Desc,
                    Name = r.Name,
                    Type = r.Type,
                    Enabled = r.Enabled
                }).ToList()
            };
            DbContext.Add(copyEnv);
            DbContext.SaveChanges();

            return RedirectToAction("index", "app");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var env =
                DbContext.Set<Envs>()
                    .Where(r => r.Id == id && r.AppId == AppId)
                    .Include(r => r.IpWhites)
                    .Include(r => r.Configs)
                    .FirstOrDefault();

            if (env == null)
                return NotFound();

            DbContext.RemoveRange(env.IpWhites);
            DbContext.RemoveRange(env.Configs);
            DbContext.Remove(env);
            DbContext.SaveChanges();
            return RedirectToAction("index", "app");
        }

        /// <summary>
        /// 刷新密钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPut]
        public IActionResult RefreshKey(int id)
        {
            var env =
                DbContext.Set<Envs>()
                    .Where(r => r.Id == id && r.AppId == AppId)
                    .Include(r => r.IpWhites)
                    .Include(r => r.Configs)
                    .FirstOrDefault();

            if (env == null)
                return NotFound();

            env.Key = Guid.NewGuid().ToString("n").ToUpperInvariant();
            DbContext.SaveChanges();
            return RedirectToAction(nameof(Index), new {id});
        }
    }
}