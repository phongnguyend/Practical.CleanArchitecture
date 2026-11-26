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
  readonly channels = [
    { value: "mobilepush", path: "mobile-push", label: "Mobile Push" },
    { value: "email", path: "email", label: "Email" },
    { value: "sms", path: "sms", label: "SMS" },
  ];
  readonly view: string;
  constructor(route: ActivatedRoute) {
    this.view = route.snapshot.data["view"] || "admin";
  }
}
