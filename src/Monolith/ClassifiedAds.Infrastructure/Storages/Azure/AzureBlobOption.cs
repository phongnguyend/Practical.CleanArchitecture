namespace ClassifiedAds.Infrastructure.Storages.Azure;

public class AzureBlobOption
{
    public string ConnectionString { get; set; }

    public string Container { get; set; }

    public string Path { get; set; }
}
