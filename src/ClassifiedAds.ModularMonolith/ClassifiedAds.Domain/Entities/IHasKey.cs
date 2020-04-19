namespace ClassifiedAds.Domain.Entities
{
    public interface IHasKey<T>
    {
        T Id { get; set; }
    }
}
