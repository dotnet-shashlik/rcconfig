using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Models;
using Shashlik.RC.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Controllers
{
    [Authorize(Roles = Roles.App)]
    public class EnvController : Controller
    {
        public EnvController(RCDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        RCDbContext dbContext { get; }
        string appId => User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier).Value;

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
                dbContext.Set<Envs>()
                    .Where(r => r.Id == id && r.AppId == appId)
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
            model.IpWhites = model.IpWhites ?? "";
            var ipWhites =
              model.IpWhites.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                  .Where(r => !r.IsNullOrWhiteSpace())
                  .Distinct()
                  .Select(r => new IpWhites { Ip = r.Trim() });

            if (model.Id.HasValue)
            {
                var env =
                dbContext.Set<Envs>()
                    .Include(r => r.IpWhites)
                    .Where(r => r.Id == model.Id && r.AppId == appId)
                    .FirstOrDefault();
                if (env == null)
                    return NotFound();

                dbContext.RemoveRange(env.IpWhites);
                env.Name = model.Name;
                env.Desc = model.Desc;
                if (!ipWhites.IsNullOrEmpty())
                    env.IpWhites = ipWhites.ToList();
            }
            else
            {
                dbContext.Add(new Envs
                {
                    Name = model.Name,
                    AppId = appId,
                    Desc = model.Desc,
                    IpWhites = ipWhites.ToList(),
                    Key = Guid.NewGuid().ToString("n").ToUpperInvariant()
                });
            }

            dbContext.SaveChanges();
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
            dbContext.Set<Envs>()
                .Where(r => r.Id == id && r.AppId == appId)
                .Include(r => r.IpWhites)
                .Include(r => r.Configs)
                .FirstOrDefault();

            if (env == null)
                return NotFound();

            var copyEnv = new Envs
            {
                AppId = appId,
                Name = env.Name + "_copy",
                IpWhites = env.IpWhites.Select(r => new IpWhites { Ip = r.Ip }).ToList(),
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
            dbContext.Add(copyEnv);
            dbContext.SaveChanges();

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
            dbContext.Set<Envs>()
                .Where(r => r.Id == id && r.AppId == appId)
                .Include(r => r.IpWhites)
                .Include(r => r.Configs)
                .FirstOrDefault();

            if (env == null)
                return NotFound();

            dbContext.RemoveRange(env.IpWhites);
            dbContext.RemoveRange(env.Configs);
            dbContext.Remove(env);
            dbContext.SaveChanges();
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
              dbContext.Set<Envs>()
                  .Where(r => r.Id == id && r.AppId == appId)
                  .Include(r => r.IpWhites)
                  .Include(r => r.Configs)
                  .FirstOrDefault();

            if (env == null)
                return NotFound();

            env.Key = Guid.NewGuid().ToString("n").ToUpperInvariant();
            dbContext.SaveChanges();
            return RedirectToAction(nameof(Index), new { id });
        }
    }
}
