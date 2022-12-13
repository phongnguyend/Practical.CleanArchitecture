using System;

namespace ClassifiedAds.CrossCuttingConcerns.Caching
{
    public interface ICache
    {
        void Add(string key, object item, TimeSpan timeSpan);

        void Remove(string key);

        object Get(string key);
    }
}
