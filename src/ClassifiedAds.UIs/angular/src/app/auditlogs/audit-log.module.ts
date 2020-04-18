import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ModalModule } from "ngx-bootstrap";

import { SharedModule } from "../shared/shared.module";
import { AuditLogListComponent } from "./audit-log-list.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: "auditlogs", component: AuditLogListComponent },
    ]),
    ModalModule.forRoot(),
    SharedModule,
  ],
  declarations: [AuditLogListComponent],
})
export class AuditLogModule {}
