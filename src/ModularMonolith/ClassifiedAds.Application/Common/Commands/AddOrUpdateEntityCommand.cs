using ClassifiedAds.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application;

public class AddOrUpdateEntityCommand<TEntity> : ICommand
    where TEntity : Entity<Guid>, IAggregateRoot
{
    public AddOrUpdateEntityCommand(TEntity entity)
    {
        Entity = entity;
    }

    public TEntity Entity { get; set; }
}

internal class AddOrUpdateEntityCommandHandler<TEntity> : ICommandHandler<AddOrUpdateEntityCommand<TEntity>>
where TEntity : Entity<Guid>, IAggregateRoot
{
    private readonly ICrudService<TEntity> _crudService;

    public AddOrUpdateEntityCommandHandler(ICrudService<TEntity> crudService)
    {
        _crudService = crudService;
    }

    public async Task HandleAsync(AddOrUpdateEntityCommand<TEntity> command, CancellationToken cancellationToken = default)
    {
        await _crudService.AddOrUpdateAsync(command.Entity, cancellationToken);
    }
}
