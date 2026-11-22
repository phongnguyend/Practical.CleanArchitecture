import { test, expect } from "@playwright/test";
test.beforeEach(async ({ page }) => {
  await page.addInitScript(() => {
    localStorage.setItem(
      "oidc.user:https://localhost:44367:ClassifiedAds.React",
      JSON.stringify({
        access_token: "local-ui-fixture",
        token_type: "Bearer",
        expires_at: Math.floor(Date.now() / 1000) + 3600,
        profile: { sub: "ui-fixture", preferred_username: "Test user" },
      }),
    );
  });
  await page.route("https://localhost:44312/**", async (route) => {
    const url = new URL(route.request().url());
    const body = url.pathname.toLowerCase().includes("/products")
      ? Array.from({ length: 12 }, (_, i) => ({
          id: String(i),
          name: `Product ${i}`,
          code: `P${i}`,
          description: "Example",
          starRating: 3,
        }))
      : url.pathname.includes("/paged")
        ? { items: [], totalItems: 0 }
        : [];
    if (url.pathname.includes("/hubs/")) {
      await route.fulfill({
        status: 503,
        body: "Unavailable in isolated UI tests",
      });
      return;
    }
    await route.fulfill({ json: body });
  });
});
test("home, theme, sign-out cancellation and mobile width", async ({
  page,
}) => {
  await page.goto("/");
  await expect(
    page.getByRole("heading", { name: "Welcome to Classified Ads" }),
  ).toBeVisible();
  await page.getByRole("button", { name: "Sign out", exact: true }).click();
  await page.getByRole("button", { name: "Cancel", exact: true }).click();
  await expect(page.getByRole("dialog")).toHaveCount(0);
  expect(
    await page.evaluate(
      () => document.documentElement.scrollWidth <= innerWidth,
    ),
  ).toBe(true);
});
for (const [route, label] of [
  ["products", "Products"],
  ["files", "Files"],
  ["users", "Users"],
  ["roles", "Roles"],
  ["settings", "Settings"],
  ["auditlogs", "Audit logs"],
  ["notifications", "Notifications"],
]) {
  test(`navigate to ${label}`, async ({ page }) => {
    await page.goto("/home");
    await page
      .getByRole("navigation")
      .getByRole("link", { name: label, exact: true })
      .click();
    await expect(page).toHaveURL(new RegExp(`/${route}$`));
    await expect(page.locator("main")).toBeVisible();
    expect(
      await page.evaluate(
        () => document.documentElement.scrollWidth <= innerWidth,
      ),
    ).toBe(true);
  });
}
test("product search, pagination, validation and discard protection", async ({
  page,
}) => {
  await page.goto("/products");
  await expect(page.getByText("Page 1 \u00b7 12 records")).toBeVisible();
  await page.getByRole("button", { name: "Next", exact: true }).click();
  await expect(page.getByText("Page 2 \u00b7 12 records")).toBeVisible();
  await page.getByRole("textbox", { name: "Filter records" }).fill("missing");
  await expect(page.getByText("No records found")).toBeVisible();
  await page.getByRole("link", { name: "Add new", exact: true }).click();
  await page.getByRole("button", { name: "Save", exact: true }).click();
  await expect(page.getByText("Name is required")).toBeVisible();
  await page.getByLabel("Name", { exact: true }).fill("Unsaved product");
  await page.getByRole("button", { name: "Cancel", exact: true }).click();
  await expect(page.getByText("Discard unsaved changes?")).toBeVisible();
  await page
    .getByRole("dialog")
    .getByRole("button", { name: "Cancel", exact: true })
    .click();
  await expect(page.getByLabel("Name", { exact: true })).toHaveValue(
    "Unsaved product",
  );
  await page.getByRole("button", { name: "Cancel", exact: true }).click();
  await page.getByRole("button", { name: "Confirm", exact: true }).click();
  await expect(page).toHaveURL(/\/products$/);
});
for (const [route, validation] of [
  ["roles", "Name is required"],
  ["settings", "Key is required"],
]) {
  test(`${route} required validation`, async ({ page }) => {
    await page.goto(`/${route}`);
    await page.getByRole("button", { name: "Add new", exact: true }).click();
    await page.getByRole("button", { name: "Save", exact: true }).click();
    await expect(
      page.getByText(validation, { exact: true }).last(),
    ).toBeVisible();
    await page.getByRole("button", { name: "Cancel", exact: true }).click();
    await expect(page.getByRole("dialog")).toHaveCount(0);
  });
}

test("theme persists across reloads", async ({ page }) => {
  await page.goto("/home");
  await page.getByLabel(/Theme:.*Choose appearance/).click();
  await page.getByRole("button", { name: "Dark", exact: true }).click();
  await expect(page.locator("html")).toHaveAttribute("data-bs-theme", "dark");
  await page.reload();
  await expect(page.locator("html")).toHaveAttribute("data-bs-theme", "dark");
});
test("user validation does not submit an incomplete account", async ({
  page,
}) => {
  let submissions = 0;
  page.on("request", (request) => {
    if (request.method() === "POST" && request.url().includes("/api/users")) {
      submissions++;
    }
  });
  await page.goto("/users/add");
  await page.getByRole("button", { name: "Save", exact: true }).click();
  await expect(page.getByText("Username is required")).toBeVisible();
  expect(submissions).toBe(0);
  await page.getByRole("button", { name: "Cancel", exact: true }).click();
  await expect(page).toHaveURL(/\/users$/);
});

test("action groups have equal columns and at least 12px spacing", async ({
  page,
}) => {
  await page.goto("/products");
  const groups = page.locator(".action-group");
  await expect(groups.first()).toBeVisible();
  const layout = await groups.evaluateAll((elements) =>
    elements.map((element) => {
      const children = Array.from(element.children).map((child) =>
        child.getBoundingClientRect(),
      );
      return {
        gap: parseFloat(getComputedStyle(element).gap),
        widths: children.slice(0, 2).map((rect) => rect.width),
        overlaps: children.some((a, i) =>
          children.some(
            (b, j) =>
              i < j &&
              a.left < b.right &&
              a.right > b.left &&
              a.top < b.bottom &&
              a.bottom > b.top,
          ),
        ),
      };
    }),
  );
  for (const group of layout) {
    expect(group.gap).toBeGreaterThanOrEqual(12);
    expect(group.overlaps).toBe(false);
    if (group.widths.length === 2) {
      expect(Math.abs(group.widths[0] - group.widths[1])).toBeLessThan(1);
    }
  }
  await page.getByRole("textbox", { name: "Filter records" }).fill("Product 1");
  await page.getByRole("link", { name: "Add new", exact: true }).click();
  await page.getByRole("button", { name: "Cancel", exact: true }).click();
  await expect(
    page.getByRole("textbox", { name: "Filter records" }),
  ).toHaveValue("Product 1");
});

test("audit cards show readable details without horizontal scrolling", async ({ page }) => {
  await page.route("**/paged?*", async (route) => {
    await route.fulfill({ json: { items: [{
      id: "audit-example", action: "UPDATED", userName: "Test user",
      createdDateTime: "2026-11-22T10:00:00Z",
      log: JSON.stringify({ name: "Example record", description: "Long detail ".repeat(40), nested: { status: "Updated" } }),
    }, { id: "plain-example", action: "CREATED", userName: "Test user", createdDateTime: "2026-11-22T09:00:00Z", log: "Plain text event" }], totalItems: 2 } });
  });
  await page.goto("/auditlogs");
  const entry = page.getByRole("article", { name: "UPDATED by Test user" });
  await expect(entry).toBeVisible();
  await expect(entry.getByText("Example record", { exact: true })).toBeHidden();
  await entry.locator("summary").click();
  await expect(entry.getByText("Example record", { exact: true })).toBeVisible();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
  await entry.getByRole("button", { name: "View JSON" }).click();
  await expect(page.getByRole("dialog")).toBeVisible();
  await page.getByRole("button", { name: "Close", exact: true }).click();
  const plain = page.getByRole("article", { name: "CREATED by Test user" });
  await plain.locator("summary").click();
  await expect(plain.getByText("Plain text event")).toBeVisible();
});

test("product history labels changed fields in readable cards", async ({ page }) => {
  await page.route("**/products/*/auditLogs", async (route) => {
    await route.fulfill({ json: [{ id: "history", action: "UPDATED", userName: "Test user", createdDateTime: "2026-11-22T10:00:00Z", data: { code: "P0", name: "Product 0", description: "Updated description" }, highLight: { description: true } }] });
  });
  await page.goto("/products");
  await page.getByRole("button", { name: "History", exact: true }).first().click();
  const dialog = page.getByRole("dialog");
  await expect(dialog.getByText("UPDATED", { exact: true })).toBeVisible();
  await expect(dialog.getByText("Updated description", { exact: true })).toBeVisible();
  await expect(dialog.getByText("Changed", { exact: true })).toBeVisible();
  expect(await dialog.evaluate((element) => element.scrollWidth <= element.clientWidth)).toBe(true);
});
