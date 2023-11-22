using Ling.EntityFrameworkCore.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ling.EntityFrameworkCore.Audit.TypeConfigurations;

internal sealed class AuditFieldLogTypeConfiguration : IEntityTypeConfiguration<AuditFieldLog>
{
    public void Configure(EntityTypeBuilder<AuditFieldLog> builder)
    {
#if NET7_0_OR_GREATER
        builder.ToTable("AuditLogDetails", t => t.HasComment("A table to record entity's fields changes."))
               .HasKey(al => al.Id);
#else
        builder.ToTable(AuditDefaults.EntityFieldChangeAuditLogTableName)
               .HasComment("A table to record entity's fields changes.");

        builder.HasKey(al => al.Id);
#endif

        builder.Property(al => al.Id)
               .ValueGeneratedOnAdd()
               .HasComment("The primary key.");

        builder.Property(al => al.EntityLogId)
               .HasComment("The entity-change-log primary key.");

        builder.Property(al => al.PropertyName)
               .IsUnicode(false)
               .HasMaxLength(64)
               .HasComment("The property name (format: entity class name + '.' + property name).");

        builder.Property(al => al.OriginalValue)
               .IsUnicode(true)
               .HasComment("The original value.");

        builder.Property(al => al.NewValue)
               .IsUnicode(true)
               .HasComment("The new value.");
    }
}
