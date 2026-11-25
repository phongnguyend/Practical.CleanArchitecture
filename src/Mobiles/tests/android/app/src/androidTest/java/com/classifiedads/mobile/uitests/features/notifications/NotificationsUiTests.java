package com.classifiedads.mobile.uitests.features.notifications;

import androidx.test.uiautomator.By;
import androidx.test.uiautomator.Direction;
import androidx.test.uiautomator.UiObject2;
import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import java.util.Comparator;
import org.junit.Assume;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class NotificationsUiTests extends MobileUiTestSupport {
    protected NotificationsUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    private void openNotifications() {
        for (int attempt = 0; attempt < 12 && find("Notifications") == null; attempt++) {
            UiObject2 tabs = device.findObjects(By.scrollable(true).pkg(packageName)).stream()
                .filter(node -> node.getVisibleBounds().width() > node.getVisibleBounds().height() * 2)
                .min(Comparator.comparingInt(node -> node.getVisibleBounds().top)).orElse(null);
            assertNotNull("Horizontal navigation", tabs);
            tabs.scroll(Direction.RIGHT, .7f);
        }
        click("Notifications");
        awaitLabel("Notification inbox");
    }

    @Test public void disabledPushExplainsConfigurationWithoutRequestingPermission() {
        openNotifications();
        Assume.assumeTrue("This case verifies credential-free builds", find("Push notifications are not configured for this build.") != null);
        scrollToText("Enable notifications");
        assertFalse("Enable stays disabled without Firebase configuration", button("Enable notifications").isEnabled());
        scrollToText("Disable notifications");
        assertFalse("Disable stays disabled without Firebase configuration", button("Disable notifications").isEnabled());
        assertEquals("No OS permission dialog was opened", packageName, device.getCurrentPackageName());
        capture("push-disabled");
    }

    @Test public void inboxRefreshRemainsAvailableWithoutLiveDelivery() {
        openNotifications();
        scrollToText("Refresh inbox");
        waitFor("Refresh inbox is ready", () -> button("Refresh inbox").isEnabled());
        click("Refresh inbox");
        ready();
        scrollToText("Page 1");
        assertFalse("Inbox loaded successfully", contains("Unable to load notifications") || contains("Notification request failed") || contains("Request failed"));
        scrollToText("Previous");
        assertFalse("Previous is disabled on page one", button("Previous").isEnabled());
        capture("push-inbox");
    }
}
