// Console scroll/viewport helpers for the game terminal.
// State lives here; Blazor is notified only when the pinned state flips.
window.gmConsole = (function () {
    let output = null;
    let dotNetRef = null;
    let pinned = true;           // user is at (or near) the bottom
    const PIN_THRESHOLD = 40;    // px from bottom still counting as "at bottom"

    function distanceFromBottom() {
        return output.scrollHeight - output.scrollTop - output.clientHeight;
    }

    function setPinned(value) {
        if (pinned === value) return;
        pinned = value;
        document.querySelector('.jump-pill')?.classList.toggle('visible', !pinned);
        dotNetRef?.invokeMethodAsync('OnScrollPinnedChanged', pinned);
    }

    function lineHeightPx() {
        // Lines render as child divs with line-height 1.0; measure a real one when possible.
        const child = output.querySelector('div');
        if (child) {
            const lh = parseFloat(getComputedStyle(child).lineHeight);
            if (!isNaN(lh) && lh > 0) return lh;
        }
        const fs = parseFloat(getComputedStyle(output).fontSize);
        return isNaN(fs) ? 16 : fs;
    }

    return {
        init(outputElementId, ref) {
            output = document.getElementById(outputElementId);
            dotNetRef = ref;
            if (!output) return;

            output.addEventListener('scroll', () => {
                setPinned(distanceFromBottom() <= PIN_THRESHOLD);
            });

            new ResizeObserver(() => {
                dotNetRef?.invokeMethodAsync('OnViewportResized', this.measureViewportLines());
            }).observe(output);
        },

        // How many text lines fit in the console, minus a reserve for the
        // pagination prompt and status bar so a full page never overflows.
        measureViewportLines() {
            if (!output) return 16;
            const usable = Math.floor(output.clientHeight / lineHeightPx()) - 5;
            return Math.max(8, Math.min(usable, 60));
        },

        // Called after each Blazor render: keep view glued to bottom only when pinned.
        afterRender() {
            if (output && pinned) {
                output.scrollTop = output.scrollHeight;
            }
        },

        scrollPage(direction) {
            if (!output) return;
            // Unpin before the (async) smooth scroll starts, or the re-render
            // triggered by the same keystroke glues the view back to the bottom.
            if (direction < 0) setPinned(false);
            output.scrollBy({ top: direction * output.clientHeight * 0.9, behavior: 'smooth' });
        },

        scrollToBottom() {
            if (!output) return;
            output.scrollTop = output.scrollHeight;
            setPinned(true);
        }
    };
})();
