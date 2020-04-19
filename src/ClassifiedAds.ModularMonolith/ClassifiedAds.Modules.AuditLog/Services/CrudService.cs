using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Modules.AuditLog.Services
{
    public class CrudService<T> : ICrudService<T>
        where T : AggregateRoot<Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        protected readonly AuditLogDbContext _dbContext;
        private readonly IDomainEvents _domainEvents;

        public CrudService(AuditLogDbContext dbContext, IDomainEvents domainEvents)
        {
            _unitOfWork = dbContext;
            _dbContext = dbContext;
            _domainEvents = domainEvents;
        }

        public virtual void AddOrUpdate(T entity)
        {
            var adding = entity.Id.Equals(default);

            _dbContext.Set<T>().Attach(entity);
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
            return _dbContext.Set<T>().ToList();
        }

        public virtual T GetById(Guid Id)
        {
            ValidationException.Requires(Id != Guid.Empty, "Invalid Id");
            return _dbContext.Set<T>().FirstOrDefault(x => x.Id == Id);
        }

        public virtual void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _unitOfWork.SaveChanges();
            _domainEvents.Dispatch(new EntityDeletedEvent<T>(entity, DateTime.Now));
        }
    }
}
