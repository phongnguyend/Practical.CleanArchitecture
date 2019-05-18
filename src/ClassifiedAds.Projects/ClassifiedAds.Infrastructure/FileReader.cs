using ClassifiedAds.DomainServices.Infrastructure;
using System.IO;

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
