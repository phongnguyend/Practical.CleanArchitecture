namespace ClassifiedAds.DomainServices.Entities
{
    public interface ITrackable
    {
        byte[] RowVersion { get; set; }
    }
}
