using Ling.Audit.Extensions;
using Ling.EntityFrameworkCore.Audit.Generator;
using Ling.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Ling.Audit.SourceGeneration;

public sealed partial class AuditGenerator
{
    private sealed class Parser
    {
        #region Private Fields

        private const string IHasCreationTimeFullName = "Ling.Audit.IHasCreationTime";
        private const string IHasCreatorFullName = "Ling.Audit.IHasCreator`1";
        private const string IHasModificationTimeFullName = "Ling.Audit.IHasModificationTime";
        private const string IHasModifierFullName = "Ling.Audit.IHasModifier`1";
        private const string ISoftDeleteFullName = "Ling.Audit.ISoftDelete";
        private const string IHasDeletionTimeFullName = "Ling.Audit.IHasDeletionTime";
        private const string IHasDeleterFullName = "Ling.Audit.IHasDeleter`1";

        private const string DateTimeOffsetTypeRef = "global::System.DateTimeOffset";
        private const string BooleanTypeRef = "global::System.Boolean";

        private readonly Compilation _compilation;

        private readonly AuditSourceGenerationContext _sourceGenerationContext;

        private readonly MetadataLoadContextInternal _metadataLoadContext;

        private readonly Type? _hasCreationTimeInterfaceType;
        private readonly Type? _hasCreatorInterfaceType;
        private readonly Type? _hasModificationTimeInterfaceType;
        private readonly Type? _hasModifierInterfaceType;
        private readonly Type? _hasSoftDeleteInterfaceType;
        private readonly Type? _hasDeletionTimeInterfaceType;
        private readonly Type? _hasDeleterInterfaceType;

        #endregion Private Fields

        #region Internal Constructors

        internal Parser(Compilation compilation, in AuditSourceGenerationContext sourceGenerationContext)
        {
            _compilation = compilation;
            _sourceGenerationContext = sourceGenerationContext;
            _metadataLoadContext = new MetadataLoadContextInternal(_compilation);

            _hasCreationTimeInterfaceType = _metadataLoadContext.Resolve(IHasCreationTimeFullName);
            _hasCreatorInterfaceType = _metadataLoadContext.Resolve(IHasCreatorFullName);
            _hasModificationTimeInterfaceType = _metadataLoadContext.Resolve(IHasModificationTimeFullName);
            _hasModifierInterfaceType = _metadataLoadContext.Resolve(IHasModifierFullName);
            _hasSoftDeleteInterfaceType = _metadataLoadContext.Resolve(ISoftDeleteFullName);
            _hasDeletionTimeInterfaceType = _metadataLoadContext.Resolve(IHasDeletionTimeFullName);
            _hasDeleterInterfaceType = _metadataLoadContext.Resolve(IHasDeleterFullName);

            Debug.Assert(_hasCreationTimeInterfaceType is not null);
            Debug.Assert(_hasCreatorInterfaceType is not null);
            Debug.Assert(_hasModificationTimeInterfaceType is not null);
            Debug.Assert(_hasModifierInterfaceType is not null);
            Debug.Assert(_hasSoftDeleteInterfaceType is not null);
            Debug.Assert(_hasDeletionTimeInterfaceType is not null);
            Debug.Assert(_hasDeleterInterfaceType is not null);
        }

        #endregion Internal Constructors

        #region Internal Methods

        internal static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } cds &&
                !cds.Modifiers.Any(st => st.IsKind(SyntaxKind.StaticKeyword));
        }

        internal static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return context.Node as ClassDeclarationSyntax;
        }

        internal SourceGenerationSpec? GetGenerationSpec(ImmutableArray<ClassDeclarationSyntax> classDeclarationSyntaxList, CancellationToken cancellationToken)
        {
            var contextGenSpecList = new List<ContextGenerationSpec>();
            foreach (IGrouping<SyntaxTree, ClassDeclarationSyntax> group in classDeclarationSyntaxList.GroupBy(c => c.SyntaxTree))
            {
                var syntaxTree = group.Key;
                var compilationSemanticModel = _compilation.GetSemanticModel(syntaxTree);
                var compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot(cancellationToken);

                foreach (var classDeclarationSyntax in group)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var contextTypeSymbol = compilationSemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken);
                    Debug.Assert(contextTypeSymbol is not null);

                    var contextLocation = contextTypeSymbol!.Locations.FirstOrDefault() ?? Location.None;

                    var classDeclarationList = contextTypeSymbol.GetTypeDeclarationList();
                    if (classDeclarationList.Count == 0)
                    {
                        _sourceGenerationContext.ReportDiagnostic(Diagnostics.InvalidTypeDeclareDeclaration(contextLocation));
                        continue;
                    }

                    var unimplementedInterfaceTypeSymbols = contextTypeSymbol.GetUnimplementedInterfaceTypeSymbols();
                    if (!unimplementedInterfaceTypeSymbols.Any())
                    {
                        continue;
                    }

                    var baseTypeLocations = classDeclarationSyntax.BaseList!.Types
                        .Select(t => new
                        {
                            BaseTypeSymbol = compilationSemanticModel.GetTypeInfo(t.Type, cancellationToken).Type as INamedTypeSymbol,
                            Location = t.GetLocation()
                        });

                    var propertyGenSpecList = new List<PropertyGenerationSpec>();

                    foreach (var (interfaceTypeSymbol, innerInterfaceTypeSymbols) in unimplementedInterfaceTypeSymbols)
                    {
                        var interfaceType = interfaceTypeSymbol.AsType(_metadataLoadContext)!;
                        var location = baseTypeLocations.FirstOrDefault(i => interfaceTypeSymbol.IsEqualsTo(i.BaseTypeSymbol))?.Location ?? Location.None;

                        foreach (var innerInterfaceTypeSymbol in innerInterfaceTypeSymbols)
                        {
                            var innerInterfaceType = innerInterfaceTypeSymbol.AsType(_metadataLoadContext)!;

                            if (_hasCreationTimeInterfaceType!.Equals(innerInterfaceType))
                            {
                                propertyGenSpecList.Add(GetCreationTimePropertySpec(location, interfaceType));
                            }
                            else if (innerInterfaceType.GetCompatibleGenericBaseClass(_hasCreatorInterfaceType!) != null)
                            {
                                var userKeyType = innerInterfaceType.GetGenericArguments().FirstOrDefault();
                                if (userKeyType is null)
                                {
                                    _sourceGenerationContext.ReportDiagnostic(Diagnostics.UnknownUserKeyType(location, interfaceType));
                                    break;
                                }
                                propertyGenSpecList.Add(GetCreatorIdPropertySpec(userKeyType, location, interfaceType));
                            }
                            else if (_hasModificationTimeInterfaceType!.Equals(innerInterfaceType))
                            {
                                propertyGenSpecList.Add(GetLastModificationTimePropertySpec(location, interfaceType));
                            }
                            else if (innerInterfaceType.GetCompatibleGenericBaseClass(_hasModifierInterfaceType!) != null)
                            {
                                var userKeyType = innerInterfaceType.GetGenericArguments().FirstOrDefault();
                                if (userKeyType is null)
                                {
                                    _sourceGenerationContext.ReportDiagnostic(Diagnostics.UnknownUserKeyType(location, interfaceType));
                                    break;
                                }
                                propertyGenSpecList.Add(GetLastModifierIdPropertySpec(userKeyType, location, interfaceType));
                            }
                            else if (_hasSoftDeleteInterfaceType!.Equals(innerInterfaceType))
                            {
                                propertyGenSpecList.Add(GetIsDeletedPropertySpec(location, interfaceType));
                            }
                            else if (_hasDeletionTimeInterfaceType!.Equals(innerInterfaceType))
                            {
                                propertyGenSpecList.Add(GetDeletionTimePropertySpec(location, interfaceType));
                            }
                            else if (innerInterfaceType.GetCompatibleGenericBaseClass(_hasDeleterInterfaceType!) != null)
                            {
                                var userKeyType = innerInterfaceType.GetGenericArguments().FirstOrDefault();
                                if (userKeyType is null)
                                {
                                    _sourceGenerationContext.ReportDiagnostic(Diagnostics.UnknownUserKeyType(location, interfaceType));
                                    break;
                                }
                                propertyGenSpecList.Add(GetDeleterIdPropertySpec(userKeyType, location, interfaceType));
                            }
                        }
                    }

                    if (propertyGenSpecList.Count == 0)
                    {
                        continue;
                    }

                    var classDeclaration = classDeclarationList[0];
                    if (classDeclaration[classDeclaration.Length - 2] != "class")
                    {
                        _sourceGenerationContext.ReportDiagnostic(Diagnostics.ImplementationMustBeClass(contextLocation));
                        continue;
                    }
                    if (!classDeclaration.Contains("partial"))
                    {
                        _sourceGenerationContext.ReportDiagnostic(Diagnostics.ClassMustBePartial(contextLocation));
                        continue;
                    }
                    if (classDeclaration.Contains("record"))
                    {
                        _sourceGenerationContext.ReportDiagnostic(Diagnostics.ClassCannotBeRecord(contextLocation));
                        continue;
                    }

                    var contextType = contextTypeSymbol.AsType(_metadataLoadContext)!;
                    contextGenSpecList.Add(new ContextGenerationSpec()
                    {
                        Location = contextLocation,
                        ContextType = contextType,
                        ContextClassDeclarationList = classDeclarationList.ConvertAll(i => i.Join(" ")),
                        GenerationType = new TypeGenerationSpec
                        {
                            Location = contextLocation,
                            ClassDeclaration = classDeclaration.Join(" "),
                            PropertyGenSpecList = propertyGenSpecList,
                            Type = contextType,
                        }
                    });
                }
            }

            return contextGenSpecList.Count == 0
                ? null
                : new SourceGenerationSpec(contextGenSpecList);
        }

        #endregion Internal Methods

        #region PropertyGenerationSpec Methods

        private static PropertyGenerationSpec GetCreationTimePropertySpec(Location location, Type? implementingType)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.CreationTimeClrName,
                IsValueType = true,
                IsNullable = false,
                Order = 991,
                Comment = AuditDefaults.CreationTimeComment,
                DeclaringTypeRef = DateTimeOffsetTypeRef,
                ImplementingType = implementingType
            };
        }

        private static PropertyGenerationSpec GetCreatorIdPropertySpec(Type propertyType, Location location, Type? implementingType)
        {
            Debug.Assert(propertyType is not null);

            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.CreatorIdClrName,
                IsValueType = propertyType!.IsValueType,
                IsNullable = true,
                IsUerKeyType = true,
                Order = 992,
                Comment = AuditDefaults.CreatorIdComment,
                DeclaringTypeRef = propertyType.GetCompilableName(),
                ImplementingType = implementingType
            };
        }

        private static PropertyGenerationSpec GetLastModificationTimePropertySpec(Location location, Type? implementingType)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.LastModificationTimeClrName,
                IsValueType = true,
                IsNullable = true,
                Order = 994,
                Comment = AuditDefaults.LastModificationTimeComment,
                DeclaringTypeRef = DateTimeOffsetTypeRef,
                ImplementingType = implementingType
            };
        }

        private static PropertyGenerationSpec GetLastModifierIdPropertySpec(Type propertyType, Location location, Type? implementingType)
        {
            Debug.Assert(propertyType is not null);

            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.LastModifierIdClrName,
                IsValueType = propertyType!.IsValueType,
                IsNullable = true,
                IsUerKeyType = true,
                Order = 995,
                Comment = AuditDefaults.LastModifierIdComment,
                DeclaringTypeRef = propertyType.GetCompilableName(),
                ImplementingType = implementingType
            };
        }

        private static PropertyGenerationSpec GetIsDeletedPropertySpec(Location location, Type? implementingType)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.IsDeletedClrName,
                IsValueType = true,
                IsNullable = false,
                Order = 997,
                Comment = AuditDefaults.IsDeletedComment,
                DeclaringTypeRef = BooleanTypeRef,
                ImplementingType = implementingType
            };
        }

        private static PropertyGenerationSpec GetDeletionTimePropertySpec(Location location, Type? implementingType)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.DeletionTimeClrName,
                IsValueType = true,
                IsNullable = true,
                Order = 998,
                Comment = AuditDefaults.DeletionTimeComment,
                DeclaringTypeRef = DateTimeOffsetTypeRef,
                ImplementingType = implementingType
            };
        }

        private static PropertyGenerationSpec GetDeleterIdPropertySpec(Type propertyType, Location location, Type? implementingType)
        {
            Debug.Assert(propertyType is not null);

            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = AuditDefaults.DeleterIdClrName,
                IsValueType = propertyType!.IsValueType,
                IsNullable = true,
                IsUerKeyType = true,
                Order = 999,
                Comment = AuditDefaults.DeleterIdComment,
                DeclaringTypeRef = propertyType.GetCompilableName(),
                ImplementingType = implementingType
            };
        }

        #endregion PropertyGenerationSpec Methods
    }
}
