import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";

import { StarComponent } from "./star.component";
import { AppendVersionPipe } from "./append-version.pipe";
import { AppendCurrentDateTimePipe } from "./append-current-datetime.pipe";

@NgModule({
  imports: [CommonModule],
  declarations: [StarComponent, AppendVersionPipe, AppendCurrentDateTimePipe],
  exports: [
    StarComponent,
    AppendVersionPipe,
    AppendCurrentDateTimePipe,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ]
})
export class SharedModule {}
