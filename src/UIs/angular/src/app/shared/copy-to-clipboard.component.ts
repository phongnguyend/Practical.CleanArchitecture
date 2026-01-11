import { Component, Input, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-copy-to-clipboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './copy-to-clipboard.component.html',
  styleUrls: ['./copy-to-clipboard.component.css']
})
export class CopyToClipboardComponent implements OnDestroy {
  @Input() text: string = '';
  @Input() className: string = 'copy-icon fa fa-clipboard';
  @Input() title: string = 'Copy Data';

  copyStatus: string = '';
  private timeoutId: any = null;

  handleCopy(): void {
    navigator.clipboard
      .writeText(this.text)
      .then(() => {
        this.copyStatus = '✅ copied';
      })
      .catch(() => {
        this.copyStatus = '❌ cannot copy';
      });

    this.timeoutId = setTimeout(() => {
      this.copyStatus = '';
    }, 1000);
  }

  ngOnDestroy(): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }
}
