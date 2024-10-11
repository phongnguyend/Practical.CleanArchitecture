using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Csv;

public interface ICsvReader<T>
    where T : ICsvResponse
{
    Task<T> ReadAsync(Stream stream);
}

public interface ICsvResponse
{
}