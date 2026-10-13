import { Component, OnInit, OnDestroy, ChangeDetectionStrategy } from "@angular/core";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { ToastService, ToastContainerComponent } from "../shared/toast/toast.component";

import { AuthService } from "../auth/auth.service";
import { environment } from "../../environments/environment";

@Component({
  selector: "app-notification",
  template: "<app-toast-container />",
  imports: [ToastContainerComponent],
  changeDetection: ChangeDetectionStrategy.Eager,
  standalone: true,
})
export class NotificationComponent implements OnInit, OnDestroy {
  private cleanup?: () => void;
  ngOnDestroy() {
    this.cleanup?.();
  }
  constructor(
    public auth: AuthService,
    private toastr: ToastService
  ) {}

  ngOnInit(): void {
    const connection = new HubConnectionBuilder()
      .withUrl(environment.ResourceServer.NotificationEndpoint, {
        accessTokenFactory: () => this.auth.getAccessToken(),
      })
      .withAutomaticReconnect()
      .build();

    const vm = this;
    let disposed = false;
    this.cleanup = () => {
      disposed = true;
      void connection.stop();
    };

    connection.start().then(
      function () {
        if (disposed) return;
        console.log("Connected to NotificationHub");
        vm.toastr.success("Connected to NotificationHub");
      },
      function () {
        if (disposed) return;
        console.log(
          "Cannot connect to NotificationHub: " + environment.ResourceServer.NotificationEndpoint
        );
        vm.toastr.error(
          "Cannot connect to NotificationHub: " + environment.ResourceServer.NotificationEndpoint,
          { title: "Connection failed" }
        );
      }
    );

    connection.on("ReceiveMessage", (message) => {
      console.log("Received Message from NotificationHub: " + message);
      vm.toastr.info(message, { title: "Received Message from NotificationHub" });
    });
  }
}
