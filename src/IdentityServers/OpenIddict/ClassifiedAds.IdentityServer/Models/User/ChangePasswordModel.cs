using System;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.IdentityServer.Models.User;

public class ChangePasswordModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    public static ChangePasswordModel FromEntity(Domain.Entities.User user)
    {
        return new ChangePasswordModel
        {
            Id = user.Id,
            UserName = user.UserName,
        };
    }
}
