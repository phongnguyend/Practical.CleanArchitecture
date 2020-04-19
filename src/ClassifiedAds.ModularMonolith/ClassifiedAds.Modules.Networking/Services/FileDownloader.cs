using ClassifiedAds.Modules.Networking.Contracts;
using System.Net;

namespace ClassifiedAds.Modules.Networking
{
    public class FileDownloader : IFileDownloader
    {
        public void DownloadFile(string url, string path)
        {
            var client = new WebClient();
            client.DownloadFile(url, path);
        }
    }
}
