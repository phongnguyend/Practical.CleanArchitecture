import { enableProdMode, importProvidersFrom, inject } from "@angular/core";
import { bootstrapApplication } from "@angular/platform-browser";
import { BrowserModule } from "@angular/platform-browser";
import { provideRouter } from "@angular/router";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { StoreModule } from "@ngrx/store";
import { StoreDevtoolsModule } from "@ngrx/store-devtools";
import { EffectsModule } from "@ngrx/effects";
import { ToastrModule } from "ngx-toastr";
import { ErrorHandler, provideAppInitializer } from "@angular/core";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { TimepickerModule } from "ngx-bootstrap/timepicker";
import { ModalModule } from "ngx-bootstrap/modal";

import { AppComponent } from "./app/app.component";
import { environment } from "./environments/environment";
import { routes } from "./app/app-routing.module";
import { GlobalErrorHandler } from "./app/shared/global-error-handler";
import { authInterceptorProvider } from "./app/auth/auth.interceptor";
import { loggingInterceptorProvider } from "./app/logging/logging.interceptor";
import { AuthInitializer } from "./app/auth/auth.initializer";
import { AuthService } from "./app/auth/auth.service";

// Guards
import { ProductDetailGuard } from "./app/products/view-product-details/product-detail.guard";
import { AddProductGuard } from "./app/products/add-product/add-product.guard";
import { EditProductGuard } from "./app/products/edit-product/edit-product.guard";

// Audit Log State
import { auditLogReducer } from "./app/auditlogs/audit-log.reducer";
import { AuditLogEffects } from "./app/auditlogs/audit-log.effects";

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      BrowserModule,
      BrowserAnimationsModule,
      StoreModule.forRoot({}),
      StoreModule.forFeature("auditLog", auditLogReducer),
      StoreDevtoolsModule.instrument({
        name: "Practical.CleanArchitecture App DevTools",
        maxAge: 25,
        logOnly: environment.production,
        connectInZone: true,
      }),
      EffectsModule.forRoot([]),
      EffectsModule.forFeature([AuditLogEffects]),
      ToastrModule.forRoot(),
      BsDatepickerModule.forRoot(),
      TimepickerModule.forRoot(),
      ModalModule.forRoot()
    ),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    authInterceptorProvider,
    loggingInterceptorProvider,
    provideAppInitializer(() => AuthInitializer(inject(AuthService))()),
    ProductDetailGuard,
    AddProductGuard,
    EditProductGuard,
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
  ],
}).catch((err) => console.error(err));
