using ClassifiedAds.Domain.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace ClassifiedAds.Infrastructure.Identity
{
    public class CurrentWebUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _context;

        public CurrentWebUser(IHttpContextAccessor context)
        {
            _context = context;
        }

        public bool IsAuthenticated
        {
            get
            {
                return _context.HttpContext.User.Identity.IsAuthenticated;
            }
        }

        public Guid UserId
        {
            get
            {
                var userId = _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? _context.HttpContext.User.FindFirst("sub")?.Value;

                return Guid.Parse(userId);
            }
        }
    }
}
