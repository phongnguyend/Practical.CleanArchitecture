import { test, expect } from "@playwright/test";

for (const channel of ["mobile-push", "email", "sms"] as const) {
  test(`${channel} playground validates, queues, and fits a narrow screen`, async ({ page }) => {
    await page.setViewportSize({ width: 320, height: 800 });
    await page.addInitScript(() => {
      localStorage.setItem("oidc.user:https://localhost:44367:ClassifiedAds.React", JSON.stringify({
        access_token: "local-ui-fixture", token_type: "Bearer", expires_at: Math.floor(Date.now() / 1000) + 3600,
        profile: { sub: "ui-fixture", preferred_username: "Test user" },
      }));
    });
    const push = channel === "mobile-push";
    const endpoint = push ? "/api/notifications/playground" : `/api/notifications/playgrounds/${channel}`;
    let payload: Record<string, string> | undefined;
    const items: Record<string, unknown>[] = [];
    await page.route("https://localhost:44312/**", async route => {
      const path = new URL(route.request().url()).pathname;
      if (path === endpoint) {
        if (route.request().method() === "POST") {
          payload = route.request().postDataJSON();
          items.push({ id: "queued", title: payload!.title || payload!.subject || "SMS", body: payload!.body, recipient: payload!.recipient, status: "Pending", attempts: 0, createdAt: "2026-01-01T12:00:00Z", pending: 0, completed: 0, failed: 0 });
          await route.fulfill({ status: 202, json: { id: "queued" } });
          return;
        }
        await route.fulfill({ json: { provider: "Fake", deviceCount: 0, items } });
        return;
      }
      await route.fulfill({ status: path.includes("/hubs/") ? 503 : 200, json: [] });
    });
    await page.goto("/admin");
    await page.getByRole("link", { name: "Playgrounds", exact: true }).click();
    await page.getByRole("link", { name: push ? "Mobile Push" : channel === "email" ? "Email" : "SMS", exact: true }).click();
    await expect(page.getByRole("navigation", { name: "Admin sections", exact: true }).getByRole("link", { name: "Playgrounds" })).toHaveClass(/active/);
    await expect(page.getByRole("navigation", { name: "Playground channels", exact: true }).locator("a.active")).toHaveCount(1);
    await expect(page.getByRole("button", { name: "Queue message" })).toBeEnabled();
    await page.getByRole("button", { name: "Queue message" }).click();
    await expect(page.getByRole("alert")).toHaveText(push ? "Title and message are required." : "Complete all required fields.");
    expect(payload).toBeUndefined();
    if (channel === "email") {
      await page.getByLabel("From", { exact: true }).fill(" sender@example.com ");
      await page.getByLabel("To", { exact: true }).fill(" recipient@example.com ");
      await page.getByLabel("Subject", { exact: true }).fill(" Subject ");
    } else if (channel === "sms") {
      await page.getByLabel("Phone number", { exact: true }).fill(" +15551234567 ");
    } else {
      await page.getByLabel("Title", { exact: true }).fill(" Title ");
    }
    await page.getByLabel("Message", { exact: true }).fill(" Message ");
    await page.getByRole("button", { name: "Queue message" }).click();
    await expect(page.getByText("Message queued. ID: queued", { exact: true })).toBeVisible();
    expect(payload?.body).toBe("Message");
    if (push) {
      expect(payload).toEqual({ title: "Title", body: "Message", section: "notifications" });
    } else {
      expect(payload?.recipient).toBe(channel === "email" ? "recipient@example.com" : "+15551234567");
    }
    await page.getByRole("button", { name: "Refresh queue" }).click();
    await expect(page.locator("article").filter({ hasText: "Message" })).toBeVisible();
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
    const buttons = await page.locator("form button").evaluateAll(nodes => nodes.map(node => {
      const bounds = node.getBoundingClientRect();
      return { left: bounds.left, right: bounds.right, height: bounds.height };
    }));
    for (const button of buttons) {
      expect(button.left).toBeGreaterThanOrEqual(0);
      expect(button.right).toBeLessThanOrEqual(320);
      expect(button.height).toBeGreaterThanOrEqual(48);
    }
  });
}
