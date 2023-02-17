using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Reflection;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal sealed class AuditIgnoreAttributeConvention : PropertyAttributeConventionBase<AuditIgnoreAttribute>
{
    public AuditIgnoreAttributeConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies)
    {
    }

    protected override void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        AuditIgnoreAttribute attribute,
        MemberInfo clrMember,
        IConventionContext context)
    {
        propertyBuilder.Metadata.SetAnnotation(AuditAnnotationNames.Include, false);
    }
}
