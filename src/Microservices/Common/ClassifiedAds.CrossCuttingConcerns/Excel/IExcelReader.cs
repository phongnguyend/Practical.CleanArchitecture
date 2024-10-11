using System.IO;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.Excel;

public interface IExcelReader<T>
    where T : IExcelResponse
{
    Task<T> ReadAsync(Stream stream);
}

public interface IExcelResponse
{
}
