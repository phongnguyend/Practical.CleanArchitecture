using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.Repositories
{
    public interface IUnitOfWork
    {
        void SaveChanges();
    }
}
