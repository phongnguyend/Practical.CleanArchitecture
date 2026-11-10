package com.classifiedads.mobile.uitests.support;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.os.SystemClock;
import android.view.KeyEvent;
import androidx.test.platform.app.InstrumentationRegistry;
import androidx.test.uiautomator.By;
import androidx.test.uiautomator.UiDevice;
import androidx.test.uiautomator.UiObject2;
import androidx.test.uiautomator.Direction;
import androidx.test.uiautomator.Configurator;
import org.junit.Before;
import org.junit.Rule;
import org.junit.rules.TestWatcher;
import org.junit.runner.Description;
import java.io.File;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.function.BooleanSupplier;
import java.util.regex.Pattern;
import static org.junit.Assert.*;

/** Runs outside the target app: no production test flags, credentials, or mocked API. */
public abstract class MobileUiTestSupport {
    protected final String app, packageName, activity;
    protected final long timeout = 60_000;
    protected UiDevice device;

    protected MobileUiTestSupport(String app, String packageName, String activity) {
        this.app = app;
        this.packageName = packageName;
        this.activity = activity;
    }

    @Rule public TestWatcher evidence = new TestWatcher() {
        @Override protected void failed(Throwable error, Description test) { capture(test.getMethodName() + "-failed"); }
        @Override protected void succeeded(Description test) { capture(test.getMethodName() + "-passed"); }
    };

    @Before public void openApp() throws Exception {
        // Live clocks prevent global idleness; scenario conditions provide the real waits.
        Configurator.getInstance().setWaitForIdleTimeout(500);
        device = UiDevice.getInstance(InstrumentationRegistry.getInstrumentation());
        home();
    }

    protected void home() throws Exception {
        // Leaving a feature disposes its local form/search state in all three apps.
        // Keep cold starts explicit in persistence tests instead of reloading Metro per case.
        if (packageName.equals(device.getCurrentPackageName()) && find("Home") != null
            && find("Save") == null && find("Confirm") == null && find("Back") == null) {
            click("Home");
            waitFor("Home navigation completes", () -> find("Practical.CleanArchitecture") != null);
        } else restart();
    }

    protected void restart() throws Exception {
        device.executeShellCommand("am force-stop " + packageName);
        waitFor("Previous app process stops", () -> {
            try { return device.executeShellCommand("pidof " + packageName).trim().isEmpty(); }
            catch (java.io.IOException error) { throw new AssertionError("Unable to inspect app process", error); }
        });
        String result = device.executeShellCommand("am start -W -n " + packageName + "/" + activity);
        assertFalse("Install " + app + " first: " + result, result.contains("Error"));
        waitFor("Signed-in home in " + app + ". Sign in once before running these tests; start Metro for React Native.",
            () -> find("Practical.CleanArchitecture") != null, app.equals("reactnative") ? 120_000 : timeout);
    }

    protected void requiredField(String section, String message) {
        click(section);
        ready();
        click("Add new");
        awaitLabel("Save");
        click("Save");
        waitFor("Validation completes", () -> find("Save") != null && button("Save").isEnabled());
        scrollToText(message);
        assertNotNull("Invalid form remains open", find("Save"));
        click("Cancel");
        waitFor("Invalid editor can be cancelled", () -> find("Save") == null);
        ready();
    }

    protected void products() { click("Products"); ready(); }
    protected String fieldText(int index) {
        String value = fields().get(index).getText();
        return value == null ? "" : value;
    }
    protected List<UiObject2> fields() {
        waitFor("Editable fields", () -> !device.findObjects(By.clazz("android.widget.EditText").pkg(packageName)).isEmpty());
        UiObject2 scope = find("Save");
        List<UiObject2> result = new ArrayList<>();
        while (scope != null) {
            result = scope.findObjects(By.clazz("android.widget.EditText").pkg(packageName));
            if (!result.isEmpty()) break;
            scope = scope.getParent();
        }
        if (scope == null) result = device.findObjects(By.clazz("android.widget.EditText").pkg(packageName));
        result.sort(Comparator.comparingInt(x -> x.getVisibleBounds().top));
        return result;
    }
    protected void ready() {
        waitFor("List request completes", () -> {
            UiObject2 refresh = find("Refresh");
            return refresh != null && actionable(refresh).isEnabled()
                && !device.hasObject(By.clazz("android.widget.ProgressBar"));
        });
    }
    protected void scrollToText(String text) {
        for (int attempt = 0; attempt < 30; attempt++) {
            if (contains(text)) return;
            UiObject2 dialogControl = find("Save");
            UiObject2 root = dialogControl;
            if (root != null) while (root.getParent() != null) root = root.getParent();
            List<UiObject2> candidates = root == null ? device.findObjects(By.scrollable(true))
                : root.findObjects(By.scrollable(true));
            UiObject2 scroller = candidates.stream()
                .max(Comparator.comparingInt(n -> n.getVisibleBounds().height())).orElse(null);
            assertNotNull("Scrollable content for " + text, scroller);
            // Compose can return false before its updated scroll semantics arrive.
            // Re-query the target after each gesture instead of treating that as the end.
            scroller.scroll(Direction.DOWN, .8f);
            SystemClock.sleep(200);
        }
        waitFor("Visible content after scrolling: " + text, () -> contains(text));
    }
    protected void enterText(int index, String value) {
        UiObject2 field = fields().get(index);
        field.click();
        field.setText(value);
        if (app.equals("flutter")) {
            // Commit through the input connection as well as the accessibility value.
            device.pressKeyCode(KeyEvent.KEYCODE_SPACE);
            device.pressKeyCode(KeyEvent.KEYCODE_DEL);
        }
        device.waitForIdle();
    }
    protected void theme(String value) {
        UiObject2 theme = findPattern("(?i)Theme.*");
        if (theme == null) theme = find("Show menu");
        assertNotNull("Appearance menu must have an accessible label", theme);
        actionable(theme).click();
        waitFor("Appearance option " + value, () -> findPattern("(?i)" + value) != null);
        actionable(findPattern("(?i)" + value)).click();
        waitFor("Appearance menu closes", () -> findPattern("(?i)" + value) == null);
    }
    protected void assertBackground(boolean dark) {
        waitFor("Expected " + (dark ? "dark" : "light") + " background", () -> {
            File file = new File(directory(), "theme-probe.png");
            if (!device.takeScreenshot(file)) return false;
            Bitmap bitmap = BitmapFactory.decodeFile(file.getAbsolutePath());
            if (bitmap == null) return false;
            int pixel = bitmap.getPixel(dp(2), bitmap.getHeight() / 2);
            bitmap.recycle();
            float brightness = (Color.red(pixel) + Color.green(pixel) + Color.blue(pixel)) / 765f;
            return dark ? brightness < .25 : brightness > .7;
        });
    }
    protected UiObject2 actionable(UiObject2 node) {
        UiObject2 original = node;
        while (node != null) {
            if (node.isClickable()) return node;
            node = node.getParent();
        }
        return original;
    }
    protected UiObject2 button(String label) { awaitLabel(label); return actionable(find(label)); }
    protected void click(String label) { button(label).click(); }
    protected void awaitLabel(String label) { waitFor("Visible control: " + label, () -> find(label) != null); }
    protected UiObject2 find(String text) { return findPattern(Pattern.quote(text)); }
    protected UiObject2 findPattern(String expression) {
        Pattern pattern = Pattern.compile(expression);
        List<UiObject2> nodes = new ArrayList<>(device.findObjects(By.text(pattern).pkg(packageName)));
        nodes.addAll(device.findObjects(By.desc(pattern).pkg(packageName)));
        // Home buttons follow navigation tabs with the same label.
        return nodes.stream().filter(n -> !n.getVisibleBounds().isEmpty())
            .max(Comparator.comparingInt(n -> n.getVisibleBounds().bottom)).orElse(null);
    }
    protected boolean contains(String text) {
        return device.hasObject(By.textContains(text).pkg(packageName)) || device.hasObject(By.descContains(text).pkg(packageName));
    }
    protected int dp(int value) {
        return Math.round(value * InstrumentationRegistry.getInstrumentation().getTargetContext().getResources().getDisplayMetrics().density);
    }
    protected void waitFor(String message, BooleanSupplier condition) {
        waitFor(message, condition, timeout);
    }
    private void waitFor(String message, BooleanSupplier condition, long timeoutMillis) {
        long end = SystemClock.uptimeMillis() + timeoutMillis;
        do {
            try { if (condition.getAsBoolean()) return; }
            catch (androidx.test.uiautomator.StaleObjectException ignored) { /* Re-query after recomposition. */ }
            SystemClock.sleep(200);
        } while (SystemClock.uptimeMillis() < end);
        fail(app + ": " + message);
    }
    protected File directory() {
        File root = InstrumentationRegistry.getInstrumentation().getTargetContext().getExternalFilesDir("ui-results");
        File result = new File(root, app);
        result.mkdirs();
        return result;
    }
    protected void capture(String name) {
        if (device == null) return;
        try {
            device.takeScreenshot(new File(directory(), name + ".png"));
            device.dumpWindowHierarchy(new File(directory(), name + ".xml"));
            // Gradle removes the test APK after a run, including its external files.
            String runId = InstrumentationRegistry.getArguments().getString("runId", "manual");
            if (!runId.matches("[A-Za-z0-9-]+")) throw new IllegalArgumentException("Invalid runId");
            String destination = "/data/local/tmp/classifiedads-ui-results/" + runId + "/" + app;
            device.executeShellCommand("mkdir -p " + destination);
            device.executeShellCommand("cp " + directory().getAbsolutePath() + "/" + name + ".png " + destination + "/");
            device.executeShellCommand("cp " + directory().getAbsolutePath() + "/" + name + ".xml " + destination + "/");
        } catch (Exception ignored) { /* Do not replace the actual test failure with an artifact error. */ }
    }
}
