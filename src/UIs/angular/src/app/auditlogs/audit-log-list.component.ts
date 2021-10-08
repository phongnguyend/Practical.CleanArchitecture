import { Component, OnInit } from "@angular/core";
import { IAuditLogEntry } from "./audit-log";
import { Store } from "@ngrx/store";
import { AuditLogState } from "./audit-log.reducer";
import * as actions from "./audit-log.actions";

@Component({
  selector: "app-audit-log-list",
  templateUrl: "./audit-log-list.component.html",
  styleUrls: ["./audit-log-list.component.css"],
})
export class AuditLogListComponent implements OnInit {
  auditLogs: IAuditLogEntry[] = [];
  constructor(private store: Store<{ auditLog: AuditLogState }>) {}

  ngOnInit(): void {
    this.store
      .select((state) => state.auditLog)
      .subscribe({
        next: (auditLog) => {
          this.auditLogs = auditLog.auditLogs;
        },
        error: (err) => {},
      });
    this.store.dispatch(actions.fetchAuditLogsStart());
    this.store.dispatch(actions.fetchAuditLogs());
  }
}
