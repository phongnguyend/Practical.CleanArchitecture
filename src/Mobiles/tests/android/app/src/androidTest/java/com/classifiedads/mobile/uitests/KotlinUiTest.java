package com.classifiedads.mobile.uitests;

import com.classifiedads.mobile.uitests.features.biometrics.BiometricsUiTests;
import com.classifiedads.mobile.uitests.features.admin.AdminUiTests;
import org.junit.runner.RunWith;
import com.classifiedads.mobile.uitests.features.notifications.NotificationsUiTests;
import org.junit.runners.Suite;
import com.classifiedads.mobile.uitests.features.home.HomeUiTests;
import com.classifiedads.mobile.uitests.features.products.ProductsUiTests;
import com.classifiedads.mobile.uitests.features.users.UsersUiTests;
import com.classifiedads.mobile.uitests.features.roles.RolesUiTests;
import com.classifiedads.mobile.uitests.features.configuration.ConfigurationUiTests;
import com.classifiedads.mobile.uitests.features.navigation.NavigationUiTests;

@RunWith(Suite.class)
@Suite.SuiteClasses({
    KotlinUiTest.Home.class,
    KotlinUiTest.Products.class,
    KotlinUiTest.Users.class,
    KotlinUiTest.Roles.class,
    KotlinUiTest.Configuration.class,
    KotlinUiTest.Navigation.class,
    KotlinUiTest.Admin.class,
    KotlinUiTest.Notifications.class
})
public class KotlinUiTest {
    public static class Home extends HomeUiTests {
        public Home() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }

        @org.junit.Test public void workspaceHeaderUsesCompactThemeControl() {
            android.graphics.Rect theme = actionable(findPattern("Theme: .*" )).getVisibleBounds();
            org.junit.Assert.assertTrue("Theme touch target", theme.width() >= dp(44));
            org.junit.Assert.assertTrue("Theme control is compact", theme.width() <= dp(48) + 2);
            android.graphics.Rect title = find("CLASSIFIED ADS (Kotlin)").getVisibleBounds();
            org.junit.Assert.assertTrue("Title does not overlap theme", title.right <= theme.left);
        }
    }
    public static class Products extends ProductsUiTests {
        public Products() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }
    }
    public static class Users extends UsersUiTests {
        public Users() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }
    }
    public static class Roles extends RolesUiTests {
        public Roles() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }
    }
    public static class Configuration extends ConfigurationUiTests {
        public Configuration() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }
    }
    public static class Navigation extends NavigationUiTests {
        public Navigation() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }
    }
    // Explicit selection only: requires enrolled emulator biometrics.
    public static class Biometrics extends BiometricsUiTests {
        public Biometrics() {
            super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity");
        }
    }
    public static class Notifications extends NotificationsUiTests {
        public Notifications() {
            super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity");
        }
    }
    public static class Admin extends AdminUiTests {
        public Admin() {
            super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity");
        }
    }
}
