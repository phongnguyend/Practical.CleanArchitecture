import { Component, Input, OnDestroy, ChangeDetectionStrategy } from "@angular/core";
import { NgIcon } from "@ng-icons/core";

@Component({
  selector: "app-copy-to-clipboard",
  standalone: true,
  imports: [NgIcon],
  templateUrl: "./copy-to-clipboard.component.html",
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ["./copy-to-clipboard.component.css"],
})
export class CopyToClipboardComponent implements OnDestroy {
  @Input() text: string = "";
  @Input() className: string = "copy-icon";
  @Input() title: string = "Copy Data";

  copyStatus: string = "";
  private timeoutId: any = null;

  async handleCopy(): Promise<void> {
    clearTimeout(this.timeoutId);
    try {
      await navigator.clipboard.writeText(this.text);
      this.copyStatus = "Copied to clipboard";
    } catch {
      this.copyStatus = "Unable to copy";
    }
    this.timeoutId = setTimeout(() => {
      this.copyStatus = "";
    }, 2000);
  }

  ngOnDestroy(): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }
}
