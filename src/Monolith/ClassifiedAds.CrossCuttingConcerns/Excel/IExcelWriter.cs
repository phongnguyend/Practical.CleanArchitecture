using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Excel;

public interface IExcelWriter<T>
    where T : IExcelRequest
{
    Task WriteAsync(T data, Stream stream);
}

public interface IExcelRequest
{
}