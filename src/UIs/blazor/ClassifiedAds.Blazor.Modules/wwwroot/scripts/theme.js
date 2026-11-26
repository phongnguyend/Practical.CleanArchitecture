window.classifiedAdsTheme = (() => {
    const media = window.matchMedia('(prefers-color-scheme: dark)');
    function apply() {
        const choice = localStorage.getItem('classifiedads-theme') || 'system';
        document.documentElement.dataset.bsTheme = choice === 'system' ? (media.matches ? 'dark' : 'light') : choice;
    }
    media.addEventListener('change', apply);
    apply();
    return {
        set(choice) {
            localStorage.setItem('classifiedads-theme', choice);
            apply();
            document.querySelectorAll('.theme-switcher').forEach(element => {
                element.open = false;
            });
        }
    };
})();

window.classifiedAdsDialog = (() => {
    let previous;
    let handler;
    let dialog;
    return {
        open(element, reference) {
            previous = document.activeElement;
            dialog = element;
            document.body.style.overflow = 'hidden';
            const focusable = () => [...element.querySelectorAll('button, a[href], input, select, textarea, [tabindex="0"]')].filter(item => !item.disabled);
            handler = event => {
                if (event.key === 'Escape') {
                    event.preventDefault();
                    reference.invokeMethodAsync('Dismiss');
                }
                if (event.key === 'Tab') {
                    const items = focusable();
                    const first = items[0] || element;
                    const last = items[items.length - 1] || element;
                    if (event.shiftKey && (document.activeElement === first || document.activeElement === element)) {
                        event.preventDefault();
                        last.focus();
                    } else if (!event.shiftKey && document.activeElement === last) {
                        event.preventDefault();
                        first.focus();
                    }
                }
            };
            element.addEventListener('keydown', handler);
            (focusable()[0] || element).focus();
        },
        close() {
            dialog?.removeEventListener('keydown', handler);
            document.body.style.overflow = '';
            if (previous?.isConnected) {
                previous.focus();
            }
            dialog = null;
            previous = null;
        }
    };
})();
