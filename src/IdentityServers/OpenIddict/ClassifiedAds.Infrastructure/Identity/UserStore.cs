using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Identity;

public class UserStore : IUserStore<User>,
                         IUserPasswordStore<User>,
                         IUserSecurityStampStore<User>,
                         IUserEmailStore<User>,
                         IUserPhoneNumberStore<User>,
                         IUserTwoFactorStore<User>,
                         IUserLockoutStore<User>,
                         IUserAuthenticationTokenStore<User>,
                         IUserAuthenticatorKeyStore<User>,
                         IUserTwoFactorRecoveryCodeStore<User>,
                         IUserPasskeyStore<User>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<UserPasskey, Guid> _userPasskey;

    public UserStore(IUserRepository userRepository, IRepository<UserPasskey, Guid> userPasskey)
    {
        _unitOfWork = userRepository.UnitOfWork;
        _userRepository = userRepository;
        _userPasskey = userPasskey;
    }

    public void Dispose()
    {
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        user.PasswordHistories = new List<PasswordHistory>()
        {
            new PasswordHistory
            {
                PasswordHash = user.PasswordHash,
                CreatedDateTime = DateTimeOffset.Now,
            },
        };
        await _userRepository.AddOrUpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        _userRepository.Delete(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _userRepository.Get(new UserQueryOptions { IncludeTokens = true }).FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken: cancellationToken);
    }

    public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return _userRepository.Get(new UserQueryOptions { IncludeTokens = true }).FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId), cancellationToken: cancellationToken);
    }

    public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return _userRepository.Get(new UserQueryOptions { IncludeTokens = true }).FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken);
    }

    public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnabled);
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.LockoutEnd);
    }

    public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedEmail);
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.SecurityStamp ?? string.Empty);
    }

    public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        await _userRepository.AddOrUpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    private const string AuthenticatorStoreLoginProvider = "[AuthenticatorStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";

    public Task<string> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        var tokenEntity = user.Tokens.SingleOrDefault(
                l => l.TokenName == name && l.LoginProvider == loginProvider);
        return Task.FromResult(tokenEntity?.TokenValue);
    }

    public async Task SetTokenAsync(User user, string loginProvider, string name, string value, CancellationToken cancellationToken)
    {
        var tokenEntity = user.Tokens.SingleOrDefault(
                l => l.TokenName == name && l.LoginProvider == loginProvider);
        if (tokenEntity != null)
        {
            tokenEntity.TokenValue = value;
        }
        else
        {
            user.Tokens.Add(new UserToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                TokenName = name,
                TokenValue = value,
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        var tokenEntity = user.Tokens.SingleOrDefault(
                l => l.TokenName == name && l.LoginProvider == loginProvider);
        if (tokenEntity != null)
        {
            user.Tokens.Remove(tokenEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
    {
        return SetTokenAsync(user, AuthenticatorStoreLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
    }

    public Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
    {
        return GetTokenAsync(user, AuthenticatorStoreLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
    }

    public Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {
        var mergedCodes = string.Join(";", recoveryCodes);
        return SetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    public async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
    {
        var mergedCodes = await GetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
        var splitCodes = mergedCodes.Split(';');
        if (splitCodes.Contains(code))
        {
            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
    {
        var mergedCodes = await GetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? string.Empty;
        if (mergedCodes.Length > 0)
        {
            return mergedCodes.Split(';').Length;
        }

        return 0;
    }

    // https://github.com/dotnet/dotnet/blob/b0f34d51fccc69fd334253924abd8d6853fad7aa/src/aspnetcore/src/Identity/EntityFrameworkCore/src/UserStore.cs
    public async Task AddOrUpdatePasskeyAsync(User user, UserPasskeyInfo passkey, CancellationToken cancellationToken)
    {
        var userPasskey = await FindUserPasskeyByIdAsync(passkey.CredentialId, cancellationToken);
        if (userPasskey != null)
        {
            UpdateFromUserPasskeyInfo(userPasskey, passkey);
            await _userPasskey.UpdateAsync(userPasskey, cancellationToken);
        }
        else
        {
            userPasskey = CreateUserPasskey(user, passkey);
            await _userPasskey.AddAsync(userPasskey, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<UserPasskeyInfo>> GetPasskeysAsync(User user, CancellationToken cancellationToken)
    {
        var userId = user.Id;
        var passkeys = await _userPasskey.GetQueryableSet()
            .Where(p => p.UserId.Equals(userId))
            .Select(p => ToUserPasskeyInfo(p))
            .ToListAsync(cancellationToken);

        return passkeys;
    }

    public async Task<User> FindByPasskeyIdAsync(byte[] credentialId, CancellationToken cancellationToken)
    {
        var passkey = await FindUserPasskeyByIdAsync(credentialId, cancellationToken);
        if (passkey != null)
        {
            return await FindUserAsync(passkey.UserId, cancellationToken);
        }

        return null;
    }

    public async Task<UserPasskeyInfo> FindPasskeyAsync(User user, byte[] credentialId, CancellationToken cancellationToken)
    {
        var passkey = await FindUserPasskeyAsync(user.Id, credentialId, cancellationToken);
        return passkey == null ? null : ToUserPasskeyInfo(passkey!);
    }

    public async Task RemovePasskeyAsync(User user, byte[] credentialId, CancellationToken cancellationToken)
    {
        var passkey = await FindUserPasskeyAsync(user.Id, credentialId, cancellationToken);
        if (passkey != null)
        {
            _userPasskey.Delete(passkey);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private Task<UserPasskey?> FindUserPasskeyByIdAsync(byte[] credentialId, CancellationToken cancellationToken)
    {
        return _userPasskey.GetQueryableSet().SingleOrDefaultAsync(userPasskey => userPasskey.CredentialId.SequenceEqual(credentialId), cancellationToken);
    }

    private static UserPasskey CreateUserPasskey(User user, UserPasskeyInfo passkey)
    {
        return new UserPasskey
        {
            UserId = user.Id,
            CredentialId = passkey.CredentialId,
            Data = new()
            {
                PublicKey = passkey.PublicKey,
                Name = passkey.Name,
                CreatedAt = passkey.CreatedAt,
                Transports = passkey.Transports,
                SignCount = passkey.SignCount,
                IsUserVerified = passkey.IsUserVerified,
                IsBackupEligible = passkey.IsBackupEligible,
                IsBackedUp = passkey.IsBackedUp,
                AttestationObject = passkey.AttestationObject,
                ClientDataJson = passkey.ClientDataJson,
            }
        };
    }

    // https://github.com/dotnet/dotnet/blob/8faa66ec621faa4bc5ff9571abc104b767b3c602/src/aspnetcore/src/Identity/EntityFrameworkCore/src/IdentityUserPasskeyExtensions.cs
    public void UpdateFromUserPasskeyInfo(UserPasskey passkey, UserPasskeyInfo passkeyInfo)
    {
        passkey.Data.Name = passkeyInfo.Name;
        passkey.Data.SignCount = passkeyInfo.SignCount;
        passkey.Data.IsBackedUp = passkeyInfo.IsBackedUp;
        passkey.Data.IsUserVerified = passkeyInfo.IsUserVerified;
    }

    public UserPasskeyInfo ToUserPasskeyInfo(UserPasskey passkey)
        => new(
            passkey.CredentialId,
            passkey.Data.PublicKey,
            passkey.Data.CreatedAt,
            passkey.Data.SignCount,
            passkey.Data.Transports,
            passkey.Data.IsUserVerified,
            passkey.Data.IsBackupEligible,
            passkey.Data.IsBackedUp,
            passkey.Data.AttestationObject,
            passkey.Data.ClientDataJson)
        {
            Name = passkey.Data.Name
        };

    private Task<User?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _userRepository.GetQueryableSet().SingleOrDefaultAsync(u => u.Id.Equals(userId), cancellationToken);
    }

    private Task<UserPasskey?> FindUserPasskeyAsync(Guid userId, byte[] credentialId, CancellationToken cancellationToken)
    {
        return _userPasskey.GetQueryableSet().SingleOrDefaultAsync(
            userPasskey => userPasskey.UserId.Equals(userId) && userPasskey.CredentialId.SequenceEqual(credentialId),
            cancellationToken);
    }
}
