package com.classifiedads.mobile.uitests.features.products;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import android.graphics.Rect;
import androidx.test.platform.app.InstrumentationRegistry;
import org.junit.Test;
import org.junit.Assume;
import java.util.UUID;
import java.util.regex.Pattern;
import androidx.test.uiautomator.UiObject2;
import static org.junit.Assert.*;

public abstract class ProductsUiTests extends MobileUiTestSupport {
    protected ProductsUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
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
        assertTrue("Cancel and Save need at least 8dp spacing", save.left - cancel.right >= dp(8) - 2);
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
