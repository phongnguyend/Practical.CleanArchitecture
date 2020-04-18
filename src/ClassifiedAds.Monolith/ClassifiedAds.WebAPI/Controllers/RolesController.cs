using AutoMapper;
using ClassifiedAds.Application;
using ClassifiedAds.Application.Roles.Commands;
using ClassifiedAds.Application.Roles.Queries;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.WebAPI.Models.Roles;
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
        private readonly IMapper _mapper;

        public RolesController(Dispatcher dispatcher, ILogger<RolesController> logger, IMapper mapper)
        {
            _dispatcher = dispatcher;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Role>> Get()
        {
            var roles = _dispatcher.Dispatch(new GetRolesQuery { AsNoTracking = true });
            var model = _mapper.Map<List<RoleModel>>(roles);
            return Ok(model);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Role> Get(Guid id)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id, AsNoTracking = true });
            var model = _mapper.Map<RoleModel>(role);
            return Ok(model);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Role>> Post([FromBody] RoleModel model)
        {
            var role = new Role
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
            };

            _dispatcher.Dispatch(new AddUpdateRoleCommand { Role = role });

            model = _mapper.Map<RoleModel>(role);

            return Created($"/api/roles/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] RoleModel model)
        {
            var role = _dispatcher.Dispatch(new GetRoleQuery { Id = id });

            role.Name = model.Name;
            role.NormalizedName = model.Name.ToUpper();

            _dispatcher.Dispatch(new AddUpdateRoleCommand { Role = role });

            model = _mapper.Map<RoleModel>(role);

            return Ok(model);
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