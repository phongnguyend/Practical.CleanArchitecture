package com.classifiedads.mobile.uitests.features.roles;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import org.junit.Test;
import org.junit.Assume;
import androidx.test.platform.app.InstrumentationRegistry;
import androidx.test.uiautomator.By;
import androidx.test.uiautomator.UiObject2;
import java.util.UUID;
import java.util.regex.Pattern;
import static org.junit.Assert.*;

public abstract class RolesUiTests extends MobileUiTestSupport {
    protected RolesUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    @Test public void roleRequiresName() {
        requiredField("Roles", "Name is required");
    }

    private void openList(String section, String name) throws Exception {
        home();
        click(section);
        ready();
        enterText(0, name);
        dismissKeyboard();
        waitFor("Fixture list is filtered", () -> contains("No records found") || recordVisible(name));
    }

    private void dismissKeyboard() {
        if (device.hasObject(By.pkg("com.google.android.inputmethod.latin"))) {
            device.pressBack();
        }
    }

    private UiObject2 recordLabel(String name) {
        Pattern pattern = Pattern.compile("(?s)" + Pattern.quote(name) + "(?:@example\\.invalid)?(?:\\n.*)?");
        return java.util.stream.Stream.concat(
                device.findObjects(By.text(pattern).pkg(packageName)).stream(),
                device.findObjects(By.desc(pattern).pkg(packageName)).stream())
            .filter(node -> !node.getClassName().equals("android.widget.EditText"))
            .filter(node -> !node.getVisibleBounds().isEmpty())
            .max(java.util.Comparator.comparingInt(node -> node.getVisibleBounds().bottom))
            .orElse(null);
    }

    private boolean recordVisible(String name) {
        return recordLabel(name) != null;
    }

    private void recordAction(String name, String action) {
        scrollToText(action);
        UiObject2 scope = recordLabel(name);
        while (scope != null) {
            UiObject2 control = scope.findObject(By.text(action));
            if (control == null) {
                control = scope.findObject(By.desc(action));
            }
            if (control != null) {
                actionable(control).click();
                return;
            }
            scope = scope.getParent();
        }
        fail("No " + action + " action found for fixture " + name);
    }

    private UiObject2 candidate(String user) {
        return java.util.stream.Stream.concat(
                device.findObjects(By.textStartsWith(user)).stream(),
                device.findObjects(By.descStartsWith(user)).stream())
            .filter(node -> !node.getClassName().equals("android.widget.EditText"))
            .filter(node -> !node.getVisibleBounds().isEmpty())
            .max(java.util.Comparator.comparingInt(node -> node.getVisibleBounds().bottom))
            .orElse(null);
    }

    private void details(String name) {
        if (app.equals("capacitor")) {
            click(name);
        } else {
            recordAction(name, "View");
            awaitLabel(app.equals("maui") ? "Close" : "Back");
        }
    }

    private void removeFixture(String section, String name) throws Exception {
        restart();
        openList(section, name);
        if (contains("No records found")) {
            return;
        }
        assertTrue("Only remove the uniquely named test fixture", recordVisible(name));
        recordAction(name, "Delete");
        waitFor("Delete confirmation names the fixture", () -> (app.equals("capacitor") ? contains(name) : contains("Delete " + name + "? This cannot be undone.")) && (find("Confirm") != null || find("Yes") != null || (app.equals("capacitor") && contains("Delete " + section.substring(0, section.length() - 1)))));
        if (find("Confirm") != null) {
            click("Confirm");
        } else if (app.equals("capacitor") && section.equals("Roles")) {
            click("Delete");
        } else {
            click("Yes");
        }
        waitFor("Fixture removed", () -> contains("No records found"));
    }

    @Test public void roleMembershipCanBeAssignedViewedAndRemoved() throws Exception {
        Assume.assumeTrue("Enable with -IncludeCrud; creates a temporary user and role",
            "true".equals(InstrumentationRegistry.getArguments().getString("includeCrud")));
        String suffix = UUID.randomUUID().toString().substring(0, 8);
        String user = "UITest-member-" + suffix;
        String role = "UITest-role-" + suffix;
        boolean userMayExist = false;
        boolean roleMayExist = false;
        try {
            click("Users");
            ready();
            click("Add new");
            awaitLabel("Save");
            enterText(0, user);
            enterText(1, user + "@example.invalid");
            userMayExist = true;
            scrollToText("Save");
            click("Save");
            waitFor("User created", () -> find("Save") == null);
            home();
            click("Roles");
            ready();
            click("Add new");
            awaitLabel("Save");
            enterText(0, role);
            roleMayExist = true;
            click("Save");
            waitFor("Role created", () -> find("Save") == null);
            openList("Roles", role);
            details(role);
            scrollToText("Role users");
            scrollToText("Search users");
            waitFor("Membership is loaded", () -> find("Refresh users") != null && button("Refresh users").isEnabled());
            enterText(fields().size() - 1, user);
            dismissKeyboard();
            if (app.equals("reactnative") || app.equals("kotlin")) {
                scrollToText("Select " + user);
                click("Select " + user);
            } else {
                scrollToText("User to assign");
                if (app.equals("capacitor")) {
                    click("Select a user");
                } else {
                    click("User to assign");
                }
                waitFor("Candidate user is available", () -> candidate(user) != null);
                candidate(user).click();
            }
            scrollToText("Assign user");
            click("Assign user");
            scrollToText("Remove " + user + " from role");
            awaitLabel("Remove " + user + " from role");
            capture("role-user-assigned");
            restart();
            openList("Users", user);
            details(user);
            scrollToText("Assigned roles");
            scrollToText(role);
            assertTrue("User details show the assigned role", contains(role));
            capture("user-assigned-roles");
            restart();
            openList("Roles", role);
            details(role);
            scrollToText("Remove " + user + " from role");
            click("Remove " + user + " from role");
            awaitLabel("Cancel");
            click("Cancel");
            awaitLabel("Remove " + user + " from role");
            click("Remove " + user + " from role");
            if (app.equals("flutter") || app.equals("maui")) {
                click("Confirm");
            } else {
                click("Remove");
            }
            if (app.equals("maui")) {
                awaitLabel("No users assigned.");
            } else {
                scrollToText("No users assigned.");
            }
            restart();
            openList("Users", user);
            details(user);
            scrollToText("No roles assigned.");
            capture("user-role-removed");
        } catch (Exception | AssertionError failure) {
            android.util.Log.e("RoleMembership", "Membership scenario failed before cleanup", failure);
            capture("membership-failure-before-cleanup");
            throw failure;
        } finally {
            try {
                if (roleMayExist) {
                    removeFixture("Roles", role);
                }
            } finally {
                if (userMayExist) {
                    removeFixture("Users", user);
                }
            }
        }
    }
}
