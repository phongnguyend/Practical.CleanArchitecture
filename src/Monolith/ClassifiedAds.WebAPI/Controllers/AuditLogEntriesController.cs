using ClassifiedAds.Application;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.AuditLogEntries.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogEntriesController : ControllerBase
    {
        private readonly Dispatcher _dispatcher;

        public AuditLogEntriesController(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> Get()
        {
            var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { });
            return Ok(logs);
        }
    }
}