// Client-side templates for *Script parameters (IgniteUI.Blazor.Lite api.js).
// They render in the browser per item, without a round trip to .NET.
import { registerScript, html } from '../_content/IgniteUI.Blazor/api.js';

// Combo item: emoji + name + area (Forms slide)
registerScript('TopicItem', ({ item }) => html`
    <span style="display:flex;align-items:center;gap:8px;width:100%">
        <span style="font-size:1.1em">${item.Emoji}</span>
        <span>${item.Name}</span>
        <small style="margin-left:auto;opacity:.6">${item.Area}</small>
    </span>
`);

// Combo group header (Forms slide)
registerScript('TopicGroup', ({ item }) => html`<b style="letter-spacing:.08em;text-transform:uppercase">${item.Area}</b>`);

// Pivot Grid value formatter: whole dollars for any aggregate (Tree, hierarchical, pivot slide).
// The commercial IgniteUI.Blazor resolves *Script parameters through this same api.js registry.
const usd = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
registerScript('PivotCurrency', value => (typeof value === 'number' ? usd.format(value) : value ?? ''));

// Watchlist day change: a client-side cell template for the commercial grid (Dock Manager slide). It renders in the
// browser per visible cell, with no round trip to .NET, which suits a column that is pure formatting.
registerScript('WatchlistChange', ({ cell }) => {
    const change = Number(cell?.value ?? 0);
    const color = change >= 0 ? '#7EE2B8' : '#FF8A65';
    const text = `${change >= 0 ? '+' : ''}${(change * 100).toFixed(2)}%`;
    return html`<span style="color:${color};font-weight:600;font-variant-numeric:tabular-nums">${text}</span>`;
});

// Employee rating: the Lite igc-rating element in a commercial grid cell (Grid slide). A Razor BodyTemplate here
// called into .NET for every cell the grid rendered, which made expanding groups and scrolling slow; this renders
// in the browser. The grid-slide attribute is the slide's CSS scope, so GridSlide.razor.css still styles it.
registerScript('EmployeeRating', ({ cell }) => html`
    <igc-rating grid-slide class="cell-rating" .value=${Number(cell?.value ?? 0)} .max=${5} .readOnly=${true}></igc-rating>
`);
