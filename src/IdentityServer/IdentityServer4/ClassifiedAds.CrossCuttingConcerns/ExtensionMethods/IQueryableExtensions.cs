﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paged<T>(this IQueryable<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
