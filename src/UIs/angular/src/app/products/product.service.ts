import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError, tap, map } from "rxjs/operators";

import { IProduct } from "./product";
import { environment } from "src/environments/environment";
import { IAuditLogEntry } from "../auditlogs/audit-log";

@Injectable({
  providedIn: "root",
})
export class ProductService {
  private productUrl = environment.ResourceServer.Endpoint + "products";

  constructor(private http: HttpClient) {}

  getProducts(): Observable<IProduct[]> {
    return this.http
      .get<IProduct[]>(this.productUrl)
      .pipe(catchError(this.handleError));
  }

  getProduct(id: string): Observable<IProduct | undefined> {
    return this.http
      .get<IProduct>(this.productUrl + "/" + id)
      .pipe(catchError(this.handleError));
  }

  addProduct(product: IProduct): Observable<IProduct | undefined> {
    return this.http
      .post<IProduct>(this.productUrl, product)
      .pipe(catchError(this.handleError));
  }

  updateProduct(product: IProduct): Observable<IProduct | undefined> {
    return this.http
      .put<IProduct>(this.productUrl + "/" + product.id, product)
      .pipe(catchError(this.handleError));
  }

  deleteProduct(product: IProduct): Observable<IProduct | undefined> {
    return this.http
      .delete<IProduct>(this.productUrl + "/" + product.id)
      .pipe(catchError(this.handleError));
  }

  getAuditLogs(id: string): Observable<IAuditLogEntry[] | undefined> {
    return this.http
      .get<IAuditLogEntry[]>(this.productUrl + "/" + id + "/auditLogs")
      .pipe(catchError(this.handleError));
  }

  exportAsPdf() {
    return this.http
      .get(this.productUrl + "/ExportAsPdf", { responseType: "blob" })
      .pipe(catchError(this.handleError));
  }

  exportAsCsv() {
    return this.http
      .get(this.productUrl + "/ExportAsCsv", { responseType: "blob" })
      .pipe(catchError(this.handleError));
  }

  importCsvFile(file: File): Observable<IProduct[] | undefined> {
    const formData: FormData = new FormData();
    formData.append("formFile", file);
    return this.http.post<IProduct[]>(this.productUrl + "/ImportCsv", formData);
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
