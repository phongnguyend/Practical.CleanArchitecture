package com.classifiedads.mobile.uitests.features.admin;

import androidx.test.uiautomator.By;
import androidx.test.uiautomator.Direction;
import androidx.test.uiautomator.UiObject2;
import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import java.util.Comparator;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class AdminUiTests extends MobileUiTestSupport {
    protected AdminUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    private void openPlaygrounds() {
        for (int attempt = 0; attempt < 12 && find("Admin") == null; attempt++) {
            UiObject2 tabs = device.findObjects(By.scrollable(true).pkg(packageName)).stream()
                .filter(node -> node.getVisibleBounds().width() > node.getVisibleBounds().height() * 2)
                .min(Comparator.comparingInt(node -> node.getVisibleBounds().top)).orElse(null);
            assertNotNull("Horizontal navigation", tabs);
            tabs.scroll(Direction.RIGHT, .7f);
        }
        click("Admin");
        click("Playgrounds");
    }

    @Test
    public void homeShortcutOpensNotificationPlaygrounds() {
        UiObject2 shortcut = homeAdminShortcut();
        for (int attempt = 0; attempt < 12 && shortcut == null; attempt++) {
            UiObject2 content = device.findObjects(By.scrollable(true).pkg(packageName)).stream()
                .filter(node -> node.getVisibleBounds().height() > dp(100))
                .max(Comparator.comparingInt(node -> node.getVisibleBounds().height())).orElse(null);
            assertNotNull("Scrollable Home shortcuts", content);
            content.scroll(Direction.DOWN, .5f);
            shortcut = homeAdminShortcut();
        }
        assertNotNull("Home must contain a full-width Admin shortcut", shortcut);
        shortcut.click();
        click("Playgrounds");
        awaitLabel("Mobile Push");
        awaitLabel("Email");
        awaitLabel("SMS");
        capture("home-admin-playgrounds");
    }

    private UiObject2 homeAdminShortcut() {
        UiObject2 label = find("Admin");
        if (label == null) {
            return null;
        }
        UiObject2 control = actionable(label);
        return control.getVisibleBounds().width() > device.getDisplayWidth() * .8 ? control : null;
    }

    @Test
    public void emailPlaygroundNavigationAndValidation() {
        messagePlayground("Email", "From");
    }

    @Test
    public void smsPlaygroundNavigationAndValidation() {
        messagePlayground("SMS", "Phone number");
    }

    private void messagePlayground(String channel, String firstField) {
        openPlaygrounds();
        click(channel);
        awaitLabel(firstField);
        scrollToText("Queue message");
        waitFor("Queue is available", () -> button("Queue message").isEnabled());
        click("Queue message");
        showValidation("Complete all required fields.");
        assertTrue("An empty message is rejected", contains("Complete all required fields."));
        scrollToText("Refresh queue");
        click("Refresh queue");
        waitFor("Queue refresh completes", () -> button("Refresh queue").isEnabled());
        scrollToText("Recent messages");
        assertTrue("Queue history is visible", contains("Recent messages"));
        capture(channel.toLowerCase() + "-playground");
    }

    private void showValidation(String text) {
        // Web forms place their alert above the fields; native forms place it below the actions.
        if (app.equals("capacitor")) {
            UiObject2 content = device.findObjects(By.scrollable(true).pkg(packageName)).stream()
                .max(Comparator.comparingInt(node -> node.getVisibleBounds().height())).orElse(null);
            if (content != null) {
                content.fling(Direction.UP);
            }
        }
        scrollToText(text);
    }

    @Test
    public void mobilePushPlaygroundNavigationAndValidation() {
        openPlaygrounds();
        click("Mobile Push");
        awaitLabel("Title");
        scrollToText("Queue message");
        waitFor("Queue message is ready", () -> button("Queue message").isEnabled());
        click("Queue message");
        showValidation("Title and message are required.");
        assertTrue("Empty messages cannot be queued", contains("Title and message are required."));
        scrollToText("Refresh queue");
        click("Refresh queue");
        waitFor("Queue refresh completes", () -> button("Refresh queue").isEnabled());
        scrollToText("Recent messages");
        assertTrue("Queue history is available", contains("Recent messages"));
        capture("mobile-push-playground");
    }
}
