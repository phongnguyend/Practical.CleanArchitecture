namespace ClassifiedAds.WebMVC.Models.Home
{
    public class TokenModel
    {
        public string IdentityToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresAt { get; set; }
    }
}
