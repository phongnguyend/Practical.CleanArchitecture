import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { NavComponent } from "./core/nav.component";
import { NotificationComponent } from "./core/notification.component";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"],
  standalone: true,
  imports: [RouterOutlet, NavComponent, NotificationComponent],
})
export class AppComponent {}
