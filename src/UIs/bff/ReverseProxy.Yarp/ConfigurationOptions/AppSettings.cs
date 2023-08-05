namespace ReverseProxy.Yarp.ConfigurationOptions
{
    public class AppSettings
    {
        public OpenIdConnect? OpenIdConnect { get; set; }
    }

    public class OpenIdConnect
    {
        public string? Authority { get; set; }

        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }

        public bool RequireHttpsMetadata { get; set; }
    }
}
