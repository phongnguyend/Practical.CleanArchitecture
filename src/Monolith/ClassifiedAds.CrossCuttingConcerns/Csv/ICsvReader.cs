using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Csv;

public interface ICsvReader<T>
{
    Task<T> ReadAsync(Stream stream);
}
