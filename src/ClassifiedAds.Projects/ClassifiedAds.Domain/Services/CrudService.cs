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

        public CrudService(IUnitOfWork unitOfWork, IRepository<T, Guid> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public virtual void Add(T entity)
        {
            _repository.Add(entity);
            _unitOfWork.SaveChanges();
            DomainEvents.Dispatch(new EntityCreatedEvent<T>(entity));
        }

        public virtual void Update(T entity)
        {
            _unitOfWork.SaveChanges();
            DomainEvents.Dispatch(new EntityUpdatedEvent<T>(entity));
        }

        public virtual IList<T> Get()
        {
            return _repository.GetAll().ToList();
        }

        public virtual T GetById(Guid Id)
        {
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
