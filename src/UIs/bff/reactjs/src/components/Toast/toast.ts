export type ToastKind = "success" | "error" | "warning" | "info";
export interface ToastOptions {
  title?: string;
  duration?: number | false;
}
interface Entry {
  id: number;
  kind: ToastKind;
  message: string;
  title: string;
  duration: number;
  remaining: number;
  pauses: Set<string>;
}
const entries: Entry[] = [];
const listeners = new Set<() => void>();
let sequence = 0;
const titles: Record<ToastKind, string> = {
  success: "Success",
  error: "Error",
  warning: "Warning",
  info: "Information",
};
const notify = () => listeners.forEach((listener) => listener());
function show(kind: ToastKind, message: string, options: ToastOptions = {}): number {
  if (typeof window === "undefined") return 0;
  const title = options.title || titles[kind];
  const duplicate = entries.find(
    (item) => item.kind === kind && item.message === String(message) && item.title === title
  );
  if (duplicate) {
    duplicate.remaining = duplicate.duration;
    notify();
    return duplicate.id;
  }
  const requested =
    options.duration === false ? 0 : (options.duration ?? (kind === "error" ? 8000 : 5000));
  const duration = Number.isFinite(requested) ? Math.max(0, requested) : 5000;
  const id = ++sequence;
  // Keep the visible four intact and bound queued notifications during bursts.
  if (entries.length >= 50) entries.splice(4, 1);
  entries.push({
    id,
    kind,
    message: String(message),
    title,
    duration,
    remaining: duration,
    pauses: new Set(),
  });
  notify();
  return id;
}
export const toast = {
  success: (message: string, options?: ToastOptions) => show("success", message, options),
  error: (message: string, options?: ToastOptions) => show("error", message, options),
  warning: (message: string, options?: ToastOptions) => show("warning", message, options),
  info: (message: string, options?: ToastOptions) => show("info", message, options),
  dismiss(id: number) {
    const index = entries.findIndex((entry) => entry.id === id);
    if (index >= 0) {
      entries.splice(index, 1);
      notify();
    }
  },
  clear() {
    entries.splice(0);
    notify();
  },
};

/** Mount once at the application root. No document access occurs during SSR. */
export function mountToasts(): () => void {
  const host = document.createElement("section");
  host.className = "app-toasts";
  host.setAttribute("aria-label", "Notifications");
  const cards = new Map<number, HTMLElement>();
  const polite = document.createElement("div");
  const urgent = document.createElement("div");
  for (const region of [polite, urgent]) {
    region.className = "app-toast-sr";
    region.setAttribute("aria-live", region === urgent ? "assertive" : "polite");
    region.setAttribute("aria-atomic", "true");
    host.append(region);
  }
  document.body.append(host);
  let disposed = false;
  function render() {
    const visible = entries.slice(0, 4);
    for (const [id, card] of cards) {
      if (!visible.some((entry) => entry.id === id)) {
        // Preserve keyboard navigation when a focused dismiss button disappears.
        const focused = card.contains(document.activeElement);
        card.remove();
        cards.delete(id);
        if (focused) host.querySelector<HTMLButtonElement>("button")?.focus();
      }
    }
    const announcements: Record<string, string[]> = { polite: [], urgent: [] };
    for (const entry of visible) {
      if (cards.has(entry.id)) continue;
      const card = document.createElement("article");
      card.className = `app-toast app-toast--${entry.kind}`;
      card.setAttribute("aria-label", entry.title);
      const icon = document.createElement("span");
      icon.className = "app-toast-icon";
      icon.setAttribute("aria-hidden", "true");
      icon.textContent = { success: "✓", error: "!", warning: "!", info: "i" }[entry.kind];
      const body = document.createElement("div");
      body.className = "app-toast-body";
      const title = document.createElement("strong");
      title.textContent = entry.title;
      const message = document.createElement("p");
      message.textContent = entry.message;
      body.append(title, message);
      const close = document.createElement("button");
      close.type = "button";
      close.className = "app-toast-close";
      close.textContent = "×";
      close.setAttribute("aria-label", `Dismiss ${entry.title} notification`);
      close.onclick = () => toast.dismiss(entry.id);
      const progress = document.createElement("div");
      progress.className = "app-toast-progress";
      progress.setAttribute("aria-hidden", "true");
      progress.hidden = !entry.duration;
      card.onmouseenter = () => entry.pauses.add("hover");
      card.onmouseleave = () => entry.pauses.delete("hover");
      card.addEventListener("focusin", () => entry.pauses.add("focus"));
      card.addEventListener("focusout", (event) => {
        if (!card.contains(event.relatedTarget as Node | null)) entry.pauses.delete("focus");
      });
      card.append(icon, body, close, progress);
      host.append(card);
      cards.set(entry.id, card);
      announcements[entry.kind === "error" ? "urgent" : "polite"]!.push(
        `${entry.title}: ${entry.message}`
      );
    }
    queueMicrotask(() => {
      if (disposed) return;
      if (announcements.polite!.length) polite.textContent = announcements.polite!.join(". ");
      if (announcements.urgent!.length) urgent.textContent = announcements.urgent!.join(". ");
    });
  }
  listeners.add(render);
  render();
  let previous = performance.now();
  const visibilityChanged = () => {
    previous = performance.now();
  };
  document.addEventListener("visibilitychange", visibilityChanged);
  const timer = window.setInterval(() => {
    const now = performance.now();
    const elapsed = now - previous;
    previous = now;
    // An open modal makes this host inert: preserve notifications until usable.
    if (document.hidden || host.inert) return;
    for (const entry of entries.slice(0, 4)) {
      if (!entry.duration || entry.pauses.size) continue;
      entry.remaining -= elapsed;
      const progress = cards.get(entry.id)?.querySelector<HTMLElement>(".app-toast-progress");
      if (progress)
        progress.style.transform = `scaleX(${Math.max(0, entry.remaining / entry.duration)})`;
      if (entry.remaining <= 0) toast.dismiss(entry.id);
    }
  }, 100);
  return () => {
    disposed = true;
    window.clearInterval(timer);
    document.removeEventListener("visibilitychange", visibilityChanged);
    listeners.delete(render);
    entries.forEach((entry) => entry.pauses.clear());
    host.remove();
  };
}
