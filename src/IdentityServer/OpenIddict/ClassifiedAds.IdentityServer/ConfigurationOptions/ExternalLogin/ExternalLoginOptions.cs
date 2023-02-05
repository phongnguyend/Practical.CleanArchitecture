namespace ClassifiedAds.IdentityServer.ConfigurationOptions.ExternalLogin
{
    public class ExternalLoginOptions
    {
        public AzureActiveDirectoryOptions AzureActiveDirectory { get; set; }

        public MicrosoftOptions Microsoft { get; set; }

        public GoogleOptions Google { get; set; }

        public FacebookOptions Facebook { get; set; }
    }
}
