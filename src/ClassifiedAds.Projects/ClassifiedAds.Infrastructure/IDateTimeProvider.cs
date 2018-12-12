using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Infrastructure
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}
