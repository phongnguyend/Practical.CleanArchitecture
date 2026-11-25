using Foundation;
using LocalAuthentication;
using Security;
using System.Text;

namespace ClassifiedAds.Mobile;

public static partial class BiometricVault
{
    public static partial bool IsAvailable()
    {
        using var context = new LAContext();
        return context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out _);
    }

    public static partial Task Create()
    {
        if (!IsAvailable())
        {
            throw new InvalidOperationException("Enroll Face ID or Touch ID on this device first.");
        }

        return Task.CompletedTask;
    }

    private static SecRecord Query() => new(SecKind.GenericPassword)
    {
        Service = Alias,
        Account = "session",
        Synchronizable = false
    };

    public static partial Task Write(string value)
    {
        using var query = Query();
        using var record = Query();
        record.AccessControl = new SecAccessControl(SecAccessible.WhenPasscodeSetThisDeviceOnly,
            SecAccessControlCreateFlags.BiometryCurrentSet);
        record.ValueData = NSData.FromArray(Encoding.UTF8.GetBytes(value));
        // The old protected item must not survive a successful session rotation.
        SecKeyChain.Remove(query);
        var status = SecKeyChain.Add(record);
        if (status != SecStatusCode.Success)
        {
            throw new InvalidOperationException("Unable to protect the session. Please sign in again.");
        }

        return Task.CompletedTask;
    }

    public static partial Task<string> Read() => Task.Run(() =>
    {
        using var context = new LAContext { LocalizedReason = "Unlock your Classified Ads session" };
        using var query = Query();
        query.AuthenticationContext = context;
        using var result = SecKeyChain.QueryAsRecord(query, out var status);
        if (status != SecStatusCode.Success || result?.ValueData is null)
        {
            throw new InvalidOperationException("Biometric unlock cancelled or credentials unavailable. Try again or sign in.");
        }

        return Encoding.UTF8.GetString(result.ValueData.ToArray());
    });

    public static partial void Clear()
    {
        using var query = Query();
        SecKeyChain.Remove(query);
        Enabled = false;
    }
}
