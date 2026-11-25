// Large chart data sets for DataSourceScript (High-performance charts slide). They are generated here, in the
// browser, so the points never cross the .NET/JS boundary: the chart asks for the data by script name.
// The commercial IgniteUI.Blazor resolves *Script parameters through the same Lite api.js registry.
import { registerScript } from '../_content/IgniteUI.Blazor/api.js';

// Small seeded PRNG (mulberry32), so every run draws the same series.
function random(seed) {
    return () => {
        seed |= 0;
        seed = (seed + 0x6d2b79f5) | 0;
        let t = Math.imul(seed ^ (seed >>> 15), 1 | seed);
        t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
        return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
    };
}

// A noisy signal on a slow random walk: lots of detail at every zoom level.
function signal(count) {
    const next = random(26_2);
    const data = new Array(count);
    let level = 50;
    for (let i = 0; i < count; i++) {
        level += (next() - 0.5) * 0.8;
        level += (50 - level) * 0.0005;
        const wave = Math.sin(i / 900) * 12 + Math.sin(i / 37) * 3;
        data[i] = { Label: String(i), Value: level + wave + (next() - 0.5) * 6 };
    }
    return data;
}

// shouldCall = true: each function is called when the chart resolves the parameter, and its result is the data.
for (const [name, count] of [['Points10K', 10_000], ['Points100K', 100_000], ['Points1M', 1_000_000]]) {
    registerScript(name, () => signal(count), true);
}
