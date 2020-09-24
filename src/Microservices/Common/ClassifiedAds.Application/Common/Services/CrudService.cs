using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public virtual void AddOrUpdate(T entity)
        {
            var adding = entity.Id.Equals(default);

            _repository.AddOrUpdate(entity);
            _unitOfWork.SaveChanges();

            if (adding)
            {
                _domainEvents.Dispatch(new EntityCreatedEvent<T>(entity, DateTime.Now));
            }
            else
            {
                _domainEvents.Dispatch(new EntityUpdatedEvent<T>(entity, DateTime.Now));
            }
        }

        public virtual IList<T> Get()
        {
            return _repository.GetAll().ToList();
        }

        public virtual T GetById(Guid Id)
        {
            ValidationException.Requires(Id != Guid.Empty, "Invalid Id");
            return _repository.GetAll().FirstOrDefault(x => x.Id == Id);
        }

        public virtual void Delete(T entity)
        {
            _repository.Delete(entity);
            _unitOfWork.SaveChanges();
            _domainEvents.Dispatch(new EntityDeletedEvent<T>(entity, DateTime.Now));
        }
    }
}
