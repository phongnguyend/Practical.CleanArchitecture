import { Actions, createEffect, ofType } from "@ngrx/effects";
import { catchError, map, mergeMap } from "rxjs/operators";
import { AuditLogService } from "./audit-log.service";
import * as actions from "./audit-log.actions";
import { of } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable()
export class AuditLogEffects {
  constructor(
    private actions$: Actions,
    private auditLogService: AuditLogService
  ) { }

  getAuditLogs = createEffect(() => {
    return this.actions$.pipe(
      ofType(actions.fetchAuditLogs),
      mergeMap((params) =>
        this.auditLogService.getAuditLogs(params.page, params.pageSize).pipe(
          map((auditLogs) => actions.fetchAuditLogsSuccess(auditLogs)),
          catchError((error) => of(actions.fetchAuditLogsFail({ error })))
        )
      )
    );
  });
}
