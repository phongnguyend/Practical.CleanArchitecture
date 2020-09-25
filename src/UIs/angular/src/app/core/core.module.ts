import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";

import { NavComponent } from "./nav.component";
import { WelcomeComponent } from "./welcome.component";
import { SharedModule } from "../shared/shared.module";

@NgModule({
  declarations: [NavComponent, WelcomeComponent],
  imports: [CommonModule, RouterModule, SharedModule],
  exports: [NavComponent]
})
export class CoreModule {}
