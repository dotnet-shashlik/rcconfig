using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.RC.Models;
using Shashlik.RC.Utils;
using Shashlik.RC.WebSocket;

namespace Shashlik.RC.Controllers
{
    [Authorize(Roles = Roles.App)]
    public class ConfigController : Controller
    {
        public ConfigController(RCDbContext dbContext, WebSocketContext webSocketContext)
        {
            this.dbContext = dbContext;
            this.webSocketContext = webSocketContext;
        }

        RCDbContext dbContext { get; }
        WebSocketContext webSocketContext { get; }

        string appId => User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier).Value;

        [HttpGet]
        public IActionResult Index(int? id, int? envId)
        {
            if (!id.HasValue && !envId.HasValue)
                return RedirectToAction("index", "app");
            ConfigModel config = null;
            if (id.HasValue)
            {
                config = GetConfigModel(id.Value);
                if (config == null)
                    return NotFound();
                envId = config.EnvId;
            }

            var env = dbContext.Set<Envs>().FirstOrDefault(r => r.AppId == appId && r.Id == envId);
            if (env == null)
                return BadRequest("参数错误");

            ViewData["EnvId"] = env.Id;
            ViewData["EnvName"] = env.Name;
            ViewData["EnvDesc"] = env.Desc;

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConfigAddOrUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Errors"] = ModelState.SelectMany(r => r.Value.Errors.Select(f => f.ErrorMessage)).FirstOrDefault();
                return View();
            }
            if (model.Type == Enums.ConfigType.json)
            {
                if (model.Content.IsNullOrWhiteSpace())
                    model.Content = "{}";
                try
                {
                    JsonConvert.DeserializeObject<object>(model.Content);
                }
                catch
                {
                    ViewData["Errors"] = "json格式错误";
                    return View();
                }
            }

            Configs config;
            if (model.Id.HasValue)
            {
                var nameIsSection = model.NameIsSection ? 1 : 0;

                config = dbContext.Set<Configs>().FirstOrDefault(r => r.Id == model.Id && r.Env.AppId == appId);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"修改配置:{config.Name}[{config.Desc}]");
                if (config.Name.Trim() != model.Name.Trim())
                    sb.AppendLine($"修改配置名称:{config.Name}->{model.Name}");
                if (config.Type != model.Type.ToString())
                    sb.AppendLine($"修改类型:{config.Type}->{model.Type}");

                await Modify(config.Id, model.EnvId, config.Name, sb.ToString(), config.Content, model.Content);
                config.Content = model.Content;
                config.Type = model.Type.ToString();
                config.Name = model.Name;
                config.Desc = model.Desc;
                dbContext.SaveChanges();
            }
            else
            {
                config =
                   new Configs
                   {
                       Content = model.Content,
                       Enabled = false,
                       Desc = model.Desc,
                       EnvId = model.EnvId,
                       Name = model.Name,
                       Type = model.Type.ToString(),
                   };
                dbContext.Add(config);
                dbContext.SaveChanges();

                await Modify(config.Id, model.EnvId, config.Name, $"新增配置:{config.Name}", "", model.Content);
            }

            var config1 = dbContext.Set<Configs>()
                 .Include(r => r.Env.App)
                 .FirstOrDefault(r => r.Env.AppId == appId && r.Id == config.Id);
            if (config1 == null)
                return BadRequest("参数错误");

            ViewData["EnvId"] = config1.Env.Id;
            ViewData["EnvName"] = config1.Env.Name;
            ViewData["EnvDesc"] = config1.Env.Desc;

            return RedirectToAction(nameof(Index));
        }

        ConfigModel GetConfigModel(int configId)
        {
            return
            dbContext.Set<Configs>()
                .Where(r => r.Id == configId && r.Env.AppId == appId)
                .Select(r => new ConfigModel
                {
                    Content = r.Content,
                    Desc = r.Desc,
                    Enabled = r.Enabled,
                    EnvId = r.EnvId,
                    EnvName = r.Env.Name,
                    EnvDesc = r.Env.Desc,
                    Id = r.Id,
                    Name = r.Name,
                    Type = r.Type,
                })
                .FirstOrDefault();
        }

        [HttpGet]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var config = dbContext.Set<Configs>().Include(r => r.Env.App).FirstOrDefault(r => r.Id == id && r.Env.AppId == appId);
            if (config == null)
                return NotFound();

            config.IsDeleted = true;
            config.DeleteTime = DateTime.Now;
            await Modify(id, config.EnvId, config.Name, $"删除配置:{config.Name}[{config.Desc}]", "", "");
            dbContext.SaveChanges();

            return RedirectToAction("index", "app");
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secretKey"></param>
        /// <param name="env"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Get([FromForm] ConfigGetModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.config.IsNullOrWhiteSpace())
            {
                var configEntity = dbContext.Set<Envs>()
                    .Where(r => r.AppId == model.appId && r.Name == model.env)
                    .Select(r => new
                    {
                        EnvName = r.Name,
                        IpWhites = r.IpWhites.Select(f => f.Ip).ToList(),
                        SecretKey = r.Key,
                        AppId = r.App.Id,
                        Configs = r.Configs.OrderBy(f => f.Id).Where(f => f.Enabled).Select(f => new ConfigApiModel
                        {
                            name = f.Name,
                            content = f.Content,
                            type = f.Type
                        }),
                    })
                    .FirstOrDefault();

                if (configEntity == null)
                    return NotFound();

                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                if (configEntity.IpWhites.Any(r => r == "*" || r == ip))
                {
                    var helper = new SignHelper(configEntity.SecretKey);
                    if (!helper.IsValidSignModel(model))
                        return BadRequest("sign error");

                    var ps = new Dictionary<string, string>();
                    var random = Guid.NewGuid().ToString("n");
                    var data = configEntity.Configs.ToJson();
                    ps.Add("random", random);
                    ps.Add("data", data);
                    var sign = helper.BuildSign(ps);

                    return Json(new
                    {
                        random,
                        data,
                        sign
                    });
                }
                else
                    return BadRequest($"invalid ip:{ip}");
            }
            else
            {
                var configEntity =
                dbContext.Set<Configs>()
                    .Where(r => r.Env.AppId == model.appId && r.Name == model.config && r.Env.Name == model.env)
                    .Select(r => new
                    {
                        r.Env.AppId,
                        r.Env.Key,
                        IpWhites = r.Env.IpWhites.Select(f => f.Ip).ToList(),
                        ConfigModel = new ConfigApiModel
                        {
                            content = r.Content,
                            name = r.Name,
                            type = r.Type
                        },
                    })
                    .FirstOrDefault();

                if (configEntity == null)
                    return NotFound();

                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                if (configEntity.IpWhites.Any(r => r == "*" || r == ip))
                {
                    var helper = new SignHelper(configEntity.Key);
                    if (!helper.IsValidSignModel(model))
                        return BadRequest("sign error");

                    var ps = new Dictionary<string, string>();
                    var random = Guid.NewGuid().ToString("n");
                    ps.Add("random", random);
                    ps.Add("name", configEntity.ConfigModel.name);
                    ps.Add("type", configEntity.ConfigModel.type);
                    ps.Add("content", configEntity.ConfigModel.content);

                    var sign = helper.BuildSign(ps);

                    return Json(new
                    {
                        random,
                        sign,
                        configEntity.ConfigModel.name,
                        configEntity.ConfigModel.type,
                        configEntity.ConfigModel.content
                    });
                }
                else
                    return BadRequest("ip error");
            }
        }

        [HttpGet]
        [HttpPut]
        public async Task<IActionResult> Enabled(int id)
        {
            var config = dbContext.Set<Configs>()
                 .Include(r => r.Env.App)
                 .FirstOrDefault(r => r.Env.AppId == appId && r.Id == id);
            if (config == null)
                return NotFound();
            config.Enabled = true;

            await Modify(id, config.EnvId, config.Name, $"启用配置:{config.Name}[{config.Desc}]", "", "");
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpGet]
        [HttpPut]
        public async Task<IActionResult> Disabled(int id)
        {
            var config = dbContext.Set<Configs>()
                .Include(r => r.Env.App)
                .FirstOrDefault(r => r.Env.AppId == appId && r.Id == id);
            if (config == null)
                return NotFound();
            config.Enabled = false;

            await Modify(id, config.EnvId, config.Name, $"禁用配置:{config.Name}[{config.Desc}]", "", "");
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index), new { id });
        }

        /// <summary>
        /// 配置内容修改
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        async Task Modify(int configId, int envId, string configName, string desc, string before, string after)
        {
            dbContext.Add(new ModifyRecords
            {
                ConfigId = configId,
                Desc = desc,
                ModifyTime = DateTime.Now,
                BeforeContent = before,
                AfterContent = after
            });
            var env = dbContext.Set<Envs>().Where(r => r.Id == envId).Select(r => r.Name).FirstOrDefault();
            await webSocketContext.SendAsync(appId, env, "refresh", new { config = configName });
        }

        /// <summary>
        /// 查看修改记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="envId"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ModifyRecords(int pageIndex, int? envId, int? configId)
        {
            if (pageIndex < 1)
                pageIndex = 1;

            var query =
            dbContext
                .Set<ModifyRecords>()
                .IgnoreQueryFilters()
                .Where(r => r.Config.Env.AppId == appId)
                .WhereIf(envId.HasValue, r => r.Config.EnvId == envId)
                .WhereIf(configId.HasValue, r => r.ConfigId == configId);

            var total = await query.CountAsync();
            var list = query
                .OrderByDescending(r => r.Id)
                .Paging(pageIndex, 20)
                .Select(r => new ModifyRecordModel
                {
                    Id = r.Id,
                    AfterContent = r.AfterContent,
                    BeforeContent = r.BeforeContent,
                    ConfigDesc = r.Config.Desc,
                    ConfigId = r.ConfigId,
                    ConfigName = r.Config.Name,
                    Desc = r.Desc,
                    ModifyTime = r.ModifyTime
                })
                .ToList();

            ViewData["Total"] = total;
            ViewData["PageIndex"] = pageIndex;
            ViewData["EnvId"] = envId.HasValue ? envId.ToString() : "";
            ViewData["ConfigId"] = configId.HasValue ? configId.ToString() : "";
            return View(list);
        }

        /// <summary>
        /// 内容比较
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Compare(int id)
        {
            if (!dbContext.Set<ModifyRecords>().Any(r => r.Id == id))
                return NotFound();

            ViewData["Id"] = id;
            return View();
        }

        /// <summary>
        /// 内容比较
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCompareContent(int id)
        {
            var record = await dbContext
                .Set<ModifyRecords>()
                .IgnoreQueryFilters()
                .Where(r => r.Id == id)
                .Select(r => new ModifyRecordModel
                {
                    Id = r.Id,
                    AfterContent = r.AfterContent,
                    BeforeContent = r.BeforeContent,
                    ConfigDesc = r.Config.Desc,
                    Type = r.Config.Type,
                    ConfigId = r.ConfigId,
                    ConfigName = r.Config.Name,
                    Desc = r.Desc,
                    ModifyTime = r.ModifyTime
                })
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            return Json(record);
        }
    }
}