<script>
import { HubConnectionBuilder } from "@microsoft/signalr";

import authService from "../auth/authService";
import env from "../../environments";

export default {
  mounted() {
    const connection = new HubConnectionBuilder()
      .withUrl(env.ResourceServer.NotificationEndpoint, {
        accessTokenFactory: () => authService.getAccessToken()
      })
      .withAutomaticReconnect()
      .build();

    let vm = this;

    connection.start().then(
      function() {
        // console.log("Connected to NotificationHub");
        vm.$toastr.s("Connected to NotificationHub");
      },
      function() {
        // console.log(
        //   "Cannot connect to NotificationHub: " +
        //     env.ResourceServer.NotificationEndpoint
        // );
        vm.$toastr.e(
          "Cannot connect to NotificationHub: " +
            env.ResourceServer.NotificationEndpoint
        );
      }
    );

    connection.on("ReceiveMessage", message => {
      // console.log("Received Message from NotificationHub: " + message);
      vm.$toastr.i("Received Message from NotificationHub: " + message);
    });
  }
};
</script>
