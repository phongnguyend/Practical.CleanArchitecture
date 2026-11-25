namespace ClassifiedAds.Mobile;

public static partial class BiometricVault
{
    private const string Alias = "classifiedads.maui.biometric";

    public static bool Enabled
    {
        get => Preferences.Get(Alias + ".enabled", false);
        set => Preferences.Set(Alias + ".enabled", value);
    }

    public static partial bool IsAvailable();

    public static partial Task Create();

    public static partial Task Write(string value);

    public static partial Task<string> Read();

    public static partial void Clear();
}
