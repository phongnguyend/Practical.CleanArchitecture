"use client";

import { useEffect, useRef, useState } from "react";
import { Check, Monitor, Moon, Sun } from "lucide-react";
import "./ThemeSwitcher.css";

type Theme = "system" | "light" | "dark";
const storageKey = "classifiedads-theme";
const choices = [
  { value: "system", label: "System", Icon: Monitor },
  { value: "light", label: "Light", Icon: Sun },
  { value: "dark", label: "Dark", Icon: Moon },
] as const;

const readTheme = (): Theme => {
  try {
    const stored = localStorage.getItem(storageKey);
    return stored === "light" || stored === "dark" ? stored : "system";
  } catch {
    return "system";
  }
};

const applyTheme = (theme: Theme) => {
  const dark = theme === "dark" || (theme === "system" && window.matchMedia("(prefers-color-scheme: dark)").matches);
  document.documentElement.setAttribute("data-bs-theme", dark ? "dark" : "light");
};

const ThemeSwitcher = () => {
  const [theme, setTheme] = useState<Theme>("system");
  const menu = useRef<HTMLDetailsElement>(null);

  useEffect(() => {
    const saved = readTheme();
    setTheme(saved);
    applyTheme(saved);
    const media = window.matchMedia("(prefers-color-scheme: dark)");
    const onSystemChange = () => { if (readTheme() === "system") applyTheme("system"); };
    const onStorage = (event: StorageEvent) => {
      if (event.key === storageKey) {
        const updated = readTheme();
        setTheme(updated);
        applyTheme(updated);
      }
    };
    const onOutside = (event: PointerEvent) => {
      if (menu.current && !menu.current.contains(event.target as Node)) menu.current.removeAttribute("open");
    };
    document.addEventListener("pointerdown", onOutside);
    media.addEventListener("change", onSystemChange);
    window.addEventListener("storage", onStorage);
    return () => {
      document.removeEventListener("pointerdown", onOutside);
      media.removeEventListener("change", onSystemChange);
      window.removeEventListener("storage", onStorage);
    };
  }, []);

  const choose = (value: Theme) => {
    try { localStorage.setItem(storageKey, value); } catch { /* The choice still applies for this page. */ }
    setTheme(value);
    applyTheme(value);
    menu.current?.removeAttribute("open");
  };

  const CurrentIcon = choices.find((choice) => choice.value === theme)!.Icon;
  return (
    <details className="theme-switcher" ref={menu}>
      <summary className="theme-switcher-trigger" title={`Theme: ${theme}`} aria-label={`Theme: ${theme}. Choose appearance`}>
        <CurrentIcon size={20} aria-hidden="true" />
      </summary>
      <div className="theme-switcher-menu" role="group" aria-label="Appearance">
        {choices.map(({ value, label, Icon }) => (
          <button key={value} type="button" className="theme-switcher-option" aria-pressed={theme === value} onClick={() => choose(value)}>
            <Icon size={17} aria-hidden="true" /> <span>{label}</span>
            {theme === value && <Check size={16} className="theme-switcher-check" aria-hidden="true" />}
          </button>
        ))}
      </div>
    </details>
  );
};

export default ThemeSwitcher;
