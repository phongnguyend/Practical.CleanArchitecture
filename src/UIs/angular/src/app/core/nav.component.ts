import { Component, OnInit, ChangeDetectionStrategy } from "@angular/core";

import { RouterModule } from "@angular/router";
import { AuthService } from "../auth/auth.service";
import { AppendVersionPipe } from "../shared/append-version.pipe";
import { ActionIconComponent } from "../shared/action-icon.component";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterModule, AppendVersionPipe, ActionIconComponent],
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
