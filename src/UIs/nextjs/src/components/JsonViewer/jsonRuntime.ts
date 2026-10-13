// Framework adapters own this host; all untrusted values are rendered as text.
export function mountJsonViewer(host: HTMLElement, input: unknown): () => void {
  let value = input;
  let valid = true;
  let formatted: string;
  try {
    if (typeof input === "string") value = JSON.parse(input);
    formatted = JSON.stringify(value, null, 2);
    if (formatted === undefined) throw new Error("Not JSON");
    // Normalize non-JSON JS values and reject cycles before traversing.
    value = JSON.parse(formatted);
  } catch {
    valid = false;
    formatted = typeof input === "string" ? input : String(input);
  }
  let disposed = false;
  let mode = valid ? "tree" : "text";
  let wrap = true;
  const make = <K extends keyof HTMLElementTagNameMap>(
    tag: K,
    className = "",
    text?: string,
  ) => {
    const element = document.createElement(tag);
    element.className = className;
    if (text !== undefined) element.textContent = text;
    return element;
  };
  host.classList.add("app-json");
  const toolbar = make("div", "app-json-toolbar");
  toolbar.setAttribute("role", "group");
  toolbar.setAttribute("aria-label", "JSON viewer controls");
  const content = make("div", "app-json-content");
  content.tabIndex = 0;
  content.setAttribute("aria-label", "JSON content");
  const status = make(
    "div",
    "app-json-status",
    valid ? "Read-only JSON" : "Invalid JSON — showing original text",
  );
  status.setAttribute("role", "status");
  const button = (label: string, action: () => void) => {
    const element = make("button", "app-json-button", label);
    element.type = "button";
    element.onclick = action;
    toolbar.append(element);
    return element;
  };
  const tree = button("Tree", () => {
    mode = "tree";
    render();
  });
  tree.disabled = !valid;
  const text = button("Formatted", () => {
    mode = "text";
    render();
  });
  const collapse = button("Collapse all", () => {
    content.querySelectorAll("details").forEach((item) => {
      item.open = false;
    });
  });
  const wrapping = button("Wrap lines", () => {
    wrap = !wrap;
    updateControls();
  });
  const copy = button("Copy JSON", async () => {
    try {
      if (!navigator.clipboard?.writeText)
        throw new Error("Clipboard unavailable");
      await navigator.clipboard.writeText(formatted);
      if (!disposed) status.textContent = "Copied to clipboard";
    } catch {
      if (!disposed)
        status.textContent =
          "Unable to copy. Select the text and copy it manually.";
    }
  });
  copy.classList.add("app-json-copy");
  function updateControls() {
    tree.setAttribute("aria-pressed", String(mode === "tree"));
    text.setAttribute("aria-pressed", String(mode === "text"));
    wrapping.setAttribute("aria-pressed", String(wrap));
    collapse.hidden = mode !== "tree";
    content.classList.toggle("app-json-nowrap", !wrap);
  }
  function scalar(item: unknown): HTMLElement {
    const type = item === null ? "null" : typeof item;
    return make("span", `app-json-${type}`, JSON.stringify(item));
  }
  function node(item: unknown, key?: string): HTMLElement {
    const container = item !== null && typeof item === "object";
    const row = make(container ? "details" : "div", "app-json-node");
    const label = container ? make("summary") : row;
    if (key !== undefined)
      label.append(make("span", "app-json-key", JSON.stringify(key)), ": ");
    if (!container) {
      label.append(scalar(item));
      return row;
    }
    const array = Array.isArray(item);
    const entries = Object.entries(item as object);
    label.append(make("span", "app-json-bracket", array ? "[ ]" : "{ }"));
    label.append(
      make(
        "span",
        "app-json-count",
        ` ${entries.length} ${array ? "items" : "properties"}`,
      ),
    );
    row.append(label);
    const children = make("div", "app-json-children");
    let count = 0;
    const more = make(
      "button",
      "app-json-button app-json-more",
      "Load next 100 items",
    );
    more.type = "button";
    function load() {
      more.remove();
      const end = Math.min(count + 100, entries.length);
      for (; count < end; count++) {
        const entry = entries[count]!;
        children.append(node(entry[1], entry[0]));
      }
      if (count < entries.length) children.append(more);
    }
    more.onclick = () => {
      load();
      (children.lastElementChild as HTMLElement | null)?.focus();
    };
    // Populate only an opened branch so large/deep documents stay responsive.
    (row as HTMLDetailsElement).ontoggle = () => {
      if ((row as HTMLDetailsElement).open && count === 0) load();
    };
    row.append(children);
    return row;
  }
  function render() {
    content.replaceChildren();
    updateControls();
    if (mode === "tree") {
      const root = node(value);
      content.append(root);
      if (root instanceof HTMLDetailsElement) root.open = true;
      return;
    }
    const pre = make("pre", "app-json-code");
    // Very large documents remain selectable without creating thousands of spans.
    if (!valid || formatted.length > 200000) pre.textContent = formatted;
    else {
      const tokens =
        /"(?:\\.|[^"\\])*"\s*:|"(?:\\.|[^"\\])*"|-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?|\b(?:true|false|null)\b/g;
      let end = 0;
      for (const match of formatted.matchAll(tokens)) {
        pre.append(formatted.slice(end, match.index));
        const token = match[0];
        const kind = token.endsWith(":")
          ? "key"
          : token.startsWith('"')
            ? "string"
            : token === "null"
              ? "null"
              : /true|false/.test(token)
                ? "boolean"
                : "number";
        pre.append(make("span", `app-json-${kind}`, token));
        end = match.index! + token.length;
      }
      pre.append(formatted.slice(end));
    }
    content.append(pre);
  }
  host.replaceChildren(toolbar, content, status);
  render();
  return () => {
    disposed = true;
    host.replaceChildren();
  };
}
