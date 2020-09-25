import { Component, OnInit } from "@angular/core";
import { AuthService } from "../auth/auth.service";

@Component({
  selector: "app-nav",
  templateUrl: "./nav.component.html",
  styleUrls: ["./nav.component.css"]
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
