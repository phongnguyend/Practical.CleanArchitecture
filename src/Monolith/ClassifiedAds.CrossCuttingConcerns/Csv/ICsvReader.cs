using System.Collections.Generic;
using System.IO;

namespace ClassifiedAds.CrossCuttingConcerns.Csv;

public interface ICsvReader<T>
{
    IEnumerable<T> Read(Stream stream);
}
