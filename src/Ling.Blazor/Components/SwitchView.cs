using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ling.Blazor.Components;

/// <summary>
/// A component that renders different content based on the value of a parameter.
/// </summary>
/// <typeparam name="TValue">The type of the value parameter.</typeparam>
[RestrictChildren(nameof(SwitchCase<TValue>), nameof(SwitchDefault<TValue>))]
[CascadingTypeParameter(nameof(TValue))]
public sealed class SwitchView<TValue> : ComponentBase where TValue : notnull
{
    /// <summary>
    /// Gets a value indicating whether a child SwitchCase component has been matched.
    /// </summary>
    internal bool IsMatched { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the SwitchDefault component has been rendered.
    /// </summary>
    internal bool IsDefaultRendered { get; set; }

    /// <summary>
    /// Gets or sets the value to be matched by the child SwitchCase components.
    /// </summary>
    [Parameter] public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the child content of the component.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content to be rendered when none of the child SwitchCase components match the value.
    /// </summary>
    [Parameter] public RenderFragment FallbackContent { get; set; } = builder => builder.AddContent(0, "No SwitchCase matches the value, please set a default value using SwitchDefault");

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Provide the value and the component instance to child components
        builder.OpenComponent<CascadingValue<SwitchView<TValue>>>(0);
        builder.AddAttribute(1, "Value", this);

        // Render the child content
        builder.AddAttribute(2, "ChildContent", (RenderFragment)((rtb) =>
        {
            rtb.AddContent(3, ChildContent);

            // Render SwitchUnmatched if no SwitchCase has matched
            rtb.OpenComponent<SwitchUnmatched<TValue>>(4);
            rtb.CloseComponent();
        }));

        builder.CloseComponent();
    }

    private class SwitchUnmatched<T> : ComponentBase where T : notnull
    {
        [CascadingParameter] SwitchView<T> Parent { get; set; } = default!;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // Render the fallback content if no SwitchCase has matched
            if (!Parent.IsMatched && !Parent.IsDefaultRendered)
            {
                builder.AddContent(0, Parent.FallbackContent);
            }
        }
    }
}

/// <summary>
/// A component that renders content when its value matches the value of the parent SwitchView component.
/// </summary>
/// <typeparam name="TValue">The type of the value parameter.</typeparam>
public sealed class SwitchCase<TValue> : ComponentBase where TValue : notnull
{
    [CascadingParameter] SwitchView<TValue> Parent { get; set; } = default!;

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
        // Ensure the parent is a SwitchView
        if (Parent is null)
        {
            throw new InvalidOperationException("'SwitchCase' must be used within a 'SwitchView'");
        }
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Render the child content if the value matches
        if (EqualityComparer<TValue?>.Default.Equals(Parent.Value, When))
        {
            // Check for invalid use of SwitchDefault or duplicated
            if (Parent.IsDefaultRendered)
            {
                // Throw an exception if SwitchDefault is used improperly.
                throw new InvalidOperationException("'SwitchDefault' must be placed below all 'SwitchCase' components.");
            }
            if (Parent.IsMatched)
            {
                // Throw an exception if there is a duplicated 'When' condition.
                throw new InvalidOperationException("A 'SwitchCase' component with the same 'When' condition already exists.");
            }
            Parent.IsMatched = true;
            // Add the child content to the render tree.
            builder.AddContent(1, ChildContent);
        }
    }
}

/// <summary>
/// A component that renders content when none of the child SwitchCase components match the value of the parent SwitchView component.
/// </summary>
public sealed class SwitchDefault<TValue> : ComponentBase where TValue : notnull
{
    [CascadingParameter] SwitchView<TValue> Parent { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content to be rendered when none of the values match.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Parent is null)
        {
            throw new InvalidOperationException("'SwitchDefault' must be used within a 'SwitchView'");
        }
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Parent.IsDefaultRendered)
        {
            throw new InvalidOperationException("'SwitchDefault' can only be set once");
        }
        else if (!Parent.IsMatched)
        {
            Parent.IsDefaultRendered = true;
            builder.AddContent(0, ChildContent);
        }
    }
}
