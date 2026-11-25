package com.classifiedads.mobile.uitests.features.biometrics;

import android.os.SystemClock;
import android.util.Log;
import androidx.test.platform.app.InstrumentationRegistry;
import androidx.test.uiautomator.By;
import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import org.junit.Test;
import org.junit.Before;
import androidx.test.uiautomator.UiDevice;
import androidx.test.uiautomator.Configurator;
import static org.junit.Assert.*;

/** Opt-in emulator scenario: Run-UiTests.ps1 supplies enrolled fingerprint touches. */
public abstract class BiometricsUiTests extends MobileUiTestSupport {
    private int touchSequence;

    protected BiometricsUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    @Override @Before public void openApp() throws Exception {
        Configurator.getInstance().setWaitForIdleTimeout(500);
        device = UiDevice.getInstance(InstrumentationRegistry.getInstrumentation());
        device.executeShellCommand("am force-stop " + packageName);
        device.executeShellCommand("am start -W -n " + packageName + "/" + activity);
        waitFor("Signed-in home or saved biometric session", () -> find("Unlock with biometrics") != null
            || find("Practical.CleanArchitecture") != null);
        if (find("Unlock with biometrics") != null) {
            click("Unlock with biometrics");
            authenticateUntil("Home");
            click("Home");
            scrollToText("Disable biometric login");
            click("Disable biometric login");
            authenticateUntil("Enable biometric login");
        }
    }

    private void authenticateUntil(String label) {
        String run = InstrumentationRegistry.getArguments().getString("runId");
        final int[] attempts = {0};
        final long[] lastTouch = {0};
        waitFor("Biometric authentication completes: " + label, () -> {
            assertFalse("Fingerprint rejected. Check the enrolled FingerprintId before retrying.",
                device.hasObject(By.pkg("com.android.systemui").text(java.util.regex.Pattern.compile("(?i).*(not recognized|too many attempts).*"))));
            assertFalse("Unlock the emulator with its screen-lock PIN before running this test.",
                device.hasObject(By.pkg("com.android.systemui").text("Enter PIN")));
            if (find(label) != null) {
                return true;
            }
            // The host emits a simulated enrolled fingerprint only when explicitly requested.
            boolean prompt = device.hasObject(By.pkg("com.android.systemui").text("Unlock Classified Ads"))
                || device.hasObject(By.pkg("com.android.systemui").text("Protect Classified Ads session"));
            if (prompt && SystemClock.uptimeMillis() - lastTouch[0] > 2500) {
                assertTrue("Biometric prompt did not accept the enrolled fingerprint", attempts[0]++ < 3);
                lastTouch[0] = SystemClock.uptimeMillis();
                Log.i("ClassifiedAdsBiometrics", "TOUCH:" + run + ":" + app + ":" + (++touchSequence));
            }
            return false;
        });
    }

    private void launchLocked() throws Exception {
        device.executeShellCommand("am force-stop " + packageName);
        device.executeShellCommand("am start -W -n " + packageName + "/" + activity);
        awaitLabel("Unlock with biometrics");
        assertNull("Protected records are not displayed", find("Add new"));
        assertNotNull("Normal sign-in remains available", find("Sign in"));
    }

    @Test public void enrollmentLockCancellationRestartAndDisable() throws Exception {
        scrollToText("Enable biometric login");
        click("Enable biometric login");
        authenticateUntil("Disable biometric login");
        scrollToText("Lock app");
        click("Lock app");
        awaitLabel("Unlock with biometrics");
        click("Unlock with biometrics");
        waitFor("Native biometric prompt", () -> device.hasObject(By.text("Cancel").pkg("com.android.systemui"))
            || device.hasObject(By.text("CANCEL").pkg("com.android.systemui")));
        device.pressBack();
        awaitLabel("Unlock with biometrics");
        assertNull("Cancelling cannot reveal the workspace", find("Practical.CleanArchitecture"));
        click("Unlock with biometrics");
        authenticateUntil("Home");

        device.pressHome();
        SystemClock.sleep(800);
        device.executeShellCommand("am start -W -n " + packageName + "/" + activity);
        awaitLabel("Unlock with biometrics");
        click("Unlock with biometrics");
        authenticateUntil("Home");

        launchLocked();
        click("Unlock with biometrics");
        authenticateUntil("Home");
        click("Home");
        scrollToText("Disable biometric login");
        click("Disable biometric login");
        authenticateUntil("Enable biometric login");
        restart();
        assertNull("Disabling restores normal session startup", find("Unlock with biometrics"));
        capture("biometric-lifecycle");
    }
}
