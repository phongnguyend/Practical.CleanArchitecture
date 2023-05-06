using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Grpc;
using ClassifiedAds.Services.Identity.Grpc;
using ClassifiedAds.Services.Storage.DTOs;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.Queries;

public class GetUsersQuery : IQuery<List<UserDTO>>
{
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeRoles { get; set; }
    public bool AsNoTracking { get; set; }
}

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, List<UserDTO>>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUsersQueryHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<UserDTO>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var token = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        var headers = new Metadata
        {
            { "Authorization", $"Bearer {token}" },
        };

        var client = new User.UserClient(ChannelFactory.Create(_configuration["Services:Identity:Grpc"]));
        var response = await client.GetUsersAsync(new GetUsersRequest(), headers);

        return response.Users.Select(x => new UserDTO
        {
            Id = Guid.Parse(x.Id),
            UserName = x.UserName,
            Email = x.Email,
        }).ToList();
    }
}
