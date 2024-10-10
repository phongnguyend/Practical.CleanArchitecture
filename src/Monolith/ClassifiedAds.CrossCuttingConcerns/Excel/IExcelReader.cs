using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Excel;

public interface IExcelReader<T>
{
    Task<T> ReadAsync(Stream stream);
}
