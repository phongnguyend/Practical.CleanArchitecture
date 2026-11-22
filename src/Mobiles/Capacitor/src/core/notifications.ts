import { useSyncExternalStore } from "react";
export interface NotificationMessage {
  id: number;
  text: string;
  time: string;
}
let state: { status: string; messages: NotificationMessage[] } = {
  status: "Disconnected",
  messages: [],
};
const listeners = new Set<() => void>();
export function updateNotifications(status: string, message?: unknown) {
  const messages =
    message === undefined
      ? state.messages
      : [
          {
            id: Date.now(),
            text:
              typeof message === "string" ? message : JSON.stringify(message),
            time: new Date().toLocaleString(),
          },
          ...state.messages,
        ].slice(0, 100);
  state = { status, messages };
  listeners.forEach((listener) => listener());
}
export function useNotifications() {
  return useSyncExternalStore(
    (listener) => {
      listeners.add(listener);
      return () => {
        listeners.delete(listener);
      };
    },
    () => state,
  );
}
export function clearNotifications() {
  state = { ...state, messages: [] };
  listeners.forEach((listener) => listener());
}
