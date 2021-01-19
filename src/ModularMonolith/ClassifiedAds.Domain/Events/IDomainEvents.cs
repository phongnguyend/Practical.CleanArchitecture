using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Events
{
    public interface IDomainEvents
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}
