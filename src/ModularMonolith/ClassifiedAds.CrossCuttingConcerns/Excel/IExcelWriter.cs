using System.IO;

namespace ClassifiedAds.CrossCuttingConcerns.Excel;

public interface IExcelWriter<T>
{
    void Write(T data, Stream stream);
}
