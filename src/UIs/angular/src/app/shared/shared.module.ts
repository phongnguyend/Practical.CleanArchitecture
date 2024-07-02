import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";

import { StarComponent } from "./star.component";
import { AppendVersionPipe } from "./append-version.pipe";
import { AppendCurrentDateTimePipe } from "./append-current-datetime.pipe";
import { TimerComponent } from "./timer.component";
import { PaginationComponent } from "./pagination.component";

@NgModule({
  imports: [CommonModule],
  declarations: [
    PaginationComponent,
    StarComponent,
    TimerComponent,
    AppendVersionPipe,
    AppendCurrentDateTimePipe,
  ],
  exports: [
    PaginationComponent,
    StarComponent,
    TimerComponent,
    AppendVersionPipe,
    AppendCurrentDateTimePipe,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  providers: [provideHttpClient(withInterceptorsFromDi())],
})
export class SharedModule {}
