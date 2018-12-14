using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Domain.Entities
{
    public interface Entity<T>
    {
        T Id { get; set; }
    }
}
