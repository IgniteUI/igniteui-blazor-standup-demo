// Keyboard driver for the deck.
// Arrow keys navigate unless focus is inside something that uses them (inputs, sliders, tabs, tree, grid, dialogs...).
// PageUp / PageDown (what presenter clickers send) always navigate, except while typing.

const TEXT_TAGS = new Set(['INPUT', 'TEXTAREA', 'SELECT']);
const ARROW_OWNERS = new Set([
    'IGC-SLIDER', 'IGC-RANGE-SLIDER', 'IGC-TABS', 'IGC-TREE', 'IGC-RATING', 'IGC-BUTTON-GROUP',
    'IGC-RADIO-GROUP', 'IGC-RADIO', 'IGC-SELECT', 'IGC-COMBO', 'IGC-CALENDAR', 'IGC-DATE-PICKER',
    'IGC-DATE-RANGE-PICKER', 'IGC-CAROUSEL', 'IGC-TILE-MANAGER', 'IGC-SPLITTER', 'IGC-GRID-LITE',
    'IGC-CHAT', 'IGC-STEPPER', 'IGC-DROPDOWN', 'IGC-COLOR-PICKER', 'IGC-LIST', 'IGC-NAV-DRAWER', 'IGC-DIALOG'
]);

let deck;
let lastNavigation = 0;

// One slide change at a time: a held key or a double-firing clicker must not queue a burst of slide mounts.
function navigate(method, ...args) {
    const now = performance.now();
    if (now - lastNavigation < 350) return;
    lastNavigation = now;
    deck.invokeMethodAsync(method, ...args);
}

function focusContext(e) {
    for (const el of e.composedPath()) {
        if (!(el instanceof Element)) continue;
        if (TEXT_TAGS.has(el.tagName) || el.isContentEditable) return 'text';
        if (ARROW_OWNERS.has(el.tagName)) return 'arrows';
    }
    return null;
}

function onKeyDown(e) {
    if (!deck || e.repeat || e.defaultPrevented || e.altKey || e.ctrlKey || e.metaKey) return;

    const ctx = focusContext(e);
    if (ctx === 'text' || (ctx && e.key !== 'PageDown' && e.key !== 'PageUp')) return;

    switch (e.key) {
        case 'ArrowRight':
        case 'ArrowDown':
        case 'PageDown':
            e.preventDefault();
            navigate('Step', 1);
            break;
        case 'ArrowLeft':
        case 'ArrowUp':
        case 'PageUp':
            e.preventDefault();
            navigate('Step', -1);
            break;
        case 'Home':
            navigate('GoTo', 0);
            break;
        case 'End':
            navigate('GoToEnd');
            break;
    }
}

export function init(dotNetRef) {
    deck = dotNetRef;
    document.addEventListener('keydown', onKeyDown);
}

export function dispose() {
    document.removeEventListener('keydown', onKeyDown);
    deck = undefined;
}

export function toggleFullscreen() {
    if (document.fullscreenElement) {
        document.exitFullscreen();
    } else {
        document.documentElement.requestFullscreen?.();
    }
}
