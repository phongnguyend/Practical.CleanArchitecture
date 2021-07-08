using System.Collections.Generic;
using System.IO;

namespace ClassifiedAds.CrossCuttingConcerns.Excel
{
    public interface IExcelWriter<T>
    {
        void Write(IEnumerable<T> collection, Stream stream);
    }
}
