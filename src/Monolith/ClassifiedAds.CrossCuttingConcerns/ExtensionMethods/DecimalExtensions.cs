namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods
{
    public static class DecimalExtensions
    {
        public static decimal Negate(this decimal d)
        {
            return decimal.Negate(d);
        }

        public static decimal? Negate(this decimal? d)
        {
            return d.HasValue ? decimal.Negate(d.Value) : (decimal?)null;
        }
    }
}
