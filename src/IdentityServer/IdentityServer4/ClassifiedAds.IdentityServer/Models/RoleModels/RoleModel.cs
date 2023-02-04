﻿using ClassifiedAds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.RoleModels
{
    public class RoleModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public static RoleModel FromEntity(Role role)
        {
            return new RoleModel { Id = role.Id, Name = role.Name };
        }
    }
}
