import { Component, VERSION } from "@angular/core";
import { Title } from "@angular/platform-browser";

@Component({
  templateUrl: "./welcome.component.html"
})
export class WelcomeComponent {
  public pageTitle = "Welcome ClassifiedAds Angular";
  public version = VERSION.full;

  constructor(private titleService: Title) {
    this.titleService.setTitle("ClassifiedAds Angular - Welcome");
  }
}
