namespace StandupDemo.Data;

public enum ComponentGroup
{
    Inputs,
    Layout,
    Display,
    Feedback,
    Grid
}

public sealed record CatalogItem(string Name, ComponentGroup Group, string Summary, bool IsNew = false);

/// <summary>What ships in IgniteUI.Blazor.Lite 1.0 + IgniteUI.Blazor.GridLite 1.0 (per the repo README).</summary>
public static class ComponentCatalog
{
    public static readonly IReadOnlyList<CatalogItem> Items =
    [
        new("Input", ComponentGroup.Inputs, "Text, email, number, password with prefix/suffix slots"),
        new("Textarea", ComponentGroup.Inputs, "Multi-line text with resize and auto-grow"),
        new("Mask Input", ComponentGroup.Inputs, "Phone numbers, codes, any pattern"),
        new("Date Time Input", ComponentGroup.Inputs, "Masked date and time entry with spin keys"),
        new("Date Picker", ComponentGroup.Inputs, "Input plus dropdown or dialog calendar"),
        new("Date Range Picker", ComponentGroup.Inputs, "Start/end range with predefined ranges"),
        new("Calendar", ComponentGroup.Inputs, "Single, multiple and range selection, multi-month"),
        new("Combo", ComponentGroup.Inputs, "Searchable multi-select with grouping and templates"),
        new("Select", ComponentGroup.Inputs, "Single selection with groups and headers"),
        new("Checkbox", ComponentGroup.Inputs, "With indeterminate state"),
        new("Switch", ComponentGroup.Inputs, "On/off toggle"),
        new("Radio & Radio Group", ComponentGroup.Inputs, "Binds enums directly"),
        new("Slider", ComponentGroup.Inputs, "Ticks, labels, generic numeric TValue"),
        new("Range Slider", ComponentGroup.Inputs, "Lower and upper thumbs"),
        new("Rating", ComponentGroup.Inputs, "Stars or any symbol, half steps"),
        new("Color Picker", ComponentGroup.Inputs, "HSV canvas, alpha, swatches, EyeDropper", IsNew: true),
        new("Button", ComponentGroup.Inputs, "Contained, outlined, flat, FAB; link mode"),
        new("Icon Button", ComponentGroup.Inputs, "Icon-only actions"),
        new("Button Group", ComponentGroup.Inputs, "Single or multiple toggle selection"),
        new("Toggle Button", ComponentGroup.Inputs, "A selectable button for button groups"),

        new("Tabs", ComponentGroup.Layout, "Content tabs with label slots"),
        new("Stepper", ComponentGroup.Layout, "Linear or free, horizontal or vertical"),
        new("Accordion", ComponentGroup.Layout, "Expansion panels with single-expand"),
        new("Expansion Panel", ComponentGroup.Layout, "Collapsible section"),
        new("Navigation Drawer", ComponentGroup.Layout, "Overlay, pinned or mini rail"),
        new("Navbar", ComponentGroup.Layout, "App bar with start/end slots"),
        new("Breadcrumbs", ComponentGroup.Layout, "Accessible navigation trail", IsNew: true),
        new("Tree", ComponentGroup.Layout, "Hierarchies with cascade selection"),
        new("Splitter", ComponentGroup.Layout, "Resizable, collapsible panes"),
        new("Divider", ComponentGroup.Layout, "Solid or dashed separator"),
        new("Tile Manager", ComponentGroup.Layout, "Draggable, resizable dashboard tiles"),
        new("Theme Provider", ComponentGroup.Layout, "Scope a theme to part of the page"),

        new("List", ComponentGroup.Display, "Items with start/title/subtitle/end slots"),
        new("Card", ComponentGroup.Display, "Media, header, content, actions"),
        new("Carousel", ComponentGroup.Display, "Slides with autoplay and indicators"),
        new("Avatar", ComponentGroup.Display, "Image, initials or icon"),
        new("Badge", ComponentGroup.Display, "Counters and status dots"),
        new("Chip", ComponentGroup.Display, "Selectable, removable tags"),
        new("Icon", ComponentGroup.Display, "SVG icon registry with collections"),
        new("Linear Progress", ComponentGroup.Display, "Determinate, indeterminate, striped"),
        new("Circular Progress", ComponentGroup.Display, "Ring progress with label"),
        new("Dropdown", ComponentGroup.Display, "Menus anchored to any target"),
        new("Tooltip", ComponentGroup.Display, "Rich or plain, sticky, arrow"),
        new("QR Code", ComponentGroup.Display, "Styled QR codes with logo and export", IsNew: true),
        new("Highlight", ComponentGroup.Display, "Find-in-page text highlighting"),
        new("Chat (preview)", ComponentGroup.Display, "Conversational UI with templates and suggestions", IsNew: true),
        new("Ripple", ComponentGroup.Display, "Material click feedback"),

        new("Dialog", ComponentGroup.Feedback, "Modal with footer actions"),
        new("Snackbar", ComponentGroup.Feedback, "Message with a single action"),
        new("Toast", ComponentGroup.Feedback, "Fire-and-forget notification"),
        new("Banner", ComponentGroup.Feedback, "Inline, persistent notice"),

        new("Grid Lite", ComponentGroup.Grid, "Virtualized grid: sort, filter, resize (GridLite package)", IsNew: true),
    ];
}
