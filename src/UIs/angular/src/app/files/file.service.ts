import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError, tap, map } from "rxjs/operators";

import { IFile } from "./file";
import { environment } from "src/environments/environment";
import { IAuditLogEntry } from "../auditlogs/audit-log";

@Injectable({
  providedIn: "root",
})
export class FileService {
  private fileUrl = environment.ResourceServer.Endpoint + "files";

  constructor(private http: HttpClient) {}

  getFiles(): Observable<IFile[]> {
    return this.http
      .get<IFile[]>(this.fileUrl)
      .pipe(catchError(this.handleError));
  }

  getFile(id: string): Observable<IFile | undefined> {
    return this.http
      .get<IFile>(this.fileUrl + "/" + id)
      .pipe(catchError(this.handleError));
  }

  uploadFile(file: IFile): Observable<IFile | undefined> {
    const formData: FormData = new FormData();
    formData.append("formFile", file.formFile);
    formData.append("name", file.name);
    formData.append("description", file.description);
    formData.append("encrypted", file.encrypted.toString());
    return this.http
      .post<IFile>(this.fileUrl, formData)
      .pipe(catchError(this.handleError));
  }

  updateFile(file: IFile): Observable<IFile | undefined> {
    return this.http
      .put<IFile>(this.fileUrl + "/" + file.id, file)
      .pipe(catchError(this.handleError));
  }

  downloadFile(file: IFile): Observable<Blob> {
    return this.http
      .get(this.fileUrl + "/" + file.id + "/download", { responseType: "blob" })
      .pipe(catchError(this.handleError));
  }

  deleteFile(file: IFile): Observable<IFile | undefined> {
    return this.http
      .delete<IFile>(this.fileUrl + "/" + file.id)
      .pipe(catchError(this.handleError));
  }

  getAuditLogs(id: string): Observable<IAuditLogEntry[] | undefined> {
    return this.http
      .get<IAuditLogEntry[]>(this.fileUrl + "/" + id + "/auditLogs")
      .pipe(catchError(this.handleError));
  }

  private handleError(err: HttpErrorResponse) {
    // in a real world app, we may send the server to some remote logging infrastructure
    // instead of just logging it to the console
    let errorMessage = "";
    if (err.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }
}
