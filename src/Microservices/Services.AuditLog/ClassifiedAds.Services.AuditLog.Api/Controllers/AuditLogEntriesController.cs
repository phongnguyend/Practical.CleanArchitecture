using ClassifiedAds.Application;
using ClassifiedAds.Application.Common.DTOs;
using ClassifiedAds.Services.AuditLog.Authorization;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Queries;
using ClassifiedAds.Services.AuditLog.RateLimiterPolicies;
using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.AuditLog.Controllers;

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
    [Authorize(AuthorizationPolicyNames.GetAuditLogsPolicy)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> Get()
    {
        var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { });
        return Ok(logs);
    }

    [Authorize(AuthorizationPolicyNames.GetAuditLogsPolicy)]
    [HttpGet("paged")]
    public async Task<ActionResult<Paged<AuditLogEntryDTO>>> GetPaged(int page, int pageSize)
    {
        var logs = await _dispatcher.DispatchAsync(new GetPagedAuditEntriesQuery { Page = page, PageSize = pageSize });
        return Ok(logs);
    }

    [AllowAnonymous]
    [HttpPost("")]
    [Topic("pubsub", "AuditLogCreatedEvent")]
    public async Task<IActionResult> ReceiveAuditLogCreatedEvent(AuditLogCreatedEvent auditLogEvent)
    {
        await Task.Delay(1);
        return Ok();
    }
}