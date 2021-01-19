﻿using ClassifiedAds.Application;
using ClassifiedAds.Services.Identity.Commands.Roles;
using ClassifiedAds.Services.Identity.DTOs;
using ClassifiedAds.Services.Identity.Entities;
using ClassifiedAds.Services.Identity.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Identity.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly Dispatcher _dispatcher;

        public RolesController(Dispatcher dispatcher, ILogger<RolesController> logger)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> Get()
        {
            var roles = await _dispatcher.DispatchAsync(new GetRolesQuery { AsNoTracking = true });
            var model = roles.ToDTOs();
            return Ok(model);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Role>> Get(Guid id)
        {
            var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id, AsNoTracking = true });
            var model = role.ToDTO();
            return Ok(model);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Role>> Post([FromBody] RoleDTO model)
        {
            var role = new Role
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
            };

            await _dispatcher.DispatchAsync(new AddUpdateRoleCommand { Role = role });

            model = role.ToDTO();

            return Created($"/api/roles/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] RoleDTO model)
        {
            var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id });

            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();

            await _dispatcher.DispatchAsync(new AddUpdateRoleCommand { Role = role });

            model = role.ToDTO();

            return Ok(model);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var role = await _dispatcher.DispatchAsync(new GetRoleQuery { Id = id });
            await _dispatcher.DispatchAsync(new DeleteRoleCommand { Role = role });

            return Ok();
        }
    }
}