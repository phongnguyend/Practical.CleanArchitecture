import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { EffectsModule } from "@ngrx/effects";
import { StoreModule } from "@ngrx/store";
import { ModalModule } from "ngx-bootstrap/modal";
import { PaginationModule } from "ngx-bootstrap/pagination";

import { SharedModule } from "../shared/shared.module";
import { AuditLogListComponent } from "./audit-log-list.component";
import { AuditLogEffects } from "./audit-log.effects";
import { auditLogReducer } from "./audit-log.reducer";

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: "auditlogs", component: AuditLogListComponent },
    ]),
    ModalModule.forRoot(),
    PaginationModule,
    SharedModule,
    StoreModule.forFeature("auditLog", auditLogReducer),
    EffectsModule.forFeature([AuditLogEffects]),
  ],
  declarations: [AuditLogListComponent],
})
export class AuditLogModule { }
