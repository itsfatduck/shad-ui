using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

// ReSharper disable once CheckNamespace
namespace ShadUI;

/// <summary>
///     Provides attached properties for setting up opacity mask bindings on Stack scroll viewers.
/// </summary>
public class ScrollOpacityMaskAssist
{
    /// <summary>
    ///     Gets or sets whether opacity mask bindings should be enabled for Stack scroll viewers.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<ScrollViewer, bool>(
            "IsEnabled",
            typeof(ScrollOpacityMaskAssist)
        );

    /// <summary>
    ///     Gets the value of <see cref="IsEnabledProperty" />
    /// </summary>
    public static bool GetIsEnabled(ScrollViewer scrollViewer) =>
        scrollViewer.GetValue(IsEnabledProperty);

    /// <summary>
    ///     Sets the value of <see cref="IsEnabledProperty" />
    /// </summary>
    public static void SetIsEnabled(ScrollViewer scrollViewer, bool value) =>
        scrollViewer.SetValue(IsEnabledProperty, value);

    static ScrollOpacityMaskAssist()
    {
        IsEnabledProperty.Changed.AddClassHandler<ScrollViewer>(OnIsEnabledChanged);
    }

    private static void OnIsEnabledChanged(
        ScrollViewer scrollViewer,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (e.NewValue is true)
        {
            // Wait for template to be applied
            scrollViewer.TemplateApplied += OnTemplateApplied;
            scrollViewer.DetachedFromVisualTree += OnDetachedFromVisualTree;

            // If template already applied, set up bindings now
            if (scrollViewer.IsInitialized)
            {
                SetupBindings(scrollViewer);
            }
        }
        else
        {
            scrollViewer.TemplateApplied -= OnTemplateApplied;
            scrollViewer.DetachedFromVisualTree -= OnDetachedFromVisualTree;
            ClearBindings(scrollViewer);
        }
    }

    private static void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            scrollViewer.TemplateApplied -= OnTemplateApplied;
            scrollViewer.DetachedFromVisualTree -= OnDetachedFromVisualTree;
            ClearBindings(scrollViewer);
        }
    }

    private static void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            // Unsubscribe after template is applied to avoid duplicate setups
            scrollViewer.TemplateApplied -= OnTemplateApplied;
            SetupBindings(scrollViewer);
        }
    }

    private static void SetupBindings(ScrollViewer scrollViewer)
    {
        // Only apply when Stack class is present
        if (!scrollViewer.Classes.Contains("Stack"))
            return;

        // Find the named elements in the template
        var innerPanel = scrollViewer.FindDescendantOfType<Panel>("InnerPanel");
        var contentPresenter = scrollViewer.FindDescendantOfType<ScrollContentPresenter>(
            "PART_ContentPresenter"
        );
        var stackScrollBar = scrollViewer.FindDescendantOfType<ScrollBar>("StackVerticalScrollBar");

        if (innerPanel == null || contentPresenter == null || stackScrollBar == null)
            return;

        // Setup Top mask binding on InnerPanel
        var topBinding1 = new Binding("Value")
        {
            Source = stackScrollBar,
            Mode = BindingMode.OneWay,
        };
        var topBinding2 = new Binding("Minimum")
        {
            Source = stackScrollBar,
            Mode = BindingMode.OneWay,
        };

        var topMultiBinding = new MultiBinding
        {
            Converter = ScrollerToOpacityMask.Top,
            Bindings = { topBinding1, topBinding2 },
        };

        innerPanel.Bind(Panel.OpacityMaskProperty, topMultiBinding);

        // Setup Bottom mask binding on ContentPresenter
        var bottomBinding1 = new Binding("Value")
        {
            Source = stackScrollBar,
            Mode = BindingMode.OneWay,
        };
        var bottomBinding2 = new Binding("Maximum")
        {
            Source = stackScrollBar,
            Mode = BindingMode.OneWay,
        };

        var bottomMultiBinding = new MultiBinding
        {
            Converter = ScrollerToOpacityMask.Bottom,
            Bindings = { bottomBinding1, bottomBinding2 },
        };

        contentPresenter.Bind(ScrollContentPresenter.OpacityMaskProperty, bottomMultiBinding);
    }

    private static void ClearBindings(ScrollViewer scrollViewer)
    {
        var innerPanel = scrollViewer.FindDescendantOfType<Panel>("InnerPanel");
        var contentPresenter = scrollViewer.FindDescendantOfType<ScrollContentPresenter>(
            "PART_ContentPresenter"
        );

        innerPanel?.ClearValue(Panel.OpacityMaskProperty);
        contentPresenter?.ClearValue(ScrollContentPresenter.OpacityMaskProperty);
    }
}

/// <summary>
///     Extension methods for finding named descendants.
/// </summary>
internal static class ScrollViewerExtensions
{
    /// <summary>
    ///     Finds a descendant of the specified type with the given name.
    /// </summary>
    public static T? FindDescendantOfType<T>(this Control control, string name)
        where T : Control
    {
        foreach (var child in control.GetVisualDescendants())
        {
            if (child is T typedChild && child.Name == name)
            {
                return typedChild;
            }

            if (child is Control childControl)
            {
                var result = childControl.FindDescendantOfType<T>(name);
                if (result != null)
                    return result;
            }
        }

        return null;
    }
}
