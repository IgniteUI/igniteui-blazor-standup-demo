using IgniteUI.Blazor.Controls;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StandupDemo.Deck;

namespace StandupDemo.Pages;

public partial class Deck
{
    [Parameter] public string? Slug { get; set; }

    [Inject] private SlideRegistry Registry { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private IgbNavDrawer drawer = default!;
    private IgbIcon iconHost = default!;
    private IJSObjectReference? module;
    private DotNetObjectReference<Deck>? selfRef;
    private bool showNotes;
    private int direction = 1;

    private int Index { get; set; }
    private SlideInfo Current => Registry.Slides[Index];
    private double Progress => Registry.Slides.Count <= 1 ? 100 : Index * 100.0 / (Registry.Slides.Count - 1);

    protected override void OnParametersSet()
    {
        var next = Math.Max(0, Registry.IndexOf(Slug));
        direction = next >= Index ? 1 : -1;
        Index = next;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/deck.js");
        selfRef = DotNetObjectReference.Create(this);
        await module.InvokeVoidAsync("init", selfRef);

        await iconHost.EnsureReady();
        await Task.WhenAll(Icons.All.Select(icon => iconHost.RegisterIconFromTextAsync(icon.Name, icon.Svg, Icons.Collection)));
    }

    [JSInvokable]
    public Task Step(int delta) => GoTo(Index + delta);

    [JSInvokable]
    public Task GoToEnd() => GoTo(Registry.Slides.Count - 1);

    [JSInvokable]
    public Task GoTo(int index)
    {
        index = Math.Clamp(index, 0, Registry.Slides.Count - 1);
        if (index != Index)
        {
            Nav.NavigateTo(Registry.Slides[index].Slug);
        }

        return Task.CompletedTask;
    }

    private void ToggleNotes() => showNotes = !showNotes;

    private async Task GoFromMenu(int index)
    {
        await drawer.HideAsync();
        await GoTo(index);
    }

    private async Task ToggleFullscreen()
    {
        if (module is not null)
        {
            await module.InvokeVoidAsync("toggleFullscreen");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (module is not null)
        {
            try
            {
                await module.InvokeVoidAsync("dispose");
                await module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
        }

        selfRef?.Dispose();
    }
}
