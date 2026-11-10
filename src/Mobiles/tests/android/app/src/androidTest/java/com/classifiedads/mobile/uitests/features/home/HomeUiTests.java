package com.classifiedads.mobile.uitests.features.home;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import android.graphics.Rect;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class HomeUiTests extends MobileUiTestSupport {
    protected HomeUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    @Test public void homeActionsShareWidthHeightAndSpacing() {
        String[] titles = {"Products", "Files", "Users", "Roles", "Configuration Entries", "Sign out"};
        Rect previous = null;
        int width = 0;
        for (String title : titles) {
            Rect bounds = button(title).getVisibleBounds();
            assertTrue(title + " must be at least 44dp high", bounds.height() >= dp(44) - 2);
            assertTrue(title + " must fill the content width", bounds.width() > device.getDisplayWidth() * .8);
            if (previous != null) {
                assertEquals("Home button widths", width, bounds.width(), dp(2));
                assertTrue("Home buttons need a gap", bounds.top - previous.bottom >= dp(8));
            }
            width = bounds.width();
            previous = bounds;
        }
        Rect project = button("Practical.CleanArchitecture").getVisibleBounds();
        assertTrue("Project link must fill the welcome card", project.width() > width * .8);
        assertTrue("Project link touch target", project.height() >= dp(44) - 2);
    }

    @Test public void signOutCancellationPreservesSession() throws Exception {
        click("Sign out");
        awaitLabel("Confirm");
        assertTrue("Sign-out confirmation explains the action", contains("Sign out"));
        click("Cancel");
        waitFor("Sign-out dialog closes", () -> find("Confirm") == null);
        restart();
        products();
    }

    @Test public void themeAndSessionSurviveRestart() throws Exception {
        try {
            theme("dark");
            assertBackground(true);
            restart();
            assertBackground(true);
            theme("light");
            assertBackground(false);
            restart();
            assertBackground(false);
        } finally {
            // Leave the normal system preference, even if the assertion fails.
            restart();
            theme("system");
        }
    }
}
