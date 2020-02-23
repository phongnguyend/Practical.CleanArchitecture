using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Domain
{
    public interface IUserService
    {
        User GetUserById(Guid Id);
    }
}
