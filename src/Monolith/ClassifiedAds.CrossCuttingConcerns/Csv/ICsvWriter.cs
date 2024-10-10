using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Csv;

public interface ICsvWriter<T>
{
    Task WriteAsync(T data, Stream stream);
}
