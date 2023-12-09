using Ling.Reflection;
using Microsoft.CodeAnalysis;

namespace Ling.Audit.Extensions;

internal static class Diagnostics
{
    internal static Diagnostic InvalidTypeDeclareDeclaration(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA001",
                title: "Invalid type declaration",
                messageFormat: "Cannot resolve type declaration",
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true),
            location);
    }

    internal static Diagnostic ImplementationMustBeClass(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA002",
                title: "Invalid implementation",
                messageFormat: "SourceGenerator only works with classes",
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location);
    }

    internal static Diagnostic ClassMustBePartial(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA003",
                title: "Class must be partial",
                messageFormat: "SourceGenerator only works with partial classes",
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location);
    }

    internal static Diagnostic ClassCannotBeRecord(Location location)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA004",
                title: "Class cannot be record",
                messageFormat: "SourceGenerator not works with records",
                category: "Design",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location);
    }

    internal static Diagnostic UnknownUserKeyType(Location location, Type? interfaceType)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA005",
                title: "Unknown user key type",
                messageFormat: "The user key type in '{0}' is not recognized",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true),
            location,
            interfaceType?.GetFriendlyName());
    }

    internal static Diagnostic UserKeyTypeConflict(Location location, Type? interfaceType1, Type? interfaceType2)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA006",
                title: "User key type conflict",
                messageFormat: "The user key type in '{0}' conflicts with '{1}'",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true),
            location,
            interfaceType1?.GetFriendlyName(),
            interfaceType2?.GetFriendlyName());
    }

    internal static Diagnostic Debug(Location location, params string?[] args)
    {
        var msg = string.Join("\n", args);

        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA999",
                title: "Debug",
                messageFormat: "{0}",
                category: "Debug",
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true),
            location,
            msg);
    }

    internal static Diagnostic Debug(params string?[] args)
    {
        var msg = string.Join("\n", args);

        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LA999",
                title: "Debug",
                messageFormat: "{0}",
                category: "Debug",
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true),
            Location.None,
            msg);
    }
}
