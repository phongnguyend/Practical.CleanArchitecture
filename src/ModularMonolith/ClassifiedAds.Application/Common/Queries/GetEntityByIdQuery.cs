﻿using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Application
{
    public class GetEntityByIdQuery<TEntity> : IQuery<TEntity>
        where TEntity : AggregateRoot<Guid>
    {
        public Guid Id { get; set; }
        public bool ThrowNotFoundIfNull { get; set; }
    }

    internal class GetEntityByIdQueryHandler<TEntity> : IQueryHandler<GetEntityByIdQuery<TEntity>, TEntity>
    where TEntity : AggregateRoot<Guid>
    {
        private readonly IRepository<TEntity, Guid> _repository;

        public GetEntityByIdQueryHandler(IRepository<TEntity, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<TEntity> HandleAsync(GetEntityByIdQuery<TEntity> query)
        {
            var entity = await _repository.FirstOrDefaultAsync(_repository.GetAll().Where(x => x.Id == query.Id));

            if (query.ThrowNotFoundIfNull && entity == null)
            {
                throw new NotFoundException($"Entity {query.Id} not found.");
            }

            return entity;
        }
    }
}
