using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Ling.EntityFrameworkCore.Audit.Internal;

internal class AuditConventionSetPlugin : IConventionSetPlugin
{
    private readonly ProviderConventionSetBuilderDependencies _dependencies;

    public AuditConventionSetPlugin(ProviderConventionSetBuilderDependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public ConventionSet ModifyConventions(ConventionSet conventionSet)
    {
        conventionSet.EntityTypeAddedConventions.Add(new AuditIncludeAttributeConvention(_dependencies));
        conventionSet.PropertyAddedConventions.Add(new AuditIgnoreAttributeConvention(_dependencies));
        return conventionSet;
    }
}
