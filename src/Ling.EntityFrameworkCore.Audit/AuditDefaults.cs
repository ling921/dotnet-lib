namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Define the audit configuration items.
/// </summary>
public static class AuditDefaults
{
    /// <summary>
    /// The key of the <see cref="AppContext"/> used to turn auditing on or off.
    /// </summary>
    public const string DisabledSwitchKey = "Ling.DisableAudit";

    /// <summary>
    /// Table name that records entity change audit logs.
    /// <para>DO NOT change this value if migrations are added.</para>
    /// </summary>
    public static string EntityChangeAuditLogTableName { get; set; } = "AuditEntityLog";

    /// <summary>
    /// The table name that records entity's fields change audit logs.
    /// <para>DO NOT change this value if migrations are added.</para>
    /// </summary>
    public static string EntityFieldChangeAuditLogTableName { get; set; } = "AuditFieldLog";
}
