using ClassifiedAds.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application
{
    public class UpdateEntityCommand<TEntity> : ICommand
        where TEntity : AggregateRoot<Guid>
    {
        public UpdateEntityCommand(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }

    internal class UpdateEntityCommandHandler<TEntity> : ICommandHandler<UpdateEntityCommand<TEntity>>
    where TEntity : AggregateRoot<Guid>
    {
        private readonly ICrudService<TEntity> _crudService;

        public UpdateEntityCommandHandler(ICrudService<TEntity> crudService)
        {
            _crudService = crudService;
        }

        public async Task HandleAsync(UpdateEntityCommand<TEntity> command, CancellationToken cancellationToken = default)
        {
            await _crudService.UpdateAsync(command.Entity);
        }
    }
}
