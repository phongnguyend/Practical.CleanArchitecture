package com.classifiedads.mobile.uitests.features.roles;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class RolesUiTests extends MobileUiTestSupport {
    protected RolesUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    @Test public void roleRequiresName() { requiredField("Roles", "Name is required"); }
}
