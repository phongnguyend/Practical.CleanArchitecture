using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Events
{
    public interface IDomainEventHandler<T>
           where T : IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}
