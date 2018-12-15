using System;

namespace ClassifiedAds.ApplicationServices.Decorators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AuditLogAttribute : Attribute
    {
        public AuditLogAttribute()
        {
        }
    }
}
