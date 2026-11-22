import { expect, jest, test } from "@jest/globals";
import { fireEvent, render, screen } from "@testing-library/react-native";
import AuditEntry from "../src/features/auditlogs/AuditEntry";

jest.mock("@react-native-community/datetimepicker", () => "DateTimePicker");
jest.mock("expo-clipboard", () => ({ setStringAsync: jest.fn(async () => {}) }));

test("event fields stay collapsed until requested and can be hidden again", async () => {
  await render(<AuditEntry entry={{ id: "1", action: "Updated", userName: "admin",
    createdDateTime: "2026-11-22T12:00:00Z", log: '{"displayName":"Updated name","missing":null}' }} />);
  expect(screen.getByText("By admin")).toBeTruthy();
  expect(screen.queryByText("Updated name")).toBeNull();
  await fireEvent.press(screen.getByText("View event details"));
  expect(screen.getByText("display Name")).toBeTruthy();
  expect(screen.getByText("Updated name")).toBeTruthy();
  expect(screen.getByText("Not provided")).toBeTruthy();
  expect(screen.getByText("Copy JSON")).toBeTruthy();
  await fireEvent.press(screen.getByText("Hide event details"));
  expect(screen.queryByText("Updated name")).toBeNull();
});

test("non-JSON events and missing metadata remain readable", async () => {
  await render(<AuditEntry entry={{ id: "2", createdDateTime: "unknown", log: "Plain message" }} />);
  expect(screen.getByText("Event")).toBeTruthy();
  expect(screen.getByText("By Unknown user")).toBeTruthy();
  await fireEvent.press(screen.getByText("View event details"));
  expect(screen.getByText("Plain message")).toBeTruthy();
});
