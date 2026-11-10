package com.classifiedads.mobile.uitests.features.navigation;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import androidx.test.uiautomator.By;
import androidx.test.uiautomator.Direction;
import androidx.test.uiautomator.UiObject2;
import java.util.Comparator;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class NavigationUiTests extends MobileUiTestSupport {
    protected NavigationUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    // Audit logs and Notifications are beyond the initially visible navigation tabs.
    private void navigate(String title, Direction direction) {
        for (int attempt = 0; attempt < 12 && find(title) == null; attempt++) {
            UiObject2 tabs = device.findObjects(By.scrollable(true).pkg(packageName)).stream()
                .filter(node -> node.getVisibleBounds().width() > node.getVisibleBounds().height() * 2)
                .min(Comparator.comparingInt(node -> node.getVisibleBounds().top)).orElse(null);
            assertNotNull("Horizontal navigation for " + title, tabs);
            tabs.scroll(direction, .7f);
        }
        awaitLabel(title);
        click(title);
    }

    private void assertNoApiError(String title) {
        assertFalse("API error on " + title, contains("Request failed") || contains("Session expired") || contains("Network error"));
    }

    @Test public void auditLogsLoadRefreshAndReturnHome() {
        navigate("Audit logs", Direction.RIGHT);
        ready();
        assertNoApiError("Audit logs");
        click("Refresh");
        ready();
        assertNoApiError("Audit logs after refresh");
        scrollToText("Page 1");
        scrollToText("Next");
        UiObject2 content = device.findObjects(By.scrollable(true).pkg(packageName)).stream()
            .filter(node -> node.getVisibleBounds().height() > node.getVisibleBounds().width())
            .findFirst().orElse(null);
        if (content != null) content.scroll(Direction.DOWN, .5f);
        android.graphics.Rect previous = button("Previous").getVisibleBounds();
        android.graphics.Rect next = button("Next").getVisibleBounds();
        assertTrue("Pagination buttons have equal widths", Math.abs(previous.width() - next.width()) <= dp(2));
        assertTrue("Pagination buttons align vertically", Math.abs(previous.top - next.top) <= dp(2));
        assertTrue("Pagination buttons have a 12dp gap", next.left - previous.right >= dp(12) - 2);
        assertTrue("Pagination touch targets are at least 48dp", previous.height() >= dp(48) - 2 && next.height() >= dp(48) - 2);
        assertFalse("Previous is disabled on page one", button("Previous").isEnabled());
        if (button("Next").isEnabled()) {
            click("Next");
            scrollToText("Page 2");
            waitFor("Previous page is available", () -> button("Previous").isEnabled());
            capture("audit-pagination-page-two");
            click("Previous");
            scrollToText("Page 1");
        }
        capture("audit-logs-page");
        navigate("Home", Direction.LEFT);
        awaitLabel("Practical.CleanArchitecture");
    }

    @Test public void notificationsConnectAndReturnHome() {
        navigate("Notifications", Direction.RIGHT);
        waitFor("Notification hub connects", () -> findPattern("Connected(?: to NotificationHub)?") != null);
        assertNull("Notifications must not leave the previous record list visible", find("Refresh"));
        capture("notifications-page");
        navigate("Home", Direction.LEFT);
        awaitLabel("Practical.CleanArchitecture");
    }

    @Test public void resourceListsLoadWithoutApiErrors() throws Exception {
        for (String title : new String[]{"Products", "Files", "Users", "Roles", "Configuration Entries"}) {
            home();
            click(title);
            ready();
            if (title.equals("Files")) {
                awaitLabel("Upload file");
                capture("files-upload-action");
            }
            scrollToText("Page 1");
            assertNoApiError(title);
        }
    }
}
