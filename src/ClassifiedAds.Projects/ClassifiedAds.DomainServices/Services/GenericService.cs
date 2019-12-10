using System;
using System.Collections.Generic;
using System.Linq;
using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices.Services
{
    public class GenericService<T> : IGenericService<T> where T : Entity<Guid>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IRepository<T> _repository;

        public GenericService(IUnitOfWork unitOfWork, IRepository<T> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public void Add(T entity)
        {
            _repository.Add(entity);
            _unitOfWork.SaveChanges();
        }

        public void Update(T entity)
        {
            _unitOfWork.SaveChanges();
        }

        public IList<T> Get()
        {
            return _repository.GetAll().ToList();
        }

        public T GetById(Guid Id)
        {
            return _repository.GetAll().FirstOrDefault(x => x.Id == Id);
        }
    }
}
