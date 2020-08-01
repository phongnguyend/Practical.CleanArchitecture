using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Application
{
    public class GetEntititesQuery<TEntity> : IQuery<List<TEntity>>
         where TEntity : AggregateRoot<Guid>
    {
    }

    internal class GetEntititesQueryHandler<TEntity> : IQueryHandler<GetEntititesQuery<TEntity>, List<TEntity>>
    where TEntity : AggregateRoot<Guid>
    {
        private readonly IRepository<TEntity, Guid> _repository;

        public GetEntititesQueryHandler(IRepository<TEntity, Guid> repository)
        {
            _repository = repository;
        }

        public List<TEntity> Handle(GetEntititesQuery<TEntity> query)
        {
            return _repository.GetAll().ToList();
        }
    }
}
