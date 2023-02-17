using Ling.EntityFrameworkCore.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ling.EntityFrameworkCore.Audit.TypeConfigurations;

internal sealed class AuditLogTypeConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
#if NET7_0_OR_GREATER
        builder.ToTable("AuditLogs", t => t.HasComment("A table that stores changes to entity properties."))
               .HasKey(al => al.Id);
#else
        builder.ToTable("AuditLogs")
               .HasComment("A table that stores changes to entity properties.");

        builder.HasKey(al => al.Id);
#endif

        builder.Property(al => al.Id)
               .HasComment("The primary key for this entity.");

        builder.Property(al => al.Schema)
               .IsUnicode(false)
               .HasMaxLength(64)
               .HasComment("The database schema name.");

        builder.Property(al => al.Table)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The table name.");

        builder.Property(al => al.PrimaryKey)
               .IsUnicode(false)
               .HasMaxLength(128)
               .HasComment("The primary key of changed entity.");

        builder.Property(al => al.EntityType)
               .IsUnicode(false)
               .HasMaxLength(64)
               .HasComment("The type of changed entity.");

        builder.Property(al => al.EventType)
               .IsUnicode(false)
               .HasMaxLength(16)
               .HasConversion<string>()
               .HasComment("The type of audit event.");

        builder.Property(al => al.EventTime)
               .HasComment("The time the audit event occurred.");

        builder.Property(al => al.OperatorId)
               .IsUnicode(true)
               .HasMaxLength(512)
               .IsRequired(false)
               .HasComment("The identity of the user who change entity.");

        builder.Property(al => al.OperatorName)
               .IsUnicode(true)
               .HasMaxLength(512)
               .IsRequired(false)
               .HasComment("The name of the user who change entity.");

        builder.HasMany(al => al.Details)
               .WithOne()
               .HasForeignKey(ald => ald.AuditLogId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
