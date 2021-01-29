using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Events
{
    public interface IDomainEvents
    {
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
