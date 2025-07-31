namespace ClassifiedAds.Modules.AuditLog.ConfigurationOptions;

public class ConnectionStringsOptions
{
    public string Default { get; set; }

    public string MigrationsAssembly { get; set; }

    public int? CommandTimeout { get; set; }
}
