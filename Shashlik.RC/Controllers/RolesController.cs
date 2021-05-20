﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Identity;
using Shashlik.RC.Services.Identity.Dtos;
using Shashlik.RC.Services.Identity.Inputs;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Resource;
using Shashlik.RC.Services.Resource.Dtos;
using Shashlik.Response;

namespace Shashlik.RC.Controllers
{
    public class RolesController : ApiControllerBase
    {
        public RolesController(RoleService roleService, ResourceService resourceService)
        {
            RoleService = roleService;
            ResourceService = resourceService;
        }

        private RoleService RoleService { get; }
        ResourceService ResourceService { get; }

        [HttpPost, Admin]
        public async Task Create(CreateRoleInput input)
        {
            await RoleService.CreateAsync(new IdentityRole<int>()
            {
                Name = input.Name
            });
        }

        [HttpGet, Admin]
        public async Task<List<RoleDto>> Get()
        {
            return await RoleService.List();
        }

        [HttpDelete("{roleName}"), Admin]
        public async Task Delete([Required, StringLength(32)] string roleName)
        {
            var role = await RoleService.FindByNameAsync(roleName);
            if (role is null)
                throw ResponseException.NotFound();

            var res = await RoleService.DeleteAsync(role);
            if (!res.Succeeded)
                throw ResponseException.ArgError(res.ToString());
        }

        [HttpGet("{roleName}/resources"), Admin]
        public async Task<IEnumerable<ResourceActionDto>> Resources(string roleName)
        {
            return await ResourceService.GetResourceActionsByRole(roleName);
        }
    }
}