using Android.App;
using Android.Content;
using Android.Hardware.Biometrics;
using Android.OS;
using Android.Security.Keystore;
using Java.Security;
using Java.Security.Spec;
using Javax.Crypto;
using Javax.Crypto.Spec;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Runtime.Versioning;
using CipherMode = Javax.Crypto.CipherMode;

namespace ClassifiedAds.Mobile;

public static partial class BiometricVault
{
    private sealed record Envelope(string Key, string Iv, string Tag, string Data);

    private static KeyStore Store()
    {
        var store = KeyStore.GetInstance("AndroidKeyStore")!;
        store.Load(null);
        return store;
    }

    public static partial bool IsAvailable()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            return false;
        }

        var manager = (BiometricManager?)Android.App.Application.Context.GetSystemService(Context.BiometricService);
        return manager?.CanAuthenticate(15) == BiometricCode.Success;
    }

    public static partial Task Create()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(30) || !IsAvailable())
        {
            throw new InvalidOperationException("Enroll a strong biometric on this device first (Android 11 or later).");
        }

        using var generator = KeyPairGenerator.GetInstance("RSA", "AndroidKeyStore")!;
        using var spec = new KeyGenParameterSpec.Builder(Alias, KeyStorePurpose.Decrypt)
            .SetKeySize(2048)!
            .SetEncryptionPaddings(KeyProperties.EncryptionPaddingRsaOaep)!
            .SetDigests(KeyProperties.DigestSha256, KeyProperties.DigestSha1)!
            .SetUserAuthenticationRequired(true)!
            .SetUserAuthenticationParameters(0, 15)!
            .SetInvalidatedByBiometricEnrollment(true)!
            .Build()!;
        generator.Initialize(spec);
        generator.GenerateKeyPair();
        return Task.CompletedTask;
    }

    private static OAEPParameterSpec Oaep() => new("SHA-256", "MGF1", MGF1ParameterSpec.Sha1, PSource.PSpecified.Default);

    public static partial Task Write(string value)
    {
        var key = RandomNumberGenerator.GetBytes(32);
        try
        {
            using var aes = new AesGcm(key, 16);
            var iv = RandomNumberGenerator.GetBytes(12);
            var plaintext = Encoding.UTF8.GetBytes(value);
            var encrypted = new byte[plaintext.Length];
            var tag = new byte[16];
            aes.Encrypt(iv, plaintext, encrypted, tag);
            CryptographicOperations.ZeroMemory(plaintext);
            using var store = Store();
            using var wrapping = Cipher.GetInstance("RSA/ECB/OAEPWithSHA-256AndMGF1Padding")!;
            wrapping.Init(CipherMode.EncryptMode, store.GetCertificate(Alias)!.PublicKey, Oaep());
            Preferences.Set(Alias + ".data", JsonSerializer.Serialize(new Envelope(
                Convert.ToBase64String(wrapping.DoFinal(key)!), Convert.ToBase64String(iv),
                Convert.ToBase64String(tag), Convert.ToBase64String(encrypted))));
            return Task.CompletedTask;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }
    }

    public static partial async Task<string> Read()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(30) || !IsAvailable())
        {
            throw new InvalidOperationException("Biometrics unavailable. Sign in with Identity Server.");
        }

        var envelope = JsonSerializer.Deserialize<Envelope>(Preferences.Get(Alias + ".data", ""))!;
        using var store = Store();
        using var cipher = Cipher.GetInstance("RSA/ECB/OAEPWithSHA-256AndMGF1Padding")!;
        cipher.Init(CipherMode.DecryptMode, store.GetKey(Alias, null), Oaep());
        var completion = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var cancellation = new CancellationSignal();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                throw new PlatformNotSupportedException("Strong biometric login requires Android 11 or later.");
            }

            var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("Open the app to unlock.");
            var prompt = new BiometricPrompt.Builder(activity)
                .SetTitle("Unlock Classified Ads")!
                .SetAllowedAuthenticators(15)!
                .SetNegativeButton("Cancel", activity.MainExecutor!, new CancelListener(completion))!
                .Build()!;
            prompt.Authenticate(new BiometricPrompt.CryptoObject(cipher), cancellation,
                activity.MainExecutor!, new Callback(completion, Convert.FromBase64String(envelope.Key)));
        });
        var key = await completion.Task;
        try
        {
            using var aes = new AesGcm(key, 16);
            var encrypted = Convert.FromBase64String(envelope.Data);
            var plaintext = new byte[encrypted.Length];
            aes.Decrypt(Convert.FromBase64String(envelope.Iv), encrypted, Convert.FromBase64String(envelope.Tag), plaintext);
            var result = Encoding.UTF8.GetString(plaintext);
            CryptographicOperations.ZeroMemory(plaintext);
            return result;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }
    }

    private sealed class CancelListener(TaskCompletionSource<byte[]> completion) : Java.Lang.Object, IDialogInterfaceOnClickListener
    {
        public void OnClick(IDialogInterface? dialog, int which)
        {
            completion.TrySetException(new InvalidOperationException("Biometric unlock cancelled."));
        }
    }

    [SupportedOSPlatform("android28.0")]
    private sealed class Callback(TaskCompletionSource<byte[]> completion, byte[] wrapped) : BiometricPrompt.AuthenticationCallback
    {
        public override void OnAuthenticationError(BiometricErrorCode errorCode, Java.Lang.ICharSequence? message)
        {
            completion.TrySetException(new InvalidOperationException("Biometric unlock unavailable or cancelled. Try again or sign in."));
        }

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult? result)
        {
            try
            {
                completion.TrySetResult(result!.CryptoObject!.Cipher!.DoFinal(wrapped)!);
            }
            catch (Exception error)
            {
                completion.TrySetException(new InvalidOperationException("Credentials changed. Sign in again.", error));
            }
        }
    }

    public static partial void Clear()
    {
        using var store = Store();
        store.DeleteEntry(Alias);
        Preferences.Remove(Alias + ".data");
        Enabled = false;
    }
}
