using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Excel;

public interface IExcelWriter<T>
{
    Task WriteAsync(T data, Stream stream);
}
