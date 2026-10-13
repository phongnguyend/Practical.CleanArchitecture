import { useEffect } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import ToastContainer, { toast } from "../Toast/ToastContainer";

import { getAccessToken } from "../../containers/Auth/authService";
import env from "../..//environments";

function Notification() {
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(env.ResourceServer.NotificationEndpoint, {
        accessTokenFactory: () => getAccessToken(),
      })
      .withAutomaticReconnect()
      .build();

    let disposed = false;
    connection.start().then(
      function () {
        if (disposed) return;
        console.log("Connected to NotificationHub");
        toast.success("Connected to NotificationHub");
      },
      function () {
        if (disposed) return;
        console.log(
          "Cannot connect to NotificationHub: " + env.ResourceServer.NotificationEndpoint
        );
        toast.error(
          "Cannot connect to NotificationHub: " + env.ResourceServer.NotificationEndpoint
        );
      }
    );

    connection.on("ReceiveMessage", (message) => {
      console.log("Received Message from NotificationHub: " + message);
      toast.info("Received Message from NotificationHub: " + message);
    });
    return () => {
      disposed = true;
      void connection.stop();
    };
  }, []);

  return <ToastContainer />;
}

export default Notification;
