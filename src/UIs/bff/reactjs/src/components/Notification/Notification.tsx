import { useEffect } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import env from "../..//environments";

function Notification() {
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(env.ResourceServer.NotificationEndpoint, {})
      .withAutomaticReconnect()
      .build();

    connection.start().then(
      function () {
        console.log("Connected to NotificationHub");
        toast.success("Connected to NotificationHub");
      },
      function () {
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
  }, []);

  return <ToastContainer />;
}

export default Notification;
