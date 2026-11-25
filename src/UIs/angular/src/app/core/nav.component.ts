import { Component, OnInit, ChangeDetectionStrategy } from "@angular/core";

import { RouterModule } from "@angular/router";
import { AuthService } from "../auth/auth.service";
import { ActionIconComponent } from "../shared/action-icon.component";
import { ThemeSwitcherComponent } from "../shared/theme-switcher.component";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterModule, ActionIconComponent, ThemeSwitcherComponent],
})
export class NavComponent implements OnInit {
  pageTitle = "ClassifiedAds.Angular";

  constructor(public auth: AuthService) {}

  login() {
    this.auth.login("");
  }

  logout() {
    this.auth.logout();
  }

  ngOnInit(): void {}
}
