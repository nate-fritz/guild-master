// Console scroll/viewport helpers for the game terminal.
//
// Two output modes, chosen per render by whether combat is active:
//
// EXPLORATION - reading prose. When a command produces more text than fits
// the viewport, the view anchors ONCE to the top of the new text (reader
// continues downward; the bottom "new text below" pill offers the way back).
//
// COMBAT - a live feed. The view stays glued to the bottom so turns stream
// naturally; if the burst has pushed earlier text (room description, an
// NPC's last words) off the top unseen, a "new text above" pill appears at
// the top and jumps back to where the burst began.
window.gmConsole = (function () {
    let output = null;
    let dotNetRef = null;
    let pinned = true;           // view is glued to the bottom
    let burstStart = -1;         // scrollHeight when the current command was submitted
    let anchored = false;        // this burst already anchored once (exploration)
    let topSeen = true;          // user has viewed the start of this burst (combat)
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

    function showTopPill(show) {
        document.querySelector('.jump-pill-top')?.classList.toggle('visible', show);
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

    function updateTopPill() {
        // Combat-mode indicator: the burst's beginning is above the viewport
        // and the player hasn't deliberately scrolled up to see it yet.
        // (Only count it "seen" on an unpinned scroll-up - text that was
        // briefly on screen while streaming past doesn't count as read.)
        if (burstStart < 0) { showTopPill(false); return; }
        if (!pinned && output.scrollTop <= burstStart + 4) topSeen = true;
        showTopPill(!topSeen && output.scrollTop > burstStart + 4);
    }

    return {
        init(outputElementId, ref) {
            output = document.getElementById(outputElementId);
            dotNetRef = ref;
            if (!output) return;

            output.addEventListener('scroll', () => {
                setPinned(distanceFromBottom() <= PIN_THRESHOLD);
                updateTopPill();
            });
        },

        // Called when the player submits a command: return to the present.
        // Out of combat this also starts a fresh burst; during combat the
        // burst reference stays where the FIGHT began, so "new text above"
        // keeps pointing at the pre-combat narrative rather than resetting
        // on every action keystroke.
        beginOutput(combatActive) {
            if (!output) return;
            output.scrollTop = output.scrollHeight;
            setPinned(true);
            if (!combatActive) {
                burstStart = output.scrollHeight;
                anchored = false;
                topSeen = false;
                showTopPill(false);
            }
        },

        // Called after each Blazor render. combatActive selects the mode.
        afterRender(combatActive) {
            if (!output) return;

            if (combatActive) {
                // Live feed: follow the bottom while pinned, surface the
                // top pill if the burst start has scrolled out of view
                if (pinned) output.scrollTop = output.scrollHeight;
                updateTopPill();
                return;
            }

            if (!pinned) return;

            // Exploration: anchor at most once per burst, then follow normally
            if (!anchored && burstStart >= 0) {
                const newContentHeight = output.scrollHeight - burstStart;
                if (newContentHeight > output.clientHeight * 0.95) {
                    anchored = true;
                    topSeen = true;   // reader is at the burst start by definition
                    output.scrollTop = Math.max(0, burstStart - lineHeightPx());
                    setPinned(false);
                    dotNetRef?.invokeMethodAsync('OnAnchoredToNewText');
                    return;
                }
            }

            output.scrollTop = output.scrollHeight;
        },

        // Top pill: jump to where this burst began (combat scrollback)
        scrollToBurstStart() {
            if (!output || burstStart < 0) return;
            setPinned(false);   // unpin BEFORE the scroll so renders don't yank us down
            output.scrollTop = Math.max(0, burstStart - lineHeightPx());
            topSeen = true;
            showTopPill(false);
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
