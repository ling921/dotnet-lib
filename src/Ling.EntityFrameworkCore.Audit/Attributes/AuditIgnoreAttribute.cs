namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Indicates that the property or field will not be audited.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AuditIgnoreAttribute : Attribute
{
}
