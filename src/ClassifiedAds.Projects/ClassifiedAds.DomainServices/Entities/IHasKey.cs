namespace ClassifiedAds.DomainServices.Entities
{
    public interface IHasKey<T>
    {
        T Id { get; set; }
    }
}
