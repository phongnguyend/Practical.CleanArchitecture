import { Component, VERSION } from "@angular/core";
import { Title } from "@angular/platform-browser";

import { AppendVersionPipe } from "../shared/append-version.pipe";
import { AppendCurrentDateTimePipe } from "../shared/append-current-datetime.pipe";
import { TimerComponent } from "../shared/timer.component";

@Component({
  templateUrl: "./welcome.component.html",
  standalone: true,
  imports: [AppendVersionPipe, AppendCurrentDateTimePipe, TimerComponent],
})
export class WelcomeComponent {
  public pageTitle = "Welcome ClassifiedAds Angular";
  public version = VERSION.full;

  constructor(private titleService: Title) {
    this.titleService.setTitle("ClassifiedAds Angular - Welcome");
  }
}
