﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Models;

namespace Shashlik.RC.Controllers
{
    [Authorize(Roles = Roles.App)]
    public class AppController : Controller
    {
        public AppController(RCDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        private string AppId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [Authorize(Roles = Roles.App)]
        public IActionResult Index()
        {
            var app = DbContext.Set<Apps>()
                .Where(r => r.Id == AppId && r.Enabled)
                .Select(r => new AppDetailModel
                {
                    AppId = r.Id,
                    Desc = r.Desc,
                    Enabled = r.Enabled,
                    Name = r.Name,
                    Envs = r.Envs.OrderBy(f => f.Id).Select(f => new EnvModel
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Desc = f.Desc,
                        IpWhites = f.IpWhites.Select(i => i.Ip).ToList(),
                        Configs = f.Configs.OrderBy(c => c.Name).Select(c => new ConfigSimpleModel
                        {
                            Desc = c.Desc,
                            Id = c.Id,
                            Name = c.Name,
                            Type = c.Type,
                            Enabled = c.Enabled
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();
            if (app == null)
                return NotFound();

            return View(app);
        }
    }
}