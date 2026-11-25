using IgniteUI.Blazor.Controls;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StandupDemo;
using StandupDemo.Deck;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// IgniteUI.Blazor.Lite: one call makes every component available; each loads its module on first render
// and the JS bundle loads itself through a JS initializer. The tree, range slider, theme provider and
// tile manager modules are preloaded, so they are ready before their slides render.
// IgniteUI.Blazor.Trial (the commercial product) builds on Lite and registers through the same call: its grids,
// Dock Manager and charts are preloaded too, plus the chart interactivity feature module (zoom and pan), which
// nothing requests on render.
builder.Services.AddIgniteUIBlazor(
    typeof(IgbTreeModule), typeof(IgbRangeSliderModule), typeof(IgbThemeProviderModule), typeof(IgbTileManagerModule),
    typeof(IgbGridModule), typeof(IgbTreeGridModule), typeof(IgbHierarchicalGridModule),
    typeof(IgbPivotGridModule), typeof(IgbPivotDataSelectorModule), typeof(IgbDockManagerModule),
    typeof(IgbCategoryChartModule), typeof(IgbFinancialChartModule), typeof(IgbDoughnutChartModule),
    typeof(IgbDataChartInteractivityModule));
builder.Services.AddSingleton<SlideRegistry>();

await builder.Build().RunAsync();
