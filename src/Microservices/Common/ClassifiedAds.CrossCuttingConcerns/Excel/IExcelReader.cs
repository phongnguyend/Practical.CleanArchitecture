using System.IO;

namespace ClassifiedAds.CrossCuttingConcerns.Excel;

public interface IExcelReader<T>
{
    T Read(Stream stream);
}
