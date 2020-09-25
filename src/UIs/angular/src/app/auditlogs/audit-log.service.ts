import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { IAuditLogEntry } from "./audit-log";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class AuditLogService {
  private productUrl = environment.ResourceServer.Endpoint + "auditLogEntries";

  constructor(private http: HttpClient) {}

  getAudtLogs(): Observable<IAuditLogEntry[]> {
    return this.http.get<IAuditLogEntry[]>(this.productUrl);
  }
}
