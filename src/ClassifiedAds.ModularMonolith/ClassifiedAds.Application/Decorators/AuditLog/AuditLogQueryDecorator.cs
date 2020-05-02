using System;
using Newtonsoft.Json;

namespace ClassifiedAds.Application.Decorators.AuditLog
{
    public class AuditLogQueryDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;

        public AuditLogQueryDecorator(IQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Handle(TQuery query)
        {
            string queryJson = JsonConvert.SerializeObject(query);
            Console.WriteLine($"Query of type {query.GetType().Name}: {queryJson}");
            return _handler.Handle(query);
        }
    }
}