import React from "react";
import { expect, jest, test } from "@jest/globals";
import { fireEvent, render, screen, waitFor } from "@testing-library/react-native";
import AdminScreen from "../src/features/admin/AdminScreen";
import { api } from "../src/core/network/api";
import NotificationPlaygrounds from "../src/features/admin/NotificationPlaygrounds";

jest.mock("../src/core/network/api", () => ({ api: jest.fn() }));

test.each(["email", "sms"] as const)("%s playground validates fields and queues trimmed data", async (channel) => {
  jest.mocked(api).mockClear();
  jest.mocked(api).mockImplementation(async (_path, method) => method === "POST" ? { id: "queued-id" } : { provider: "Fake", items: [] });
  await render(<NotificationPlaygrounds channel={channel} />);
  await waitFor(() => expect(screen.getByRole("button", { name: "Queue message" })).toBeEnabled());
  await fireEvent.press(screen.getByRole("button", { name: "Queue message" }));
  expect(screen.getByText("Complete all required fields.")).toBeTruthy();
  if (channel === "email") {
    await fireEvent.changeText(screen.getByLabelText("From"), " from@example.com ");
    await fireEvent.changeText(screen.getByLabelText("Subject"), " Subject ");
  }
  await fireEvent.changeText(screen.getByLabelText(channel === "email" ? "To" : "Phone number"), "invalid");
  await fireEvent.changeText(screen.getByLabelText("Message"), " Message ");
  await fireEvent.press(screen.getByRole("button", { name: "Queue message" }));
  expect(api).not.toHaveBeenCalledWith(`notifications/playgrounds/${channel}`, "POST", expect.anything());
  await fireEvent.changeText(screen.getByLabelText(channel === "email" ? "To" : "Phone number"), channel === "email" ? " to@example.com " : " +15551234567 ");
  await fireEvent.press(screen.getByRole("button", { name: "Queue message" }));
  await waitFor(() => expect(api).toHaveBeenCalledWith(`notifications/playgrounds/${channel}`, "POST", expect.objectContaining({ body: "Message", recipient: channel === "email" ? "to@example.com" : "+15551234567" })));
  expect(screen.getByText("Message queued successfully.")).toBeTruthy();
});

test("playground validates fields and queues trimmed content for the current account", async () => {
  jest.mocked(api).mockImplementation(async (_path, method) => method === "POST" ? { id: "queued-id" } : { provider: "Fake", deviceCount: 0, items: [] });
  await render(<AdminScreen />);
  await fireEvent.press(screen.getByRole("tab", { name: "Playgrounds" }));
  await fireEvent.press(screen.getByRole("tab", { name: "Mobile Push" }));
  await waitFor(() => expect(screen.getByRole("button", { name: "Queue message" })).toBeEnabled());
  await fireEvent.press(screen.getByRole("button", { name: "Queue message" }));
  expect(screen.getByText("Title and message are required.")).toBeTruthy();
  expect(api).not.toHaveBeenCalledWith("notifications/playground", "POST", expect.anything());
  await fireEvent.changeText(screen.getByLabelText("Title"), " Update ");
  await fireEvent.changeText(screen.getByLabelText("Message"), " Details ");
  await fireEvent.press(screen.getByRole("button", { name: "Queue message" }));
  await waitFor(() => expect(api).toHaveBeenCalledWith("notifications/playground", "POST", {
    title: "Update", body: "Details", section: "notifications",
  }));
  expect(screen.getByText("Message queued successfully.")).toBeTruthy();
});


test("nested tabs remain available and identify the selected channel", async () => {
  jest.mocked(api).mockImplementation(async () => ({ provider: "Fake", items: [] }));
  await render(<AdminScreen />);
  await fireEvent.press(screen.getByRole("tab", { name: "Playgrounds" }));
  for (const label of ["Mobile Push", "Email", "SMS"]) {
    await fireEvent.press(screen.getByRole("tab", { name: label }));
    await waitFor(() => expect(screen.getByRole("button", { name: "Queue message" })).toBeEnabled());
    expect(screen.getByRole("tab", { name: "Playgrounds", selected: true })).toBeTruthy();
    expect(screen.getByRole("tab", { name: label, selected: true })).toBeTruthy();
    expect(screen.getAllByRole("tab")).toHaveLength(4);
  }
});
