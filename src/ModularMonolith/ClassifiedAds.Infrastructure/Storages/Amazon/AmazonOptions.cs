using Amazon.S3;

namespace ClassifiedAds.Infrastructure.Storages.Amazon;

public class AmazonOptions
{
    public string AccessKeyID { get; set; }

    public string SecretAccessKey { get; set; }

    public string BucketName { get; set; }

    public string Path { get; set; }

    public string RegionEndpoint { get; set; }

    public AmazonS3Client CreateAmazonS3Client()
    {
        var regionEndpoint = global::Amazon.RegionEndpoint.GetBySystemName(RegionEndpoint);

        if (!string.IsNullOrWhiteSpace(AccessKeyID))
        {
            return new AmazonS3Client(AccessKeyID, SecretAccessKey, regionEndpoint);
        }

        return new AmazonS3Client(regionEndpoint);
    }
}
