// Console scroll/viewport helpers for the game terminal.
//
// Output model: when a command produces more text than fits the viewport,
// the view anchors to the TOP of the new text (reader continues downward at
// their own pace, "new text below" pill shows the way back). Shorter output
// keeps the classic follow-the-bottom behavior. This replaces the old
// "Press Enter to continue" page breaks.
window.gmConsole = (function () {
    let output = null;
    let dotNetRef = null;
    let pinned = true;           // view is glued to the bottom
    let burstStart = -1;         // scrollHeight when the current command was submitted
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
        },

        // Called when the player submits a command: return to the present and
        // remember where the new output will begin.
        beginOutput() {
            if (!output) return;
            output.scrollTop = output.scrollHeight;
            setPinned(true);
            burstStart = output.scrollHeight;
        },

        // Called after each Blazor render.
        afterRender() {
            if (!output || !pinned) return;

            // If this command's output has outgrown the viewport, anchor the
            // view to where the new text starts instead of its bottom.
            if (burstStart >= 0) {
                const newContentHeight = output.scrollHeight - burstStart;
                if (newContentHeight > output.clientHeight * 0.95) {
                    output.scrollTop = Math.max(0, burstStart - lineHeightPx());
                    setPinned(false);
                    dotNetRef?.invokeMethodAsync('OnAnchoredToNewText');
                    return;
                }
            }

            output.scrollTop = output.scrollHeight;
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
