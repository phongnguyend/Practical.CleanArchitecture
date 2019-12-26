using ClassifiedAds.CrossCuttingConcerns.Caching;
using System;

namespace ClassifiedAds.Infrastructure.Caching.Redis
{
    public class Cache : ICache
    {
        public void Add(string key, object item, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}
