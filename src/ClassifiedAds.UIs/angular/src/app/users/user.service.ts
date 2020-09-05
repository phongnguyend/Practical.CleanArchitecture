import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError, tap, map } from "rxjs/operators";

import { IUser } from "./user";
import { environment } from "src/environments/environment";
import { IAuditLogEntry } from "../auditlogs/audit-log";

@Injectable({
  providedIn: "root",
})
export class UserService {
  private userUrl = environment.ResourceServer.Endpoint + "users";

  constructor(private http: HttpClient) {}

  getUsers(): Observable<IUser[]> {
    return this.http
      .get<IUser[]>(this.userUrl)
      .pipe(catchError(this.handleError));
  }

  getUser(id: string): Observable<IUser | undefined> {
    return this.http
      .get<IUser>(this.userUrl + "/" + id)
      .pipe(catchError(this.handleError));
  }

  addUser(user: IUser): Observable<IUser | undefined> {
    return this.http
      .post<IUser>(this.userUrl, user)
      .pipe(catchError(this.handleError));
  }

  updateUser(user: IUser): Observable<IUser | undefined> {
    return this.http
      .put<IUser>(this.userUrl + "/" + user.id, user)
      .pipe(catchError(this.handleError));
  }

  setPassword(id: string, password: string): Observable<IUser | undefined> {
    return this.http
      .put<IUser>(this.userUrl + "/" + id + "/password", {
        id: id,
        password: password,
      })
      .pipe(catchError(this.handleSetPasswordError));
  }

  deleteUser(user: IUser): Observable<IUser | undefined> {
    return this.http
      .delete<IUser>(this.userUrl + "/" + user.id)
      .pipe(catchError(this.handleError));
  }

  getAuditLogs(id: string): Observable<IAuditLogEntry[] | undefined> {
    return this.http
      .get<IAuditLogEntry[]>(this.userUrl + "/" + id + "/auditLogs")
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

  private handleSetPasswordError(err: HttpErrorResponse) {
    if (err.error) {
      return throwError(err.error);
    }
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
