import { BrowserModule } from "@angular/platform-browser";
import { NgModule, ErrorHandler } from "@angular/core";
import { StoreModule } from "@ngrx/store";
import { StoreDevtoolsModule } from "@ngrx/store-devtools";
import { EffectsModule } from "@ngrx/effects";

import { environment } from "src/environments/environment";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { ProductModule } from "./products/product.module";
import { AuthModule } from "./auth/auth.module";
import { LoggingModule } from "./logging/logging.module";
import { GlobalErrorHandler } from "./shared/global-error-handler";
import { SharedModule } from "./shared/shared.module";
import { CoreModule } from "./core/core.module";
import { AuditLogModule } from "./auditlogs/audit-log.module";
import { FileModule } from "./files/file.module";
import { UserModule } from "./users/user.module";
import { SettingModule } from "./settings/setting.module";

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    SharedModule,
    CoreModule,
    SettingModule,
    AuditLogModule,
    ProductModule,
    FileModule,
    UserModule,
    AppRoutingModule,
    AuthModule,
    LoggingModule,
    StoreModule.forRoot({}),
    StoreDevtoolsModule.instrument({
      name: "Practical.CleanArchitecture App DevTools",
      maxAge: 25,
      logOnly: environment.production,
    connectInZone: true}),
    EffectsModule.forRoot([]),
  ],
  providers: [
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
