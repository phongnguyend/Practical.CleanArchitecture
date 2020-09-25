import { Injectable } from "@angular/core";
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse
} from "@angular/common/http";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

import { AuthService } from "./auth.service";
import { environment } from "src/environments/environment";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private _authService: AuthService, private _router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (req.url.startsWith(environment.ResourceServer.Endpoint)) {
      var accessToken = this._authService.getAccessToken();
      const headers = req.headers.set("Authorization", `Bearer ${accessToken}`);
      const authReq = req.clone({ headers });

      return next.handle(authReq).pipe(
        tap(
          () => {},
          (error: any) => {
            var respError = error as HttpErrorResponse;
            if (
              respError &&
              (respError.status === 401 || respError.status === 403)
            ) {
              var currentUrl = this._router.url;
              this._authService.login(currentUrl);
            }
          }
        )
      );
    } else {
      return next.handle(req);
    }
  }
}
