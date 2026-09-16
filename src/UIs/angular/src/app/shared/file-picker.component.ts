import { Component, EventEmitter, Input, Output } from "@angular/core";
import { ActionIconComponent } from "./action-icon.component";

@Component({
  selector: "app-file-picker",
  standalone: true,
  imports: [ActionIconComponent],
  template: `
    <div>
      <label class="file-picker" [class.file-picker-invalid]="invalid">
        <input [id]="inputId" [name]="name" type="file" [accept]="accept" [title]="file?.name || ''" [attr.aria-label]="label" [attr.aria-describedby]="inputId + '-hint'" [attr.aria-invalid]="invalid" (change)="fileSelected($event)" />
        <span class="file-picker-icon"><app-action-icon action="import" /></span>
        <span class="file-picker-copy">
          <strong class="file-picker-title">{{ file ? file.name : 'Choose a file' }}</strong>
          <span class="file-picker-hint" [id]="inputId + '-hint'">{{ hint }}</span>
        </span>
        <span class="file-picker-button">Browse</span>
      </label>
      @if (invalid) { <div class="text-danger small mt-1">Select a file</div> }
    </div>
  `,
  styleUrls: ["./file-picker.component.css"],
})
export class FilePickerComponent {
  @Input() inputId = "";
  @Input() name = "";
  @Input() label = "File";
  @Input() hint = "Any file type";
  @Input() file: File | null = null;
  @Input() invalid = false;
  @Input() accept = "";
  @Output() fileChange = new EventEmitter<FileList | null>();

  fileSelected(event: Event) {
    this.fileChange.emit((event.target as HTMLInputElement).files);
  }
}
