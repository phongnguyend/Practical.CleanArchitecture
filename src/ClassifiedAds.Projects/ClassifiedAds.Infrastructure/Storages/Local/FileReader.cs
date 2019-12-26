using ClassifiedAds.DomainServices.Infrastructure;
using System.IO;

namespace ClassifiedAds.Infrastructure.Storages.Local
{
    public class FileReader : IFileReader
    {
        public string Read(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
