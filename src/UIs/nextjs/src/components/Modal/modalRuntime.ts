// Framework-independent behavior for the application's locally rendered modals.
type Entry = { layer: HTMLElement; panel: HTMLElement; dismiss: () => void };
const stack: Entry[] = [];
const inertStates = new Map<HTMLElement, boolean>();
let originalOverflow = "";
let sequence = 0;

function focusable(panel: HTMLElement): HTMLElement[] {
  return Array.from(
    panel.querySelectorAll<HTMLElement>(
      'button, [href], input, select, textarea, [tabindex], [contenteditable="true"]',
    ),
  ).filter(
    (el) =>
      el.tabIndex >= 0 &&
      !el.matches(":disabled") &&
      !el.closest("[inert]") &&
      el.getClientRects().length > 0,
  );
}

function focusFirst(panel: HTMLElement) {
  const preferred = panel.querySelector<HTMLElement>("[data-modal-autofocus]");
  (preferred || focusable(panel)[0] || panel).focus({ preventScroll: true });
}

function updateBackground() {
  const top = stack[stack.length - 1];
  for (const child of Array.from(document.body.children)) {
    if (!(child instanceof HTMLElement)) continue;
    if (!inertStates.has(child)) inertStates.set(child, child.inert);
    child.inert = top
      ? child !== top.layer && !child.contains(top.layer)
      : inertStates.get(child)!;
  }
  stack.forEach((entry, index) => {
    entry.layer.style.zIndex = String(1050 + index * 10);
  });
  if (!top) {
    for (const [element, inert] of inertStates) element.inert = inert;
    inertStates.clear();
  }
}

export function mountModal(
  layer: HTMLElement,
  panel: HTMLElement,
  dismiss: () => void,
): () => void {
  const previousFocus =
    document.activeElement instanceof HTMLElement
      ? document.activeElement
      : null;
  if (!stack.length) {
    originalOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";
  }
  const entry = { layer, panel, dismiss };
  stack.push(entry);
  panel.setAttribute("role", "dialog");
  panel.setAttribute("aria-modal", "true");
  panel.tabIndex = -1;
  const title = panel.querySelector<HTMLElement>("[data-modal-title]");
  if (title) {
    if (!title.id) title.id = `app-modal-title-${++sequence}`;
    panel.setAttribute("aria-labelledby", title.id);
  } else if (!panel.hasAttribute("aria-label")) {
    panel.setAttribute("aria-label", "Dialog");
  }
  updateBackground();
  focusFirst(panel);
  const isTop = () => stack[stack.length - 1] === entry;
  const onKeyDown = (event: KeyboardEvent) => {
    if (!isTop()) return;
    if (event.key === "Escape" && !event.isComposing) {
      event.preventDefault();
      event.stopImmediatePropagation();
      dismiss();
    } else if (event.key === "Tab") {
      const items = focusable(panel);
      const first = items[0];
      const last = items[items.length - 1];
      if (!first) {
        event.preventDefault();
        panel.focus();
      } else if (
        event.shiftKey &&
        (document.activeElement === first || document.activeElement === panel)
      ) {
        event.preventDefault();
        last.focus();
      } else if (
        !event.shiftKey &&
        (document.activeElement === last || document.activeElement === panel)
      ) {
        event.preventDefault();
        first.focus();
      }
    }
  };
  const onFocus = (event: FocusEvent) => {
    if (isTop() && !panel.contains(event.target as Node)) focusFirst(panel);
  };
  const onBackdrop = (event: MouseEvent) => {
    if (isTop() && event.target === layer) dismiss();
  };
  document.addEventListener("keydown", onKeyDown, true);
  document.addEventListener("focusin", onFocus);
  layer.addEventListener("click", onBackdrop);
  let disposed = false;
  return () => {
    if (disposed) return;
    disposed = true;
    const wasTop = isTop();
    stack.splice(stack.indexOf(entry), 1);
    document.removeEventListener("keydown", onKeyDown, true);
    document.removeEventListener("focusin", onFocus);
    layer.removeEventListener("click", onBackdrop);
    updateBackground();
    if (!stack.length) document.body.style.overflow = originalOverflow;
    if (wasTop)
      queueMicrotask(() => {
        if (previousFocus?.isConnected && !previousFocus.closest("[inert]"))
          previousFocus.focus({ preventScroll: true });
        else if (stack.length) focusFirst(stack[stack.length - 1].panel);
      });
  };
}

export interface ConfirmOptions {
  title?: string;
  confirmText?: string;
  cancelText?: string;
  alert?: boolean;
}

// Async replacement for browser confirm/alert, using the same Header/Body/Footer layout.
export function confirmModal(
  message: string,
  options: ConfirmOptions = {},
): Promise<boolean> {
  return new Promise((resolve) => {
    const layer = document.createElement("div");
    layer.className = "app-modal-layer";
    const panel = document.createElement("section");
    panel.className = "app-modal-panel";
    const header = document.createElement("header");
    header.className = "app-modal-header";
    const title = document.createElement("h2");
    title.className = "app-modal-title";
    title.dataset.modalTitle = "";
    title.textContent =
      options.title || (options.alert ? "Notice" : "Confirm action");
    const close = document.createElement("button");
    close.type = "button";
    close.className = "app-modal-close";
    close.textContent = String.fromCharCode(215);
    close.setAttribute("aria-label", "Close dialog");
    const body = document.createElement("div");
    body.className = "app-modal-body";
    body.textContent = message;
    const footer = document.createElement("footer");
    footer.className = "app-modal-footer";
    const cancel = document.createElement("button");
    cancel.type = "button";
    cancel.className = "btn btn-secondary";
    cancel.textContent = options.cancelText || "Cancel";
    cancel.dataset.modalAutofocus = "";
    const accept = document.createElement("button");
    accept.type = "button";
    accept.className = "btn btn-primary";
    accept.textContent =
      options.confirmText || (options.alert ? "OK" : "Confirm");
    header.append(title, close);
    if (!options.alert) footer.append(cancel);
    else accept.dataset.modalAutofocus = "";
    footer.append(accept);
    panel.append(header, body, footer);
    layer.append(panel);
    document.body.append(layer);
    const cleanup = mountModal(layer, panel, () => finish(false));
    let settled = false;
    function finish(value: boolean) {
      if (settled) return;
      settled = true;
      cleanup();
      layer.remove();
      resolve(value);
    }
    close.onclick = () => finish(false);
    cancel.onclick = () => finish(false);
    accept.onclick = () => finish(true);
  });
}
