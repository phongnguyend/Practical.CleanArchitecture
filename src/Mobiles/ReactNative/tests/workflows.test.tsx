import { beforeEach, expect, jest, test } from "@jest/globals";
import {
  fireEvent,
  render,
  screen,
  waitFor,
} from "@testing-library/react-native";
import { router } from "expo-router";
import { api, upload } from "../src/core/services";
import App from "../src/features/workspace/WorkspaceScreen";

let mockSection = "products";
jest.mock("expo-router", () => ({
  useLocalSearchParams: () => ({ section: mockSection }),
  router: { setParams: jest.fn() },
}));
jest.mock("react-native-safe-area-context", () => {
  const { View } = require("react-native");
  return { SafeAreaProvider: View, SafeAreaView: View };
});
jest.mock("@react-native-community/datetimepicker", () => "DateTimePicker");
jest.mock("expo-clipboard", () => ({
  setStringAsync: jest.fn(async () => {}),
}));
jest.mock("../src/core/services", () => ({
  readTheme: jest.fn(async () => "system"),
  saveTheme: jest.fn(async () => {}),
  restore: jest.fn(async () => true),
  login: jest.fn(),
  logout: jest.fn(),
  notifications: jest.fn(() => () => {}),
  api: jest.fn(),
  download: jest.fn(),
  upload: jest.fn(async () => []),
  pickFile: jest.fn(async () => ({
    name: "records.csv",
    uri: "file:///records.csv",
    size: 12,
  })),
}));
const product = {
  id: "p1",
  name: "Sample product",
  code: "abc",
  description: "Sample description",
  price: 12,
  starRating: 3,
};
const user = { id: "u1", userName: "tester", email: "test@example.com" };
const file = {
  id: "f1",
  name: "Report",
  fileName: "report.pdf",
  description: "Quarterly report",
  size: 1024,
  encrypted: true,
  uploadedTime: "2026-01-01T00:00:00Z",
};
beforeEach(() => {
  mockSection = "products";
  jest.mocked(api).mockImplementation(async (path, method) => {
    if (method && method !== "GET") return null;
    if (path.endsWith("/auditlogs"))
      return [
        {
          id: "a1",
          createdDateTime: "2026-01-01T00:00:00Z",
          userName: "admin",
          action: "Updated",
          data: { name: "Changed product" },
          highLight: { name: true },
        },
      ];
    if (path === "products") return [product];
    if (path === "users") return [user];
    if (path === "files") return [file];
    if (path === "roles")
      return [{ id: "r1", name: "Reader", normalizedName: "READER" }];
    if (path === "configurationentries")
      return [
        {
          id: "c1",
          key: "Secret setting",
          value: "hidden-secret",
          description: "Example",
          isSensitive: true,
        },
      ];
    if (path.startsWith("configurationentries/"))
      return {
        id: "c1",
        key: "Secret setting",
        value: "hidden-secret",
        description: "Example",
        isSensitive: true,
      };
    if (path.startsWith("files/")) return file;
    return path.startsWith("users/") ? user : product;
  });
});

test("Role form validates before posting its own payload", async () => {
  mockSection = "roles";
  await render(<App />);
  await screen.findByText("Reader");
  await fireEvent.press(screen.getByText("Add new"));
  await fireEvent.press(screen.getByText("Save"));
  await screen.findAllByText("Name is required.");
  expect(
    jest.mocked(api).mock.calls.some(([, method]) => method === "POST"),
  ).toBe(false);
  await fireEvent.changeText(screen.getByLabelText("Name"), "Editor");
  await fireEvent.press(screen.getByText("Save"));
  await waitFor(() =>
    expect(api).toHaveBeenCalledWith("roles", "POST", { name: "Editor" }),
  );
});

test("Configuration details hide secrets and edits preserve exact values", async () => {
  mockSection = "configurationentries";
  await render(<App />);
  await screen.findByText("Secret setting");
  expect(screen.queryByText("hidden-secret")).toBeNull();
  await fireEvent.press(screen.getByText("Edit"));
  const input = await screen.findByLabelText("Value");
  expect(input.props.secureTextEntry).toBe(true);
  await fireEvent.changeText(input, "  exact value  ");
  await fireEvent.press(screen.getByText("Save"));
  await waitFor(() =>
    expect(api).toHaveBeenCalledWith(
      "configurationentries/c1",
      "PUT",
      expect.objectContaining({ value: "  exact value  ", isSensitive: true }),
    ),
  );
});

test("Discard confirmation protects edited product data", async () => {
  await render(<App />);
  await screen.findByText("Sample product");
  await fireEvent.press(screen.getByText("Add new"));
  await fireEvent.changeText(screen.getByLabelText("Name"), "Unsaved product");
  await fireEvent.press(screen.getByText("Cancel"));
  await screen.findByText("Discard unsaved changes?");
  await fireEvent.press(screen.getAllByText("Cancel").at(-1)!);
  expect(screen.getByLabelText("Name").props.value).toBe("Unsaved product");
  await fireEvent.press(screen.getByText("Cancel"));
  await fireEvent.press(screen.getByText("Confirm"));
  expect(screen.queryByLabelText("Name")).toBeNull();
});
test("Changing sections loads the new resource and file edits preserve read-only fields", async () => {
  const app = await render(<App />);
  await screen.findByText("Sample product");
  mockSection = "files";
  await app.rerender(<App />);
  await screen.findByText("report.pdf");
  expect(screen.queryByText("Sample product")).toBeNull();
  await fireEvent.press(screen.getByText("Edit"));
  await waitFor(() =>
    expect(screen.getByLabelText("File name").props.editable).toBe(false),
  );
  expect(screen.getByLabelText("Encrypted").props.disabled).toBe(true);
});
test("Cancelling an unchanged editor returns directly to the list", async () => {
  await render(<App />);
  await screen.findByText("Sample product");
  await fireEvent.press(screen.getByText("Add new"));
  await fireEvent.press(screen.getByText("Cancel"));
  expect(screen.queryByText("Discard unsaved changes?")).toBeNull();
  expect(screen.queryByText("Save")).toBeNull();
});
test("Active navigation keeps records and details/history are readable", async () => {
  await render(<App />);
  await screen.findByText("Sample product");
  await fireEvent.press(screen.getAllByText("Products").at(-1)!);
  expect(router.setParams).not.toHaveBeenCalled();
  expect(screen.getByText("Sample product")).toBeTruthy();
  await fireEvent.press(screen.getByText("View"));
  await screen.findByText("Name");
  expect(screen.getAllByText("Code").length).toBeGreaterThan(0);
  await fireEvent.press(screen.getByText("Back"));
  await fireEvent.press(screen.getByText("History"));
  await screen.findByText("Name · Changed");
  expect(screen.getByText("Changed product")).toBeTruthy();
});
test("Password changes require confirmation and preserve exact characters", async () => {
  mockSection = "users";
  await render(<App />);
  await screen.findByText("tester");
  await fireEvent.press(screen.getByText("Set password"));
  await fireEvent.changeText(screen.getByLabelText("New password"), " secret ");
  await fireEvent.changeText(
    screen.getByLabelText("Confirm password"),
    "different",
  );
  await fireEvent.press(screen.getByText("Save"));
  await waitFor(() =>
    expect(
      screen.getAllByText("Passwords do not match.").length,
    ).toBeGreaterThan(0),
  );
  expect(
    jest.mocked(api).mock.calls.some(([, method]) => method === "PUT"),
  ).toBe(false);
  await fireEvent.changeText(
    screen.getByLabelText("Confirm password"),
    " secret ",
  );
  await fireEvent.press(screen.getByText("Save"));
  await waitFor(() =>
    expect(api).toHaveBeenCalledWith("users/u1/password", "PUT", {
      id: "u1",
      password: " secret ",
    }),
  );
});
test("Selecting an import file does not upload until confirmed", async () => {
  await render(<App />);
  await screen.findByText("Sample product");
  await fireEvent.press(screen.getByText("Import"));
  await fireEvent.press(screen.getByText("Choose file"));
  await screen.findByText("records.csv");
  expect(upload).not.toHaveBeenCalled();
  await fireEvent.press(screen.getAllByText("Import").at(-1)!);
  await waitFor(() =>
    expect(upload).toHaveBeenCalledWith(
      "products/importcsv",
      expect.objectContaining({ name: "records.csv" }),
    ),
  );
});
