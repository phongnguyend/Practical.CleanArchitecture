namespace ClassifiedAds.Contracts.Identity.DTOs;

public class UserQueryOptions
{
    public bool IncludeClaims { get; set; }
    public bool IncludeUserRoles { get; set; }
    public bool IncludeRoles { get; set; }
    public bool IncludeTokens { get; set; }
    public bool AsNoTracking { get; set; }
}
