using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ling.Blazor.Components;

/// <summary>
/// A component that renders different content based on the value of a parameter.
/// </summary>
/// <typeparam name="TValue">The type of the value parameter.</typeparam>
[CascadingTypeParameter(nameof(TValue))]
public sealed class Switch<TValue> : ComponentBase where TValue : notnull
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

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        IsMatched = false;
        IsDefaultRendered = false;
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var sequence = 0;

        builder.OpenComponent<CascadingValue<Switch<TValue>>>(sequence++);

        builder.AddAttribute(sequence++, "IsFixed", true);
        builder.AddAttribute(sequence++, "Value", this);
        builder.AddAttribute(sequence++, nameof(ChildContent), ChildContent);

        builder.CloseComponent();
    }
}

/// <summary>
/// A component that renders content when its value matches the value of the parent SwitchView component.
/// </summary>
/// <typeparam name="TValue">The type of the value parameter.</typeparam>
public sealed class Case<TValue> : ComponentBase where TValue : notnull
{
    [CascadingParameter] Switch<TValue> Switch { get; set; } = default!;

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
        if (Switch is null)
        {
            throw new InvalidOperationException("'Case' component must be used within a 'Switch' component.");
        }
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (EqualityComparer<TValue?>.Default.Equals(Switch.Value, When))
        {
            if (Switch.IsDefaultRendered)
            {
                throw new InvalidOperationException("'Case' component must be placed before 'Default' component.");
            }
            if (Switch.IsMatched)
            {
                throw new InvalidOperationException("Duplicate 'Case' components with the same value.");
            }
            Switch.IsMatched = true;
            builder.AddContent(0, ChildContent);
        }
    }
}

/// <summary>
/// A component that renders content when none of the child SwitchCase components match the value of the parent SwitchView component.
/// </summary>
public sealed class Default<TValue> : ComponentBase where TValue : notnull
{
    [CascadingParameter] Switch<TValue> Switch { get; set; } = default!;

    /// <summary>
    /// Gets or sets the content to be rendered when none of the values match.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if (Switch is null)
        {
            throw new InvalidOperationException("'Default' component must be used within a 'Switch' component.");
        }
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Switch.IsMatched)
        {
            return;
        }

        if (Switch.IsDefaultRendered)
        {
            throw new InvalidOperationException("'Default' component cannot use multiple times.");
        }

        Switch.IsDefaultRendered = true;
        builder.AddContent(0, ChildContent);
    }
}
