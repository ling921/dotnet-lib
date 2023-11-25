namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Indicates that the entity is automatically audited.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AuditIncludeAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the operation type that allows anonymous.
    /// </summary>
    public AuditOperate AllowAnonymousOperate { get; init; } = AuditOperate.None;
}
