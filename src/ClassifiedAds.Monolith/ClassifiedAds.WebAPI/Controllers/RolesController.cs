using ClassifiedAds.Application;
using ClassifiedAds.Application.Roles.Commands;
using ClassifiedAds.Application.Roles.Queries;
using ClassifiedAds.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Controllers
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
        public ActionResult<IEnumerable<Role>> Get()
        {
            var roles = _dispatcher.Dispatch(new GetRolesQuery { AsNoTracking = true });
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Role> Get(Guid id)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id, AsNoTracking = true });
            return Ok(role);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Role>> Post([FromBody] Role model)
        {
            var role = new Role
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
            };

            _dispatcher.Dispatch(new AddUpdateRoleCommand { Role = role });

            return Created($"/api/roles/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] Role model)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id });

            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();

            _dispatcher.Dispatch(new AddUpdateRoleCommand { Role = role });

            return Ok(role);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(Guid id)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id });
            _dispatcher.Dispatch(new DeleteRoleCommand { Role = role });

            return Ok();
        }
    }
}