import { Component, OnInit } from "@angular/core";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { ToastrService } from "ngx-toastr";

import { AuthService } from "../auth/auth.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-notification",
  template: "",
})
export class NotificationComponent implements OnInit {
  constructor(public auth: AuthService, private toastr: ToastrService) {}

  ngOnInit(): void {
    const connection = new HubConnectionBuilder()
      .withUrl(environment.ResourceServer.NotificationEndpoint, {
        accessTokenFactory: () => this.auth.getAccessToken(),
      })
      .withAutomaticReconnect()
      .build();

    let vm = this;

    connection.start().then(
      function () {
        console.log("Connected to NotificationHub");
        vm.toastr.success("Connected to NotificationHub", "", {
          progressBar: true,
        });
      },
      function () {
        console.log(
          "Cannot connect to NotificationHub: " +
            environment.ResourceServer.NotificationEndpoint
        );
        vm.toastr.error(
          "Cannot connect to NotificationHub: " +
            environment.ResourceServer.NotificationEndpoint,
          "",
          { progressBar: true }
        );
      }
    );

    connection.on("ReceiveMessage", (message) => {
      console.log("Received Message from NotificationHub: " + message);
      vm.toastr.info(message, "Received Message from NotificationHub:", {
        progressBar: true,
      });
    });
  }
}
