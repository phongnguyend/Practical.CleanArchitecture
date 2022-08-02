import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { IAuditLogEntry, Paged } from "./audit-log";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class AuditLogService {
  private auditLogUrl = environment.ResourceServer.Endpoint + "auditLogEntries";

  constructor(private http: HttpClient) { }

  getAuditLogs(page: number, pageSize: number): Observable<Paged<IAuditLogEntry>> {
    return this.http.get<Paged<IAuditLogEntry>>(this.auditLogUrl + "/paged?page=" + page + "&pageSize=" + pageSize);
  }
}
