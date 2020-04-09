using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Domain.Services
{
    public class CrudService<T> : ICrudService<T>
        where T : AggregateRoot<Guid>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IRepository<T, Guid> _repository;

        public CrudService(IRepository<T, Guid> repository)
        {
            _unitOfWork = repository.UnitOfWork;
            _repository = repository;
        }

        public virtual void AddOrUpdate(T entity)
        {
            var adding = entity.Id.Equals(default);

            _repository.AddOrUpdate(entity);
            _unitOfWork.SaveChanges();

            if (adding)
            {
                DomainEvents.Dispatch(new EntityCreatedEvent<T>(entity));
            }
            else
            {
                DomainEvents.Dispatch(new EntityUpdatedEvent<T>(entity));
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
            DomainEvents.Dispatch(new EntityDeletedEvent<T>(entity));
        }
    }
}
