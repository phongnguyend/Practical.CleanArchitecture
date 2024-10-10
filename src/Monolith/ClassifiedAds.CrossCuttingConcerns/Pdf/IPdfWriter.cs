using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Pdf;

public interface IPdfWriter<T>
    where T : IPdfRequest
{
    Task WriteAsync(T data, Stream stream);

    Task<byte[]> GetBytesAsync(T data);
}

public interface IPdfRequest
{
}
