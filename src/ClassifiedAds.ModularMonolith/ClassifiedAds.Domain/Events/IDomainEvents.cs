namespace ClassifiedAds.Domain.Events
{
    public interface IDomainEvents
    {
        void Dispatch(IDomainEvent domainEvent);
    }
}
