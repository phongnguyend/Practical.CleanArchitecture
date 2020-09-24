namespace ClassifiedAds.Domain.Infrastructure.Networking
{
    public interface IFileDownloader
    {
        void DownloadFile(string url, string path);
    }
}
