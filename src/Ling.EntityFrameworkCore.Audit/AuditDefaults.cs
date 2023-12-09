using Microsoft.Extensions.Configuration;

namespace Ling.EntityFrameworkCore.Audit;

/// <summary>
/// Define the audit configuration items.
/// </summary>
public static class AuditDefaults
{
    /// <summary>
    /// The key of the <see cref="AppContext"/> for disable auditing. Setting this value to <see langword="true"/> will disable.
    /// </summary>
    public const string DisabledSwitchKey = "Ling.DisableAudit";

    /// <summary>
    /// The key for the <see cref="AuditOptions"/> configuration.
    /// It will be used to get the <see cref="AuditOptions"/> from <see cref="IConfiguration"/>'s section.
    /// </summary>
    public const string ConfigurationKey = "Audit";

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
