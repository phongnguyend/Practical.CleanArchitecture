using ClassifiedAds.DomainServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClassifiedAds.Infrastructure
{
    public class FileReader : IFileReader
    {
        public string Read(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
