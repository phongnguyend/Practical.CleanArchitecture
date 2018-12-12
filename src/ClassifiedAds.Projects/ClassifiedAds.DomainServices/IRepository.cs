using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassifiedAds.DomainServices
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> DbSet { get; }

        IQueryable<T> GetAll();
    }
}
