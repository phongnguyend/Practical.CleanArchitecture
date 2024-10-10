using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Csv;

public interface ICsvWriter<T>
    where T : ICsvRequest
{
    Task WriteAsync(T data, Stream stream);
}

public interface ICsvRequest
{
}