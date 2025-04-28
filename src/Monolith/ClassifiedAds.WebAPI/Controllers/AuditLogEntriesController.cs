using ClassifiedAds.Application;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.AuditLogEntries.Queries;
using ClassifiedAds.Application.Common.DTOs;
using ClassifiedAds.WebAPI.Authorization;
using ClassifiedAds.WebAPI.RateLimiterPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Controllers;

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

    [EnableRateLimiting(RateLimiterPolicyNames.GetAuditLogsPolicy)]
    [Authorize(Permissions.GetAuditLogs)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> Get()
    {
        var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { });
        return Ok(logs);
    }

    [Authorize(Permissions.GetAuditLogs)]
    [HttpGet("paged")]
    public async Task<ActionResult<Paged<AuditLogEntryDTO>>> GetPaged(int page, int pageSize)
    {
        var logs = await _dispatcher.DispatchAsync(new GetPagedAuditEntriesQuery { Page = page, PageSize = pageSize });
        return Ok(logs);
    }
}