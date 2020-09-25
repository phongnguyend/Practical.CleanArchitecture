import { BrowserModule } from "@angular/platform-browser";
import { NgModule, ErrorHandler } from "@angular/core";

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

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    SharedModule,
    CoreModule,
    AuditLogModule,
    ProductModule,
    FileModule,
    UserModule,
    AppRoutingModule,
    AuthModule,
    LoggingModule,
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
