namespace ClassifiedAds.DomainServices.Infrastructure
{
    public interface IFileDownloader
    {
        void DownloadFile(string url, string path);
    }
}
