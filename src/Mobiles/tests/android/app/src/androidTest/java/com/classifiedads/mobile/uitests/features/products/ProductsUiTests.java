package com.classifiedads.mobile.uitests.features.products;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import android.graphics.Rect;
import androidx.test.platform.app.InstrumentationRegistry;
import org.junit.Test;
import org.junit.Assume;
import java.util.UUID;
import java.util.regex.Pattern;
import androidx.test.uiautomator.UiObject2;
import androidx.test.uiautomator.By;
import java.util.HashMap;
import java.util.Map;
import static org.junit.Assert.*;

public abstract class ProductsUiTests extends MobileUiTestSupport {
    protected ProductsUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    private Map<String, String> pdfExports() throws Exception {
        Map<String, String> exports = new HashMap<>();
        String paths = device.executeShellCommand("run-as " + packageName + " find cache/exports -type f");
        assertFalse("PDF verification requires a debuggable installed app: " + paths, paths.contains("run-as:"));
        for (String path : paths.split("\\r?\\n")) {
            if (path.matches("cache/exports/(?:[0-9]+-)?Products\\.pdf")) {
                exports.put(path, device.executeShellCommand("run-as " + packageName + " stat -c %y " + path).trim());
            }
        }
        return exports;
    }

    @Test public void productPdfExportCreatesValidFileAndOpensShareSheet() throws Exception {
        Assume.assumeTrue("Enable with -IncludePdf; requires the HTML-to-PDF server",
            "true".equals(InstrumentationRegistry.getArguments().getString("includePdf")));
        products();
        Map<String, String> previous = pdfExports();
        click(app.equals("capacitor") ? "Export as Pdf" : "Export PDF");
        try {
            waitFor("Android share sheet displays Products.pdf", () ->
                !packageName.equals(device.getCurrentPackageName())
                    && device.hasObject(By.textContains("Products.pdf")));
            Map<String, String> current = pdfExports();
            String exported = null;
            for (Map.Entry<String, String> entry : current.entrySet()) {
                if (!entry.getValue().equals(previous.get(entry.getKey()))) {
                    exported = entry.getKey();
                }
            }
            assertNotNull("Export must create or update a PDF, not reuse an old download", exported);
            String prefix = "run-as " + packageName;
            assertEquals("Downloaded content must be a PDF", "%PDF-",
                device.executeShellCommand(prefix + " head -c 5 " + exported));
            assertTrue("PDF must contain an end-of-file marker",
                device.executeShellCommand(prefix + " tail -c 1024 " + exported).contains("%%EOF"));
            capture("product-pdf-share-sheet");
        } finally {
            if (!packageName.equals(device.getCurrentPackageName())) {
                device.pressBack();
            }
        }
        waitFor("Dismissing sharing returns to the app", () -> packageName.equals(device.getCurrentPackageName()));
        ready();
    }

    private void removeFixture(String name) {
        assertTrue("Never delete without the unique fixture visible: " + name, recordLabelVisible(name));
        click("Delete");
        awaitLabel("Confirm");
        assertTrue("Confirmation must name the test fixture", contains("Delete " + name + "? This cannot be undone."));
        click("Confirm");
        waitFor("Temporary product deleted", () -> contains("No records found"));
    }

    private void filter(String value) {
        enterText(0, value);
        waitFor("Filtered product result", () -> contains("No records found") ||
            recordLabelVisible(value));
        if (!contains("No records found")) scrollToText("Edit");
    }

    private boolean recordLabelVisible(String value) {
        UiObject2 label = findPattern("(?s)" + Pattern.quote(value) + "(?:\\n.*)?");
        return label != null && !label.getClassName().equals("android.widget.EditText");
    }

    @Test public void toolbarButtonsHaveSeparatedTouchTargets() {
        products();
        Rect refresh = button("Refresh").getVisibleBounds();
        Rect create = button("Add new").getVisibleBounds();
        if (!app.equals("capacitor")) {
            assertEquals("Primary toolbar actions need equal widths", refresh.width(), create.width(), dp(2));
        }
        if (app.equals("kotlin")) {
            for (String title : new String[]{"Import", "Export CSV", "Export PDF"}) {
                Rect action = button(title).getVisibleBounds();
                assertEquals(title + " must fill its action column", refresh.width(), action.width(), dp(2));
                assertEquals(title + " must match the toolbar height", refresh.height(), action.height(), dp(2));
            }
        }
        for (Rect bounds : new Rect[]{refresh, create}) {
            assertTrue("Toolbar buttons must fit the screen", bounds.left >= 0 && bounds.right <= device.getDisplayWidth());
            assertTrue("Toolbar buttons need 48dp touch targets", bounds.width() >= dp(48) - 2 && bounds.height() >= dp(48) - 2);
        }
        int horizontalGap = Math.max(create.left - refresh.right, refresh.left - create.right);
        int verticalGap = Math.max(create.top - refresh.bottom, refresh.top - create.bottom);
        assertTrue("Toolbar buttons need a 12dp gap, including when wrapped",
            horizontalGap >= dp(12) - 2 || verticalGap >= dp(12) - 2);
    }

    @Test public void productSearchEmptyStateRefreshAndClear() {
        products();
        String emptySearch = fieldText(0);
        String query = "missing-" + UUID.randomUUID();
        enterText(0, query);
        waitFor("Unmatched search shows empty state", () -> contains("No records found"));
        assertNull("Empty search must not expose record actions", find("Delete"));
        click("Refresh");
        ready();
        assertTrue("Refresh preserves search", fieldText(0).contains(query));
        assertTrue("Refresh preserves empty result", contains("No records found"));
        enterText(0, "");
        ready();
        assertEquals("Search can be cleared", emptySearch, fieldText(0));
        scrollToText("Page 1");
    }

    @Test public void unsavedProductCanBeKeptThenDiscarded() {
        products();
        click("Add new");
        awaitLabel("Save");
        String emptyName = fieldText(0);
        String draft = "Unsaved-" + UUID.randomUUID();
        enterText(0, draft);
        capture("unsaved-product-draft");
        click("Cancel");
        waitFor("Discard warning appears", () -> contains("Discard unsaved changes?"));
        click("Cancel");
        awaitLabel("Save");
        assertEquals("Cancelling discard keeps the draft", draft, fieldText(0));
        click("Cancel");
        awaitLabel("Confirm");
        click("Confirm");
        waitFor("Discard closes editor", () -> find("Save") == null);
        click("Add new");
        awaitLabel("Save");
        assertEquals("New editor must not retain discarded draft", emptyName, fieldText(0));
        click("Cancel");
    }

    @Test public void productValidationAndPopupActionSpacing() {
        products();
        click("Add new");
        awaitLabel("Save");
        click("Save");
        waitFor("Required name validation", () -> contains("Name is required"));
        Rect cancel = button("Cancel").getVisibleBounds();
        Rect save = button("Save").getVisibleBounds();
        assertTrue("Cancel and Save need at least 12dp spacing, including when stacked",
            save.left - cancel.right >= dp(12) - 2 || save.top - cancel.bottom >= dp(12) - 2);
        assertTrue("Dialog actions need 48dp touch targets", cancel.height() >= dp(48) - 2 && save.height() >= dp(48) - 2);
        click("Cancel");
        waitFor("Editor closes without creating a record", () -> find("Save") == null);
        ready();
    }

    @Test public void productCreateUpdateHistoryDelete() throws Exception {
        Assume.assumeTrue("Enable with -IncludeCrud; creates and removes one temporary product",
            "true".equals(InstrumentationRegistry.getArguments().getString("includeCrud")));
        String name = "UITest-" + app + "-" + UUID.randomUUID().toString().substring(0, 8);
        boolean mayExist = false;
        try {
            products();
            click("Add new");
            awaitLabel("Save");
            enterText(0, name);
            enterText(1, "UI" + name.substring(name.length() - 8));
            enterText(2, "Temporary UI automation product");
            mayExist = true;
            click("Save");
            waitFor("Product save closes editor", () -> find("Save") == null);
            ready();
            filter(name);
            waitFor("Created product appears", () -> contains(name));
            click("Edit");
            awaitLabel("Save");
            assertEquals("Only edit the unique test fixture", name, fieldText(0));
            enterText(2, "Updated UI automation product");
            click("Save");
            waitFor("Product update closes editor", () -> find("Save") == null);
            ready();
            waitFor("Updated description appears", () -> contains("Updated UI automation product"));
            click("History");
            waitFor("Update history appears", () -> contains("UPDATED"));
            capture("product-history");
            if (find("Back") != null) click("Back"); else click("Close");
            ready();
            removeFixture(name);
            mayExist = false;
        } catch (Throwable failure) {
            capture("product-crud-failure-before-cleanup");
            throw failure;
        } finally {
            if (mayExist) {
                restart();
                products();
                filter(name);
                if (!contains("No records found")) removeFixture(name);
            }
        }
    }
}
