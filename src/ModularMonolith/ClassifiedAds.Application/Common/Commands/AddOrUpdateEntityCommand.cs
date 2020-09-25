using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Application
{
    public class AddOrUpdateEntityCommand<TEntity> : ICommand
        where TEntity : AggregateRoot<Guid>
    {
        public AddOrUpdateEntityCommand(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }

    internal class AddOrUpdateEntityCommandHandler<TEntity> : ICommandHandler<AddOrUpdateEntityCommand<TEntity>>
    where TEntity : AggregateRoot<Guid>
    {
        private readonly ICrudService<TEntity> _crudService;

        public AddOrUpdateEntityCommandHandler(ICrudService<TEntity> crudService)
        {
            _crudService = crudService;
        }

        public void Handle(AddOrUpdateEntityCommand<TEntity> command)
        {
            _crudService.AddOrUpdate(command.Entity);
        }
    }
}
