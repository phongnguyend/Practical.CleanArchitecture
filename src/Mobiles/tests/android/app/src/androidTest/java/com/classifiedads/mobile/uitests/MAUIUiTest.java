package com.classifiedads.mobile.uitests;

import com.classifiedads.mobile.uitests.features.biometrics.BiometricsUiTests;
import org.junit.runner.RunWith;
import org.junit.runners.Suite;
import com.classifiedads.mobile.uitests.features.home.HomeUiTests;
import com.classifiedads.mobile.uitests.features.products.ProductsUiTests;
import com.classifiedads.mobile.uitests.features.users.UsersUiTests;
import com.classifiedads.mobile.uitests.features.roles.RolesUiTests;
import com.classifiedads.mobile.uitests.features.configuration.ConfigurationUiTests;
import com.classifiedads.mobile.uitests.features.navigation.NavigationUiTests;

@RunWith(Suite.class)
@Suite.SuiteClasses({
    MAUIUiTest.Home.class,
    MAUIUiTest.Products.class,
    MAUIUiTest.Users.class,
    MAUIUiTest.Roles.class,
    MAUIUiTest.Configuration.class,
    MAUIUiTest.Navigation.class
})
public class MAUIUiTest {
    public static class Home extends HomeUiTests {
        public Home() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
    public static class Products extends ProductsUiTests {
        public Products() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
    public static class Users extends UsersUiTests {
        public Users() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
    public static class Roles extends RolesUiTests {
        public Roles() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
    public static class Configuration extends ConfigurationUiTests {
        public Configuration() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
    public static class Navigation extends NavigationUiTests {
        public Navigation() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
    // Explicit selection only: requires enrolled emulator biometrics.
    public static class Biometrics extends BiometricsUiTests {
        public Biometrics() {
            super("maui", "com.classifiedads.mobile.maui", ".MainActivity");
        }
    }
}
