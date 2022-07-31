import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ModalModule } from "ngx-bootstrap/modal";

import { SharedModule } from "../shared/shared.module";
import { ConfigurationEntryListComponent } from "./configuration-entry-list.component";

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: "settings", component: ConfigurationEntryListComponent },
    ]),
    ModalModule.forRoot(),
    SharedModule,
  ],
  declarations: [ConfigurationEntryListComponent],
})
export class SettingModule {}
