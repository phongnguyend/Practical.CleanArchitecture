namespace ClassifiedAds.Modules.Networking.Contracts
{
    public interface IFileDownloader
    {
        void DownloadFile(string url, string path);
    }
}
