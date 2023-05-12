using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ling.Blazor.Components;

/// <summary>
/// A component that renders an enumeration of items with optional separator and empty content.
/// </summary>
/// <typeparam name="TItem">The type of the items in the enumeration.</typeparam>
public sealed class EnumerationView<TItem> : ComponentBase
{
    private static readonly RenderFragment _defaultEmptyContent
        = builder => builder.AddContent(0, "empty");

    /// <summary>
    /// Gets or sets the source enumeration of items to render.
    /// </summary>
    [Parameter] public IEnumerable<TItem>? Source { get; set; }

    /// <summary>
    /// Gets or sets the template that will be used to render each item in the enumeration.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment<TItem> EachTemplate { get; set; } = null!;

    /// <summary>
    /// Gets or sets the content that will be rendered between each item in the enumeration.
    /// </summary>
    [Parameter] public RenderFragment? SeparatorContent { get; set; }

    /// <summary>
    /// Gets or sets the content that will be rendered if the source enumeration is null or empty.
    /// </summary>
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Source?.Any() == true)
        {
            var i = 0;
            foreach (var item in Source)
            {
                if (i > 0 && SeparatorContent is not null)
                {
                    builder.AddContent(i++, SeparatorContent);
                }
                builder.AddContent(i++, EachTemplate(item));
            }
        }
        else
        {
            builder.AddContent(0, EmptyContent ?? _defaultEmptyContent);
        }
    }
}
