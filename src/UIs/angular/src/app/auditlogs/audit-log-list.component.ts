import { Component, OnInit } from "@angular/core";
import { IAuditLogEntry } from "./audit-log";
import { Store } from "@ngrx/store";
import { AuditLogState } from "./audit-log.reducer";
import * as actions from "./audit-log.actions";

@Component({
    selector: "app-audit-log-list",
    templateUrl: "./audit-log-list.component.html",
    styleUrls: ["./audit-log-list.component.css"],
    standalone: false
})
export class AuditLogListComponent implements OnInit {
  auditLogs: IAuditLogEntry[] = [];
  totalItems: number = 0;
  currentPage: number = 1;
  constructor(private store: Store<{ auditLog: AuditLogState }>) {}

  ngOnInit(): void {
    this.store
      .select((state) => state.auditLog)
      .subscribe({
        next: (auditLog) => {
          this.auditLogs = auditLog.auditLogs;
          this.totalItems = auditLog.totalItems;
        },
        error: (err) => {},
      });
    this.store.dispatch(actions.fetchAuditLogsStart());
    this.store.dispatch(actions.fetchAuditLogs({ page: 1, pageSize: 5 }));
  }

  pageChanged(page: number): void {
    if (this.currentPage !== page) {
      this.store.dispatch(actions.fetchAuditLogsStart());
      this.store.dispatch(actions.fetchAuditLogs({ page: page, pageSize: 5 }));
      this.currentPage = page;
    }
  }
}
