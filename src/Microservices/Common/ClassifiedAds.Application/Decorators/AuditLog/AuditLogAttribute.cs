using System;

namespace ClassifiedAds.Application.Decorators.AuditLog;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class AuditLogAttribute : Attribute
{
}
