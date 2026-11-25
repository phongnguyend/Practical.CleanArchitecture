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
    CapacitorUiTest.Home.class,
    CapacitorUiTest.Products.class,
    CapacitorUiTest.Users.class,
    CapacitorUiTest.Roles.class,
    CapacitorUiTest.Configuration.class,
    CapacitorUiTest.Navigation.class,
    CapacitorUiTest.Admin.class,
    CapacitorUiTest.Notifications.class
})
public class CapacitorUiTest {
    public static class Home extends HomeUiTests {
        public Home() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Products extends ProductsUiTests {
        public Products() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Users extends UsersUiTests {
        public Users() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Roles extends RolesUiTests {
        public Roles() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Configuration extends ConfigurationUiTests {
        public Configuration() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Navigation extends NavigationUiTests {
        public Navigation() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    // Explicit selection only: requires enrolled emulator biometrics.
    public static class Biometrics extends BiometricsUiTests {
        public Biometrics() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Notifications extends NotificationsUiTests {
        public Notifications() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
    public static class Admin extends AdminUiTests {
        public Admin() {
            super("capacitor", "com.classifiedads.mobile.capacitor", ".MainActivity");
        }
    }
}
