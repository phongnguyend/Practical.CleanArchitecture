import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { AuthService } from "../auth/auth.service";
import { AppendVersionPipe } from "../shared/append-version.pipe";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"],
  standalone: true,
  imports: [CommonModule, RouterModule, AppendVersionPipe],
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
