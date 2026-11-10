package com.classifiedads.mobile.uitests;

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
    ReactNativeUiTest.Home.class,
    ReactNativeUiTest.Products.class,
    ReactNativeUiTest.Users.class,
    ReactNativeUiTest.Roles.class,
    ReactNativeUiTest.Configuration.class,
    ReactNativeUiTest.Navigation.class
})
public class ReactNativeUiTest {
    public static class Home extends HomeUiTests {
        public Home() { super("reactnative", "com.classifiedads.mobile.reactnative", ".MainActivity"); }
    }
    public static class Products extends ProductsUiTests {
        public Products() { super("reactnative", "com.classifiedads.mobile.reactnative", ".MainActivity"); }
    }
    public static class Users extends UsersUiTests {
        public Users() { super("reactnative", "com.classifiedads.mobile.reactnative", ".MainActivity"); }
    }
    public static class Roles extends RolesUiTests {
        public Roles() { super("reactnative", "com.classifiedads.mobile.reactnative", ".MainActivity"); }
    }
    public static class Configuration extends ConfigurationUiTests {
        public Configuration() { super("reactnative", "com.classifiedads.mobile.reactnative", ".MainActivity"); }
    }
    public static class Navigation extends NavigationUiTests {
        public Navigation() { super("reactnative", "com.classifiedads.mobile.reactnative", ".MainActivity"); }
    }
}
