using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ling.Blazor.Components;

/// <summary>
/// A component that renders content based on a boolean predicate.
/// </summary>
public sealed class Condition : ComponentBase
{
    /// <summary>
    /// Gets or sets the boolean predicate that determines which content to render.
    /// </summary>
    [Parameter] public bool Predicate { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the predicate is true.
    /// </summary>
    [Parameter] public RenderFragment? True { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the predicate is true and TrueContent is null.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the predicate is false.
    /// </summary>
    [Parameter] public RenderFragment? False { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, Predicate ? (True ?? ChildContent) : False);
    }
}
