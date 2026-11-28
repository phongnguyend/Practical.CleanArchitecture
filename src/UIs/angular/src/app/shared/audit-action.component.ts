import { Component, Input, ChangeDetectionStrategy } from "@angular/core";

@Component({
  selector: "app-audit-action",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: '<span class="badge" [attr.data-action]="kind">{{ label }}</span>',
  styleUrls: ["./audit-action.component.css"],
})
export class AuditActionComponent {
  @Input() action: string | null | undefined;

  get label(): string {
    return formatAuditAction(this.action);
  }

  get kind(): string {
    return this.label.toUpperCase().split(/[_\s-]/)[0];
  }
}

function formatAuditAction(action?: string | null): string {
  return (action?.trim() || "Event").replace(/_/g, " ");
}
