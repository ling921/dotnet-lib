using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal class AuditIncludeAttributeConvention : EntityTypeAttributeConventionBase<AuditIncludeAttribute>
{
    public AuditIncludeAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
    {
    }

    protected override void ProcessEntityTypeAdded(
        IConventionEntityTypeBuilder entityTypeBuilder,
        AuditIncludeAttribute attribute,
        IConventionContext<IConventionEntityTypeBuilder> context)
    {
        entityTypeBuilder.Metadata.SetAnnotation(AuditAnnotationNames.Include, true);
    }
}
