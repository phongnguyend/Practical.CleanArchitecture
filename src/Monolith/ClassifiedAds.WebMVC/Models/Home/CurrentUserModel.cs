using System.Collections.Generic;

namespace ClassifiedAds.WebMVC.Models.Home
{
    public class CurrentUserModel
    {
        public CurrentUserIdentityModel Identity { get; set; }
        public List<ClaimModel> Claims { get; set; }
    }

    public class CurrentUserIdentityModel
    {
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
    }

    public class ClaimModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
