using ClassifiedAds.Modules.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Identity.MappingConfigurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

            builder.HasMany(x => x.Claims)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed
            builder.HasData(new List<User>
            {
                new User
                {
                    Id = Guid.Parse("12837D3D-793F-EA11-BECB-5CEA1D05F660"),
                    UserName = "phong@gmail.com",
                    NormalizedUserName = "PHONG@GMAIL.COM",
                    Email = "phong@gmail.com",
                    NormalizedEmail = "PHONG@GMAIL.COM",
                    PasswordHash = "AQAAAAEAACcQAAAAELBcKuXWkiRQEYAkD/qKs9neac5hxWs3bkegIHpGLtf+zFHuKnuI3lBqkWO9TMmFAQ==", // v*7Un8b4rcN@<-RN
                    SecurityStamp = "5M2QLL65J6H6VFIS7VZETKXY27KNVVYJ",
                    LockoutEnabled = true,
                },
            });
        }
    }
}
