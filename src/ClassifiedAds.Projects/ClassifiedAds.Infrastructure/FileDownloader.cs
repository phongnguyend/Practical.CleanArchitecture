using ClassifiedAds.DomainServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ClassifiedAds.Infrastructure
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
