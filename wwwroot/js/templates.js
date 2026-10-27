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
