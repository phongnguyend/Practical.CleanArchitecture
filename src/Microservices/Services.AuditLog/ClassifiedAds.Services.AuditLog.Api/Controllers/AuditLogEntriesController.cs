using System.Collections.Generic;
using System.Threading.Tasks;
using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Services.AuditLog.Authorization.Policies.AuditLogs;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.Services.AuditLog.Controllers
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

        [AuthorizePolicy(typeof(GetAuditLogsPolicy))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> Get()
        {
            var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { });
            return Ok(logs);
        }
    }
}