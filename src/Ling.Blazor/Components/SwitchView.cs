using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ling.Blazor.Components;

/// <summary>
/// A component that renders different content based on the value of a parameter.
/// </summary>
/// <typeparam name="TValue">The type of the value parameter.</typeparam>
[RestrictChildren(nameof(SwitchCase<TValue>), nameof(SwitchDefault))]
public sealed class SwitchView<TValue> : ComponentBase
{
    private static readonly RenderFragment _defaultUnmatchedContent
        = builder => builder.AddContent(0, "not matched");

    /// <summary>
    /// Gets or sets the value to be matched by the child SwitchCase components.
    /// </summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the child content of the component.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;

    internal bool IsMatched { get; set; }

    internal RenderFragment? DefaultContent { get; set; }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<SwitchView<TValue>>>(0);
        builder.AddAttribute(1, "Value", this);
        builder.AddAttribute(2, "ChildContent", CreateChildContent());
        builder.CloseElement();
    }

    private RenderFragment CreateChildContent()
    {
        return builder =>
        {
            builder.AddContent(0, ChildContent);
            if (!IsMatched)
            {
                builder.AddContent(1, DefaultContent ?? _defaultUnmatchedContent);
            }
        };
    }
}

/// <summary>
/// A component that renders content when its value matches the value of the parent SwitchView component.
/// </summary>
/// <typeparam name="TValue">The type of the value parameter.</typeparam>
public sealed class SwitchCase<TValue> : ComponentBase
{
    /// <summary>
    /// Gets or sets the parent SwitchView component that provides the value to be matched.
    /// </summary>
    [CascadingParameter]
    public SwitchView<TValue> Parent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value to be matched by this component.
    /// </summary>
    [Parameter, EditorRequired] public TValue? When { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content to be rendered when the value matches.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("'SwitchCase' component must be used inside a 'SwitchView' component.");
        }
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Parent.IsMatched && EqualityComparer<TValue>.Default.Equals(Parent.Value, When))
        {
            Parent.IsMatched = true;
            builder.AddContent(0, ChildContent);
        }
    }
}

/// <summary>
/// A component that renders content when none of the child SwitchCase components match the value of the parent SwitchView component.
/// </summary>
public sealed class SwitchDefault : ComponentBase
{
    /// <summary>
    /// Gets or sets the parent SwitchView component that provides the value to be matched.
    /// </summary>
    [CascadingParameter]
    public SwitchView<object?> Parent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content to be rendered when none of the values match.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("'SwitchDefault' component must be used inside a 'SwitchView' component.");
        }
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Parent.DefaultContent = ChildContent;
    }
}
