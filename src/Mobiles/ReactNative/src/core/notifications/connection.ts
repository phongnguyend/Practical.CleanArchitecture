import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { accessToken } from "../auth/session";
import { config } from "../config/environment";

export function notifications(
  onMessage: (message: string) => void,
  onStatus: (status: string) => void = () => {},
) {
  const connection = new HubConnectionBuilder()
    .withUrl(config.hub, { accessTokenFactory: accessToken })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Error)
    .build();
  connection.on("ReceiveMessage", (value) =>
    onMessage(typeof value === "string" ? value : JSON.stringify(value)),
  );
  let stopped = false;
  let timer: ReturnType<typeof setTimeout> | undefined;
  const start = () => {
    void connection
      .start()
      .then(() => {
        if (!stopped) onStatus("Connected to NotificationHub");
      })
      .catch(() => {
        if (!stopped) {
          onStatus("Cannot connect to NotificationHub. Retrying…");
          timer = setTimeout(start, 10000);
        }
      });
  };
  connection.onclose(() => {
    if (!stopped) timer = setTimeout(start, 10000);
  });
  connection.onreconnecting(() => {
    if (!stopped) onStatus("Reconnecting…");
  });
  connection.onreconnected(() => {
    if (!stopped) onStatus("Connected to NotificationHub");
  });
  start();
  return () => {
    stopped = true;
    clearTimeout(timer);
    void connection.stop();
  };
}
