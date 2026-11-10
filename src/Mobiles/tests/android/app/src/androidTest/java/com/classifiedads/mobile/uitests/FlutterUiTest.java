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
    FlutterUiTest.Home.class,
    FlutterUiTest.Products.class,
    FlutterUiTest.Users.class,
    FlutterUiTest.Roles.class,
    FlutterUiTest.Configuration.class,
    FlutterUiTest.Navigation.class
})
public class FlutterUiTest {
    public static class Home extends HomeUiTests {
        public Home() { super("flutter", "com.classifiedads.mobile.classified_ads_mobile", ".MainActivity"); }
    }
    public static class Products extends ProductsUiTests {
        public Products() { super("flutter", "com.classifiedads.mobile.classified_ads_mobile", ".MainActivity"); }
    }
    public static class Users extends UsersUiTests {
        public Users() { super("flutter", "com.classifiedads.mobile.classified_ads_mobile", ".MainActivity"); }
    }
    public static class Roles extends RolesUiTests {
        public Roles() { super("flutter", "com.classifiedads.mobile.classified_ads_mobile", ".MainActivity"); }
    }
    public static class Configuration extends ConfigurationUiTests {
        public Configuration() { super("flutter", "com.classifiedads.mobile.classified_ads_mobile", ".MainActivity"); }
    }
    public static class Navigation extends NavigationUiTests {
        public Navigation() { super("flutter", "com.classifiedads.mobile.classified_ads_mobile", ".MainActivity"); }
    }
}
