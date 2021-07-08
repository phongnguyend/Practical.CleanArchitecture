using System.Collections.Generic;
using System.IO;

namespace ClassifiedAds.CrossCuttingConcerns.Excel
{
    public interface IExcelReader<T>
    {
        IEnumerable<T> Read(Stream stream);
    }
}
