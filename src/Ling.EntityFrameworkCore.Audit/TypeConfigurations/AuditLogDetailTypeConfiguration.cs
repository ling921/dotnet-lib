using Ling.EntityFrameworkCore.Audit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ling.EntityFrameworkCore.Audit.TypeConfigurations;

internal sealed class AuditLogDetailTypeConfiguration : IEntityTypeConfiguration<AuditLogDetail>
{
    public void Configure(EntityTypeBuilder<AuditLogDetail> builder)
    {
#if NET7_0_OR_GREATER
        builder.ToTable("AuditLogDetails", t => t.HasComment("A table to store entity changes."))
               .HasKey(al => al.Id);
#else
        builder.ToTable("AuditLogDetails")
               .HasComment("A table to store entity changes.");

        builder.HasKey(al => al.Id);
#endif

        builder.Property(al => al.Id)
               .HasComment("The primary key of changed entity.");

        builder.Property(al => al.AuditLogId)
               .HasComment("The primary key of the AuditLog that the detail belongs to.");

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
