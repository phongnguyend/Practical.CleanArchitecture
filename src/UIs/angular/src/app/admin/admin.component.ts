import { Component } from "@angular/core";
import { ActivatedRoute, RouterModule } from "@angular/router";
import { ActionIconComponent } from "../shared/action-icon.component";
import { NotificationPlaygroundsComponent } from "./notification-playgrounds.component";

@Component({
  selector: "app-admin",
  standalone: true,
  imports: [RouterModule, ActionIconComponent, NotificationPlaygroundsComponent],
  templateUrl: "./admin.component.html",
})
export class AdminComponent {
  readonly view: string;
  constructor(route: ActivatedRoute) {
    this.view = route.snapshot.data["view"] || "admin";
  }
}
