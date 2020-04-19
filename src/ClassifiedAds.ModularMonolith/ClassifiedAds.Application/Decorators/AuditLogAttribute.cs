using System;

namespace ClassifiedAds.Application.Decorators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AuditLogAttribute : Attribute
    {
        public AuditLogAttribute()
        {
        }
    }
}
