using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices
{
    public interface IFileDownloader
    {
        void DownloadFile(string url, string path);
    }
}
