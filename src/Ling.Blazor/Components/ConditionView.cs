using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ling.Blazor.Components;

/// <summary>
/// A component that renders different content based on a boolean predicate.
/// </summary>
public sealed class ConditionView : ComponentBase
{
    /// <summary>
    /// Gets or sets the boolean predicate that determines which content to render.
    /// </summary>
    [Parameter] public bool Predicate { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the predicate is true.
    /// </summary>
    [Parameter] public RenderFragment? TrueContent { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the predicate is true and TrueContent is null.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the predicate is false.
    /// </summary>
    [Parameter] public RenderFragment? FalseContent { get; set; }
    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Predicate)
        {
            if (TrueContent is not null)
            {
                builder.AddContent(0, TrueContent);
            }
            else if (ChildContent is not null)
            {
                builder.AddContent(0, ChildContent);
            }
        }
        else if (FalseContent is not null)
        {
            builder.AddContent(1, FalseContent);
        }
    }
}
