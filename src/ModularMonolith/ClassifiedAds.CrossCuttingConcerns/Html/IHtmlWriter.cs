using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Html;

public interface IHtmlWriter<T>
    where T : IHtmlRequest
{
    Task WriteAsync(T data, Stream stream);

    Task<string> GetStringAsync(T data);
}

public interface IHtmlRequest
{
}