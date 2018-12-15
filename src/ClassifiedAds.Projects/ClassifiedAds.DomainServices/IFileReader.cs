using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices
{
    public interface IFileReader
    {
        string Read(string path);
    }
}
