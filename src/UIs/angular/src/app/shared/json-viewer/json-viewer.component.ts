import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  OnChanges,
  OnDestroy,
  TemplateRef,
  ViewChild,
  inject,
} from "@angular/core";
import { ModalModule, ModalRef, ModalService } from "../modal/modal";
import { mountJsonViewer } from "./jsonRuntime";

@Component({
  selector: "app-json-content",
  standalone: true,
  template: "",
  changeDetection: ChangeDetectionStrategy.Eager,
})
export class JsonContentComponent implements AfterViewInit, OnChanges, OnDestroy {
  @Input() jsonData: unknown;
  private host = inject(ElementRef<HTMLElement>);
  private ready = false;
  private cleanup?: () => void;
  ngAfterViewInit() {
    this.ready = true;
    this.render();
  }
  ngOnChanges() {
    if (this.ready) this.render();
  }
  private render() {
    this.cleanup?.();
    this.cleanup = mountJsonViewer(this.host.nativeElement, this.jsonData);
  }
  ngOnDestroy() {
    this.cleanup?.();
  }
}

@Component({
  selector: "app-json-viewer",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [ModalModule, JsonContentComponent],
  template: ` <button
      type="button"
      class="app-json-trigger"
      title="View JSON"
      aria-label="View JSON"
      (click)="open()"
    >
      {{ "{ }" }}
    </button>
    <ng-template #dialog>
      <app-modal-header>JSON Data</app-modal-header>
      <app-modal-body><app-json-content [jsonData]="jsonData" /></app-modal-body>
      <app-modal-footer
        ><button type="button" class="btn btn-secondary" modalClose>Close</button></app-modal-footer
      >
    </ng-template>`,
})
export class JsonViewerComponent implements OnDestroy {
  @Input() jsonData: unknown;
  @ViewChild("dialog") dialog!: TemplateRef<unknown>;
  private modals = inject(ModalService);
  private activeDialog?: ModalRef;
  open() {
    this.activeDialog?.close();
    this.activeDialog = this.modals.open(this.dialog, { width: "50rem" });
  }
  ngOnDestroy() {
    this.activeDialog?.close();
  }
}
