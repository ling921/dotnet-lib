using Ling.EntityFrameworkCore.Audit.Generator;
using Ling.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

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

        private static readonly DiagnosticDescriptor UnknownUserKeyType = new(
            id: "LDEFCG001",
            title: GeneratorConstants.UnknownUserKeyTypeTitle,
            messageFormat: GeneratorConstants.UnknownUserKeyTypeMessageFormat,
            category: GeneratorConstants.AuditSourceGenerationName,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

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

        private static DiagnosticDescriptor ContextClassesMustBePartial { get; } = new DiagnosticDescriptor(
            id: "LDEFCG002",
            title: GeneratorConstants.ContextClassesMustBePartialTitle,
            messageFormat: GeneratorConstants.ContextClassesMustBePartialMessageFormat,
            category: GeneratorConstants.AuditSourceGenerationName,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

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
                !cds.Modifiers.Any(st => st.IsKind(SyntaxKind.StaticKeyword)) &&
                cds.Modifiers.Any(st => st.IsKind(SyntaxKind.PartialKeyword));
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

                    var unimplementedInterfaceTypeSymbols = GetUnimplementedInterfaceTypeSymbols(contextTypeSymbol);
                    if (!unimplementedInterfaceTypeSymbols.Any())
                    {
                        continue;
                    }

                    var baseTypeLocations = classDeclarationSyntax.BaseList!.Types
                        .Select(t => new { DeclarationName = t.ToFullString().Trim(), Location = t.GetLocation() });

                    var propertyGenSpecList = new List<PropertyGenerationSpec>();

                    foreach (var (interfaceTypeSymbol, declarationName) in unimplementedInterfaceTypeSymbols)
                    {
                        var interfaceType = interfaceTypeSymbol.AsType(_metadataLoadContext)!;
                        var location = baseTypeLocations.FirstOrDefault(i => i.DeclarationName == declarationName)?.Location ?? Location.None;
                        if (_hasCreationTimeInterfaceType!.Equals(interfaceType))
                        {
                            propertyGenSpecList.Add(GetCreationTimePropertySpec(location));
                        }
                        else if (interfaceType.GetCompatibleGenericBaseClass(_hasCreatorInterfaceType!) != null)
                        {
                            var userKeyType = interfaceType.GetGenericArguments().FirstOrDefault();
                            if (userKeyType is null)
                            {
                                _sourceGenerationContext.ReportDiagnostic(Diagnostic.Create(UnknownUserKeyType, location, GetClassDeclarationName(interfaceTypeSymbol)));
                                break;
                            }
                            propertyGenSpecList.Add(GetCreatorIdPropertySpec(userKeyType, location));
                        }
                        else if (_hasModificationTimeInterfaceType!.Equals(interfaceType))
                        {
                            propertyGenSpecList.Add(GetLastModificationTimePropertySpec(location));
                        }
                        else if (interfaceType.GetCompatibleGenericBaseClass(_hasModifierInterfaceType!) != null)
                        {
                            var userKeyType = interfaceType.GetGenericArguments().FirstOrDefault();
                            if (userKeyType is null)
                            {
                                _sourceGenerationContext.ReportDiagnostic(Diagnostic.Create(UnknownUserKeyType, location, GetClassDeclarationName(interfaceTypeSymbol)));
                                break;
                            }
                            propertyGenSpecList.Add(GetLastModifierIdPropertySpec(userKeyType, location));
                        }
                        else if (_hasSoftDeleteInterfaceType!.Equals(interfaceType))
                        {
                            propertyGenSpecList.Add(GetIsDeletedPropertySpec(location));
                        }
                        else if (_hasDeletionTimeInterfaceType!.Equals(interfaceType))
                        {
                            propertyGenSpecList.Add(GetDeletionTimePropertySpec(location));
                        }
                        if (interfaceType.GetCompatibleGenericBaseClass(_hasDeleterInterfaceType!) != null)
                        {
                            var userKeyType = interfaceType.GetGenericArguments().FirstOrDefault();
                            if (userKeyType is null)
                            {
                                _sourceGenerationContext.ReportDiagnostic(Diagnostic.Create(UnknownUserKeyType, location, GetClassDeclarationName(interfaceTypeSymbol)));
                                break;
                            }
                            propertyGenSpecList.Add(GetDeleterIdPropertySpec(userKeyType, location));
                        }
                    }

                    if (propertyGenSpecList.Count == 0)
                    {
                        continue;
                    }

                    if (!TryGetClassDeclarationList(contextTypeSymbol, out List<string>? classDeclarationList))
                    {
                        // Class or one of its containing types is not partial so we can't add to it.
                        _sourceGenerationContext.ReportDiagnostic(Diagnostic.Create(ContextClassesMustBePartial, contextLocation, new string[] { contextTypeSymbol.Name }));
                        continue;
                    }

                    var contextType = contextTypeSymbol.AsType(_metadataLoadContext)!;
                    contextGenSpecList.Add(new ContextGenerationSpec()
                    {
                        Location = contextLocation,
                        ContextType = contextType,
                        ContextClassDeclarationList = classDeclarationList!,
                        GenerationType = new TypeGenerationSpec
                        {
                            Location = contextLocation,
                            ClassDeclaration = classDeclarationList![0],
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

        #region Private Static Methods

        private static bool TryGetClassDeclarationList(INamedTypeSymbol typeSymbol, out List<string>? classDeclarationList)
        {
            INamedTypeSymbol currentSymbol = typeSymbol;
            classDeclarationList = null;

            while (currentSymbol != null)
            {
                if (currentSymbol.DeclaringSyntaxReferences[0].GetSyntax() is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    SyntaxTokenList tokenList = classDeclarationSyntax.Modifiers;
                    int tokenCount = tokenList.Count;

                    var isStatic = false;
                    var isInterface = false;
                    var isPartial = false;

                    var declarationElements = new string[tokenCount + 2];

                    for (int i = 0; i < tokenCount; i++)
                    {
                        var token = tokenList[i];
                        declarationElements[i] = token.Text;

                        if (token.IsKind(SyntaxKind.StaticKeyword))
                        {
                            isStatic = true;
                        }
                        if (token.IsKind(SyntaxKind.InterfaceDeclaration))
                        {
                            isInterface = true;
                        }
                        if (token.IsKind(SyntaxKind.PartialKeyword))
                        {
                            isPartial = true;
                        }
                    }

                    if (isStatic || isInterface || !isPartial)
                    {
                        classDeclarationList = null;
                        return false;
                    }

                    declarationElements[tokenCount] = "class";
                    declarationElements[tokenCount + 1] = GetClassDeclarationName(currentSymbol);

                    (classDeclarationList ??= new List<string>()).Add(string.Join(" ", declarationElements));
                }

                currentSymbol = currentSymbol.ContainingType;
            }

            Debug.Assert(classDeclarationList!.Count > 0);
            return true;
        }

        private static string GetClassDeclarationName(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeArguments.Length == 0)
            {
                return typeSymbol.Name;
            }

            var sb = new StringBuilder();

            sb.Append(typeSymbol.Name);
            sb.Append('<');

            bool first = true;
            foreach (ITypeSymbol typeArg in typeSymbol.TypeArguments)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                else
                {
                    first = false;
                }

                sb.Append(typeArg.Name);
            }

            sb.Append('>');

            return sb.ToString();
        }

        private static IEnumerable<(INamedTypeSymbol InterfaceTypeSymbol, string DeclarationName)> GetUnimplementedInterfaceTypeSymbols(INamedTypeSymbol? classTypeSymbol)
        {
            if (classTypeSymbol is not null)
            {
                var excludes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
                if (classTypeSymbol.BaseType is not null)
                {
                    foreach (var interfaceTypeSymbol in classTypeSymbol.BaseType.AllInterfaces)
                    {
                        excludes.Add(interfaceTypeSymbol);
                    }
                }
                foreach (var interfaceTypeSymbol in classTypeSymbol.Interfaces)
                {
                    var declarationName = GetClassDeclarationName(interfaceTypeSymbol);
                    if (!excludes.Contains(interfaceTypeSymbol))
                    {
                        yield return (interfaceTypeSymbol, declarationName);
                        excludes.Add(interfaceTypeSymbol);
                    }
                    foreach (var innerInterfaceTypeSymbol in interfaceTypeSymbol.AllInterfaces)
                    {
                        if (!excludes.Contains(innerInterfaceTypeSymbol))
                        {
                            yield return (innerInterfaceTypeSymbol, declarationName);
                            excludes.Add(innerInterfaceTypeSymbol);
                        }
                    }
                }
            }
        }

        #endregion Private Static Methods

        #region PropertyGenerationSpec Methods

        private static PropertyGenerationSpec GetCreationTimePropertySpec(Location location)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "CreationTime",
                IsValueType = true,
                IsNullable = false,
                Order = 991,
                Comment = GeneratorConstants.CreationTimeComment,
                DeclaringTypeRef = DateTimeOffsetTypeRef
            };
        }

        private static PropertyGenerationSpec GetCreatorIdPropertySpec(Type propertyType, Location location)
        {
            Debug.Assert(propertyType is not null);

            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "CreatorId",
                IsValueType = propertyType!.IsValueType,
                IsNullable = true,
                IsUerKeyType = true,
                Order = 992,
                Comment = GeneratorConstants.CreatorIdComment,
                DeclaringTypeRef = propertyType.GetCompilableName()
            };
        }

        private static PropertyGenerationSpec GetLastModificationTimePropertySpec(Location location)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "LastModificationTime",
                IsValueType = true,
                IsNullable = true,
                Order = 994,
                Comment = GeneratorConstants.LastModificationTimeComment,
                DeclaringTypeRef = DateTimeOffsetTypeRef
            };
        }

        private static PropertyGenerationSpec GetLastModifierIdPropertySpec(Type propertyType, Location location)
        {
            Debug.Assert(propertyType is not null);

            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "LastModifierId",
                IsValueType = propertyType!.IsValueType,
                IsNullable = true,
                IsUerKeyType = true,
                Order = 995,
                Comment = GeneratorConstants.LastModifierIdComment,
                DeclaringTypeRef = propertyType.GetCompilableName()
            };
        }

        private static PropertyGenerationSpec GetIsDeletedPropertySpec(Location location)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "IsDeleted",
                IsValueType = true,
                IsNullable = false,
                Order = 997,
                Comment = GeneratorConstants.IsDeletedComment,
                DeclaringTypeRef = BooleanTypeRef
            };
        }

        private static PropertyGenerationSpec GetDeletionTimePropertySpec(Location location)
        {
            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "DeletionTime",
                IsValueType = true,
                IsNullable = true,
                Order = 998,
                Comment = GeneratorConstants.DeletionTimeComment,
                DeclaringTypeRef = DateTimeOffsetTypeRef
            };
        }

        private static PropertyGenerationSpec GetDeleterIdPropertySpec(Type propertyType, Location location)
        {
            Debug.Assert(propertyType is not null);

            return new PropertyGenerationSpec
            {
                Location = location,
                ClrName = "DeleterId",
                IsValueType = propertyType!.IsValueType,
                IsNullable = true,
                IsUerKeyType = true,
                Order = 999,
                Comment = GeneratorConstants.DeleterIdComment,
                DeclaringTypeRef = propertyType.GetCompilableName()
            };
        }

        #endregion PropertyGenerationSpec Methods
    }
}
