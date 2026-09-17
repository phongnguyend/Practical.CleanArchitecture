import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { NgIcon } from "@ng-icons/core";

type Theme = "system" | "light" | "dark";
const storageKey = "classifiedads-theme";

@Component({
  selector: "app-theme-switcher",
  standalone: true,
  imports: [NgIcon],
  template: `
    <details #menu class="theme-switcher">
      <summary class="theme-switcher-trigger" [title]="'Theme: ' + theme" [attr.aria-label]="'Theme: ' + theme + '. Choose appearance'">
        <ng-icon [name]="currentIcon" size="20" aria-hidden="true" />
      </summary>
      <div class="theme-switcher-menu" role="group" aria-label="Appearance">
        <button type="button" class="theme-switcher-option" [attr.aria-pressed]="theme === 'system'" (click)="choose('system')">
          <ng-icon name="lucideMonitor" size="17" aria-hidden="true" /><span>System</span>
          @if (theme === 'system') { <ng-icon name="lucideCheck" size="16" class="theme-switcher-check" aria-hidden="true" /> }
        </button>
        <button type="button" class="theme-switcher-option" [attr.aria-pressed]="theme === 'light'" (click)="choose('light')">
          <ng-icon name="lucideSun" size="17" aria-hidden="true" /><span>Light</span>
          @if (theme === 'light') { <ng-icon name="lucideCheck" size="16" class="theme-switcher-check" aria-hidden="true" /> }
        </button>
        <button type="button" class="theme-switcher-option" [attr.aria-pressed]="theme === 'dark'" (click)="choose('dark')">
          <ng-icon name="lucideMoon" size="17" aria-hidden="true" /><span>Dark</span>
          @if (theme === 'dark') { <ng-icon name="lucideCheck" size="16" class="theme-switcher-check" aria-hidden="true" /> }
        </button>
      </div>
    </details>
  `,
  styleUrls: ["./theme-switcher.component.css"],
})
export class ThemeSwitcherComponent implements OnInit, OnDestroy {
  @ViewChild("menu") menu?: ElementRef<HTMLDetailsElement>;
  theme: Theme = "system";
  private media = window.matchMedia("(prefers-color-scheme: dark)");

  get currentIcon(): "lucideMonitor" | "lucideSun" | "lucideMoon" {
    return this.theme === "light" ? "lucideSun" : this.theme === "dark" ? "lucideMoon" : "lucideMonitor";
  }

  private readTheme(): Theme {
    try {
      const stored = localStorage.getItem(storageKey);
      return stored === "light" || stored === "dark" ? stored : "system";
    } catch {
      return "system";
    }
  }

  private applyTheme(theme: Theme) {
    const dark = theme === "dark" || (theme === "system" && this.media.matches);
    document.documentElement.setAttribute("data-bs-theme", dark ? "dark" : "light");
  }

  private onSystemChange = () => {
    if (this.readTheme() === "system") this.applyTheme("system");
  };

  private onOutside = (event: PointerEvent) => {
    if (this.menu && !this.menu.nativeElement.contains(event.target as Node)) this.menu.nativeElement.removeAttribute("open");
  };

  private onStorage = (event: StorageEvent) => {
    if (event.key === storageKey) {
      this.theme = this.readTheme();
      this.applyTheme(this.theme);
    }
  };

  ngOnInit() {
    this.theme = this.readTheme();
    this.applyTheme(this.theme);
    this.media.addEventListener("change", this.onSystemChange);
    document.addEventListener("pointerdown", this.onOutside);
    window.addEventListener("storage", this.onStorage);
  }

  ngOnDestroy() {
    this.media.removeEventListener("change", this.onSystemChange);
    document.removeEventListener("pointerdown", this.onOutside);
    window.removeEventListener("storage", this.onStorage);
  }

  choose(theme: Theme) {
    try { localStorage.setItem(storageKey, theme); } catch { /* Apply for this page. */ }
    this.theme = theme;
    this.applyTheme(theme);
    this.menu?.nativeElement.removeAttribute("open");
  }
}
