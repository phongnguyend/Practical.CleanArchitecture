using ClassifiedAds.Application;
using ClassifiedAds.Application.Users.Commands;
using ClassifiedAds.Application.Users.Queries;
using ClassifiedAds.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class UsersController : ControllerBase
    {
        private readonly Dispatcher _dispatcher;
        private readonly UserManager<User> _userManager;

        public UsersController(Dispatcher dispatcher, UserManager<User> userManager, ILogger<UsersController> logger)
        {
            _dispatcher = dispatcher;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            var users = _dispatcher.Dispatch(new GetUsersQuery());
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> Get(Guid id)
        {
            var user = _dispatcher.Dispatch(new GetUserQuery { Id = id, AsNoTracking = true });
            return Ok(user);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<User>> Post([FromBody] User model)
        {
            User user = new User
            {
                UserName = model.UserName,
                NormalizedUserName = model.UserName.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                EmailConfirmed = model.EmailConfirmed,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = model.PhoneNumberConfirmed,
                TwoFactorEnabled = model.TwoFactorEnabled,
                LockoutEnabled = model.LockoutEnabled,
                LockoutEnd = model.LockoutEnd,
                AccessFailedCount = model.AccessFailedCount,
            };

            _ = await _userManager.CreateAsync(user);

            return Created($"/api/users/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] User model)
        {
            User user = _dispatcher.Dispatch(new GetUserQuery { Id = id });

            user.UserName = model.UserName;
            user.NormalizedUserName = model.UserName.ToUpper();
            user.Email = model.Email;
            user.NormalizedEmail = model.Email.ToUpper();
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;
            user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            user.TwoFactorEnabled = model.TwoFactorEnabled;
            user.LockoutEnabled = model.LockoutEnabled;
            user.LockoutEnd = model.LockoutEnd;
            user.AccessFailedCount = model.AccessFailedCount;

            _ = await _userManager.UpdateAsync(user);

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(Guid id)
        {
            var user = _dispatcher.Dispatch(new GetUserQuery { Id = id });
            _dispatcher.Dispatch(new DeleteUserCommand { User = user });

            return Ok();
        }
    }
}