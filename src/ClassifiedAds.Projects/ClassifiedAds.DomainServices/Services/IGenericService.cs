
namespace ClassifiedAds.DomainServices.Services
{
    public interface IGenericService<T>
    {
        void Add(T entity);
    }
}
