namespace ClassifiedAds.Domain.Entities
{
    public interface ITrackable
    {
        byte[] RowVersion { get; set; }
    }
}
