using ClassifiedAds.Domain.Infrastructure.Networking;
using System.Net;

namespace ClassifiedAds.Infrastructure.Networking
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
