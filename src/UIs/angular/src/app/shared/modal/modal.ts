import { NgTemplateOutlet } from "@angular/common";
import {
  ApplicationRef,
  ChangeDetectionStrategy,
  Component,
  createComponent,
  Directive,
  EnvironmentInjector,
  HostListener,
  inject,
  Injectable,
  Input,
  NgModule,
  TemplateRef,
} from "@angular/core";
import { mountModal } from "./modalRuntime";
import { NavigationStart, Router } from "@angular/router";

export class ModalRef<T = unknown> {
  constructor(private dispose: () => void) {}
  close(_result?: T): void {
    this.dispose();
  }
}

@Component({
  selector: "app-modal",
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: true,
  imports: [NgTemplateOutlet],
  template: `<section
    class="app-modal-panel"
    role="dialog"
    aria-modal="true"
    tabindex="-1"
    [style.width]="width"
    [style.max-width]="maxWidth"
  >
    <ng-container *ngTemplateOutlet="template" />
  </section>`,
  host: { class: "app-modal-layer" },
})
export class ModalComponent {
  @Input() template: TemplateRef<unknown> | null = null;
  @Input() width = "100%";
  @Input() maxWidth = "32rem";
}

@Injectable({ providedIn: "root" })
export class ModalService {
  private app = inject(ApplicationRef);
  private environment = inject(EnvironmentInjector);
  private refs: ModalRef[] = [];
  private navigation = inject(Router, { optional: true })?.events.subscribe((event) => {
    if (event instanceof NavigationStart) this.closeAll();
  });

  open<T>(template: TemplateRef<T>, options: { width?: string; maxWidth?: string } = {}): ModalRef {
    const host = document.createElement("app-modal");
    document.body.append(host);
    const component = createComponent(ModalComponent, {
      hostElement: host,
      environmentInjector: this.environment,
    });
    component.setInput("template", template);
    component.setInput("width", options.width || "100%");
    component.setInput("maxWidth", options.maxWidth || "calc(100vw - 2rem)");
    this.app.attachView(component.hostView);
    component.changeDetectorRef.detectChanges();
    const panel = host.querySelector<HTMLElement>(".app-modal-panel")!;
    if (!panel.querySelector(".app-modal-footer")) {
      const footer = document.createElement("footer");
      footer.className = "app-modal-footer";
      const close = document.createElement("button");
      close.type = "button";
      close.className = "btn btn-secondary";
      close.textContent = "Close";
      close.onclick = () => ref.close();
      footer.append(close);
      panel.append(footer);
    }
    let closed = false;
    const ref = new ModalRef(() => {
      if (closed) return;
      closed = true;
      cleanup();
      this.refs = this.refs.filter((item) => item !== ref);
      this.app.detachView(component.hostView);
      component.destroy();
      host.remove();
    });
    this.refs.push(ref);
    const cleanup = mountModal(host, panel, () => ref.close());
    return ref;
  }

  closeTop(): void {
    this.refs[this.refs.length - 1]?.close();
  }
  private closeAll(): void {
    [...this.refs].reverse().forEach((ref) => ref.close());
  }
  ngOnDestroy(): void {
    this.navigation?.unsubscribe();
    this.closeAll();
  }
}

@Component({
  selector: "app-modal-header",
  standalone: true,
  template: `<h2 class="app-modal-title" data-modal-title><ng-content /></h2>
    <button
      type="button"
      class="app-modal-close"
      aria-label="Close dialog"
      (click)="modals.closeTop()"
    >
      &#215;
    </button>`,
  host: { class: "app-modal-header" },
})
export class ModalHeaderComponent {
  protected modals = inject(ModalService);
}

@Component({
  selector: "app-modal-body",
  standalone: true,
  template: "<ng-content />",
  host: { class: "app-modal-body" },
})
export class ModalBodyComponent {}

@Component({
  selector: "app-modal-footer",
  standalone: true,
  template: "<ng-content />",
  host: { class: "app-modal-footer" },
})
export class ModalFooterComponent {}

@Directive({ selector: "[modalClose]", standalone: true })
export class ModalCloseDirective {
  private modals = inject(ModalService);
  @HostListener("click") close() {
    this.modals.closeTop();
  }
}

@NgModule({
  imports: [
    ModalComponent,
    ModalHeaderComponent,
    ModalBodyComponent,
    ModalFooterComponent,
    ModalCloseDirective,
  ],
  exports: [
    ModalComponent,
    ModalHeaderComponent,
    ModalBodyComponent,
    ModalFooterComponent,
    ModalCloseDirective,
  ],
})
export class ModalModule {}
