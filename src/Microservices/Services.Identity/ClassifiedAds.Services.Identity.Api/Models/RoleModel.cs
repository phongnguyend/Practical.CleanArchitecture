﻿using System;

namespace ClassifiedAds.Services.Identity.Models;

public class RoleModel
{
    public Guid Id { get; set; }

    public virtual string Name { get; set; }

    public virtual string NormalizedName { get; set; }

    public virtual string ConcurrencyStamp { get; set; }
}
