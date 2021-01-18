using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<List<T>> GetAsync()
        {
            return _repository.ToListAsync(_repository.GetAll());
        }

        public Task<T> GetByIdAsync(Guid Id)
        {
            ValidationException.Requires(Id != Guid.Empty, "Invalid Id");
            return _repository.FirstOrDefaultAsync(_repository.GetAll().Where(x => x.Id == Id));
        }

        public async Task AddOrUpdateAsync(T entity)
        {
            var adding = entity.Id.Equals(default);

            await _repository.AddOrUpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            if (adding)
            {
                await _domainEvents.DispatchAsync(new EntityCreatedEvent<T>(entity, DateTime.UtcNow));
            }
            else
            {
                await _domainEvents.DispatchAsync(new EntityUpdatedEvent<T>(entity, DateTime.UtcNow));
            }
        }

        public async Task DeleteAsync(T entity)
        {
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            await _domainEvents.DispatchAsync(new EntityDeletedEvent<T>(entity, DateTime.UtcNow));
        }
    }
}
