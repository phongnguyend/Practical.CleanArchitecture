using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Application
{
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }
}
