using ClassifiedAds.DomainServices.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace ClassifiedAds.GraphQL
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _context;

        public CurrentUser(IHttpContextAccessor context)
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
                return Guid.Parse(_context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
        }
    }
}
