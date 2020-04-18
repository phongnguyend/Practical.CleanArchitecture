import { Component, OnInit } from "@angular/core";
import { AuditLogService } from "./audit-log.service";
import { IAuditLogEntry } from "./audit-log";

@Component({
  selector: "app-audit-log-list",
  templateUrl: "./audit-log-list.component.html",
  styleUrls: ["./audit-log-list.component.css"],
})
export class AuditLogListComponent implements OnInit {
  auditLogs: IAuditLogEntry[] = [];
  constructor(private auditLogService: AuditLogService) {}

  ngOnInit(): void {
    this.auditLogService.getAudtLogs().subscribe({
      next: (auditLogs) => {
        this.auditLogs = auditLogs;
      },
      error: (err) => {},
    });
  }
}
