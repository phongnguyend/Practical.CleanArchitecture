using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassifiedAds.Domain.Services
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> DbSet { get; }

        IQueryable<T> GetAll();
    }
}
