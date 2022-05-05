using ClassifiedAds.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application
{
    public class AddEntityCommand<TEntity> : ICommand
        where TEntity : AggregateRoot<Guid>
    {
        public AddEntityCommand(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }

    internal class AddEntityCommandHandler<TEntity> : ICommandHandler<AddEntityCommand<TEntity>>
    where TEntity : AggregateRoot<Guid>
    {
        private readonly ICrudService<TEntity> _crudService;

        public AddEntityCommandHandler(ICrudService<TEntity> crudService)
        {
            _crudService = crudService;
        }

        public async Task HandleAsync(AddEntityCommand<TEntity> command, CancellationToken cancellationToken = default)
        {
            await _crudService.AddAsync(command.Entity);
        }
    }
}
