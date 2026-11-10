package com.classifiedads.mobile.uitests.features.users;

import com.classifiedads.mobile.uitests.support.MobileUiTestSupport;
import org.junit.Test;
import static org.junit.Assert.*;

public abstract class UsersUiTests extends MobileUiTestSupport {
    protected UsersUiTests(String app, String packageName, String activity) {
        super(app, packageName, activity);
    }

    @Test public void userRequiresUsername() { requiredField("Users", "Username is required"); }
}
