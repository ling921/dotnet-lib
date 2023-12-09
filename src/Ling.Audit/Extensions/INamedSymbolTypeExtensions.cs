using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Text;

namespace Ling.Audit.Extensions;

internal static class INamedSymbolTypeExtensions
{
    internal static List<(INamedTypeSymbol InterfaceTypeSymbol, List<INamedTypeSymbol> UnimplementedInterfaceTypeSymbols)> GetUnimplementedInterfaceTypeSymbols(this INamedTypeSymbol? classTypeSymbol)
    {
        var symbolsList = new List<(INamedTypeSymbol, List<INamedTypeSymbol>)>();

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
                var unimplementedInterfaceTypeSymbols = new List<INamedTypeSymbol>();

                if (!excludes.Contains(interfaceTypeSymbol))
                {
                    unimplementedInterfaceTypeSymbols.Add(interfaceTypeSymbol);
                    foreach (var innerInterfaceTypeSymbol in interfaceTypeSymbol.AllInterfaces)
                    {
                        if (!excludes.Contains(innerInterfaceTypeSymbol))
                        {
                            unimplementedInterfaceTypeSymbols.Add(innerInterfaceTypeSymbol);
                            excludes.Add(innerInterfaceTypeSymbol);
                        }
                    }
                }

                symbolsList.Add((interfaceTypeSymbol, unimplementedInterfaceTypeSymbols));
            }
        }

        return symbolsList;
    }

    internal static string GetTypeDeclarationName(this INamedTypeSymbol typeSymbol)
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

    internal static List<string[]> GetTypeDeclarationList(this INamedTypeSymbol typeSymbol)
    {
        INamedTypeSymbol currentSymbol = typeSymbol;
        var typeDeclarationList = new List<string[]>();

        while (currentSymbol != null)
        {
            if (currentSymbol.DeclaringSyntaxReferences[0].GetSyntax() is TypeDeclarationSyntax typeDeclarationSyntax)
            {
                SyntaxTokenList tokenList = typeDeclarationSyntax.Modifiers;
                int tokenCount = tokenList.Count;

                var declarationElements = new string[tokenCount + 2];

                for (int i = 0; i < tokenCount; i++)
                {
                    var token = tokenList[i];
                    declarationElements[i] = token.Text;
                }

                declarationElements[tokenCount] = typeDeclarationSyntax.Keyword.Text;
                declarationElements[tokenCount + 1] = currentSymbol.GetTypeDeclarationName();

                typeDeclarationList.Add(declarationElements);
            }

            currentSymbol = currentSymbol.ContainingType;
        }

        Debug.Assert(typeDeclarationList.Count > 0);
        return typeDeclarationList;
    }

    internal static bool IsEqualsTo(this INamedTypeSymbol? typeSymbol, INamedTypeSymbol? otherTypeSymbol)
    {
        if (SymbolEqualityComparer.Default.Equals(typeSymbol, otherTypeSymbol))
        {
            return true;
        }

        var typeSymbolName = typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var otherTypeSymbolName = otherTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return typeSymbolName == otherTypeSymbolName;
    }
}
