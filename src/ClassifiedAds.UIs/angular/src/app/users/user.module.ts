import { NgModule } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { BsDatepickerModule } from "ngx-bootstrap/datepicker";
import { TimepickerModule } from "ngx-bootstrap/timepicker";

import { ListUsersComponent } from "./list-users/list-users.component";
import { AddEditUserComponent } from "./add-edit-user/add-edit-user.component";
import { RouterModule } from "@angular/router";
import { ModalModule } from "ngx-bootstrap";
import { SharedModule } from "../shared/shared.module";
import { ViewUserDetailsComponent } from "./view-user-details/view-user-details.component";

@NgModule({
  declarations: [
    ListUsersComponent,
    AddEditUserComponent,
    ViewUserDetailsComponent,
  ],
  imports: [
    RouterModule.forChild([
      { path: "users", component: ListUsersComponent },
      {
        path: "users/add",
        component: AddEditUserComponent,
      },
      {
        path: "users/edit/:id",
        component: AddEditUserComponent,
      },
      {
        path: "users/:id",
        component: ViewUserDetailsComponent,
      },
    ]),
    BrowserAnimationsModule,
    BsDatepickerModule.forRoot(),
    TimepickerModule.forRoot(),
    ModalModule.forRoot(),
    SharedModule,
  ],
})
export class UserModule {}
