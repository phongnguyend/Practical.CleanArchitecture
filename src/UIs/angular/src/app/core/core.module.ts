import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { ToastrModule } from "ngx-toastr";

import { NavComponent } from "./nav.component";
import { NotificationComponent } from "./notification.component";
import { WelcomeComponent } from "./welcome.component";
import { SharedModule } from "../shared/shared.module";

@NgModule({
  declarations: [NavComponent, NotificationComponent, WelcomeComponent],
  imports: [CommonModule, RouterModule, SharedModule, ToastrModule.forRoot()],
  exports: [NavComponent, NotificationComponent],
})
export class CoreModule {}
