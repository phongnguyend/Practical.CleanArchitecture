package com.classifiedads.mobile.uitests.features.configuration;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class ConfigurationUiTests extends MobileUiTestSupport {
    protected ConfigurationUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    @Test public void configurationRequiresKey() { requiredField("Configuration Entries", "Key is required"); }
}
