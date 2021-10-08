import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { IAuditLogEntry } from "./audit-log";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class AuditLogService {
  private auditLogUrl = environment.ResourceServer.Endpoint + "auditLogEntries";

  constructor(private http: HttpClient) {}

  getAuditLogs(): Observable<IAuditLogEntry[]> {
    return this.http.get<IAuditLogEntry[]>(this.auditLogUrl);
  }
}
