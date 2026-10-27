# Ignite UI for Blazor: open source slide deck

The slide deck for the Blazor Community Standup on Ignite UI for Blazor going open source, built as a live
Blazor WebAssembly app. Every slide is made of **IgniteUI.Blazor.Lite** and **IgniteUI.Blazor.GridLite** components
(both MIT, 1.0), with [Blazor-ApexCharts](https://github.com/apexcharts/Blazor-ApexCharts) for the charts.

## Run it

```bash
dotnet run -c Release   # http://localhost:5009
```

Release runs the WebAssembly runtime without debugging, like the published app; the Grid Lite slide shows 50,000 rows there
(10,000 in Debug builds).

## Navigating

| Key | Action |
|---|---|
| `→` `↓` / `PgDn` | Next slide (`PgUp`/`PgDn` is what most presenter clickers send) |
| `←` `↑` / `PgUp` | Previous slide |
| `Home` / `End` | First / last slide |

Arrow keys are ignored while focus is in an input or in a component that uses them (slider, tabs, tree, grid,
dialog); `PgUp`/`PgDn` still navigate there. The top bar has the slide menu, speaker notes and fullscreen.
Every slide has its own URL (`/grid-lite`, `/theming`, …), and the deck scales with the window.

## Slides

| # | Slide | Components |
|---|---|---|
| 1 | Title | Avatar, Chip, Card, QR Code, Badge |
| 2 | Open source: searchable component explorer | Input, Chip, Badge, Switch, Highlight |
| 3 | Forms: a conference badge that every control updates live | Input, Mask Input, Select, Combo (client-side template), Date Picker, Color Picker, Radio Group, Slider, Rating, Switch, Checkbox, Card, QR Code, Dialog, Snackbar |
| 4 | Getting started: linked Tree and Stepper through the setup steps | Splitter, Tree, Breadcrumbs, Stepper, Tabs, Button Group, Theme Provider |
| 5 | Grid Lite: 50,000 rows (10,000 in Debug builds), side panel and grid bound to the same filters | Grid Lite, Range Slider, Chip, Button Group, Circular Progress |
| 6 | ApexCharts with Lite controls and Grid Lite | Blazor-ApexCharts, Button Group, Chip, Slider, Switch, Grid Lite |
| 7 | Dashboard: drag, resize, save and restore the layout | Tile Manager, Calendar, List, Carousel, Grid Lite, progress indicators |
| 8 | Theming: design systems, palettes and layout tokens | Theme Provider, Color Picker, Slider, Button Group |
| 9 | AI-ready: Agent Skills (`gh skill install` or `ig ai-config`), MCP servers | Chat (preview), Tabs |
| 10 | Thank you | Card, QR Code, Avatar |

Slide order, titles and speaker notes live in `Deck/SlideRegistry.cs`; each slide is a component in `Slides/`
(the chart components are in `Charts/`).

## Deploying to GitHub Pages

`.github/workflows/deploy.yml` follows [igniteui-blazor-lite-examples](https://github.com/IgniteUI/igniteui-blazor-lite-examples):
it publishes on every push to `main`/`master`, rewrites `<base href>` to `/<repo-name>/`, and copies `index.html` to
`404.html` so deep links work. Enable it under **Settings → Pages → Source: GitHub Actions**.

## Tooling used to build this

- **Agent instructions**: `AGENTS.md`, imported by a one-line `.claude/CLAUDE.md` (`@../AGENTS.md`) for Claude Code
- **Agent Skills**: `gh skill install IgniteUI/igniteui-blazor --all --agent universal`, installed in `.agents/skills/` (read by Copilot, Codex, Cursor and other agents) and linked into `.claude/skills/` for Claude Code
- **MCP servers** (`.mcp.json` for Claude Code, `.vscode/mcp.json` for VS Code): `igniteui-theming` and `igniteui-cli`
- **Palette**: `wwwroot/css/ig-palette.css` comes from the theming MCP. `create_palette` (primary `#0099FF`, secondary
  `#F05A2B`, surface `#27265F`) generates the full set. `create_custom_palette` pins the 500 shades to the exact brand colors
  and adds a navy-tinted gray ramp, because some themes paint cards and dialogs with `--ig-gray-50`.

## Add a slide with an agent

With the skills and MCP servers above, this prompt adds a slide to the deck. It takes Claude Code under two minutes
on Sonnet:

```text
Add an "FAQ" slide after "AI-ready": a searchable FAQ for Ignite UI for Blazor Lite.
- Seed it with the questions this deck answers (MIT/commercial use, install, hosting models, theming, AI skills, upgrading to the full grid), as an accordion.
- Searching filters the questions, opens the matches and highlights them.
- Below, a way to ask a new question: it's added on top, open and marked unanswered, and can be answered right in place.
- Style the slide like .NET Conf, with its own palette: purple-to-pink gradients (#512BD4 → #D600AA).
```
