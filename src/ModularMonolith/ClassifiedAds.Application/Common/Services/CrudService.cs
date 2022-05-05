using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application
{
    public class CrudService<T> : ICrudService<T>
        where T : AggregateRoot<Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        protected readonly IRepository<T, Guid> _repository;
        private readonly IDomainEvents _domainEvents;

        public CrudService(IRepository<T, Guid> repository, IDomainEvents domainEvents)
        {
            _unitOfWork = repository.UnitOfWork;
            _repository = repository;
            _domainEvents = domainEvents;
        }

        public Task<List<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            return _repository.ToListAsync(_repository.GetAll());
        }

        public Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            ValidationException.Requires(id != Guid.Empty, "Invalid Id");
            return _repository.FirstOrDefaultAsync(_repository.GetAll().Where(x => x.Id == id));
        }

        public async Task AddOrUpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id.Equals(default))
            {
                await AddAsync(entity, cancellationToken);
            }
            else
            {
                await UpdateAsync(entity, cancellationToken);
            }
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _domainEvents.DispatchAsync(new EntityCreatedEvent<T>(entity, DateTime.UtcNow), cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _domainEvents.DispatchAsync(new EntityUpdatedEvent<T>(entity, DateTime.UtcNow), cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _domainEvents.DispatchAsync(new EntityDeletedEvent<T>(entity, DateTime.UtcNow), cancellationToken);
        }
    }
}
