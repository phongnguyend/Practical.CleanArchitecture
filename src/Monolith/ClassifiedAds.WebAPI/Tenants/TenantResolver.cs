using ClassifiedAds.CrossCuttingConcerns.Tenants;
using Microsoft.AspNetCore.Http;

namespace ClassifiedAds.WebAPI.Tenants
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _context;

        public TenantResolver(IHttpContextAccessor context)
        {
            _context = context;
        }

        public Tenant Tenant
        {
            get
            {
                var request = _context?.HttpContext?.Request;

                if (request is null)
                {
                    return null;
                }

                var scheme = request.Scheme;
                var host = request.Host.Host;
                var port = request.Host.Port;

                // TODO: query configuration store to get Tenant information
                return new Tenant
                {
                };
            }
        }
    }
}
