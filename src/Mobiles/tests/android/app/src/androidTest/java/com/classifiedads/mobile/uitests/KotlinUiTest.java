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
    KotlinUiTest.Home.class,
    KotlinUiTest.Products.class,
    KotlinUiTest.Users.class,
    KotlinUiTest.Roles.class,
    KotlinUiTest.Configuration.class,
    KotlinUiTest.Navigation.class
})
public class KotlinUiTest {
    public static class Home extends HomeUiTests {
        public Home() { super("kotlin", "com.classifiedads.mobile.kotlin", "com.classifiedads.mobile.MainActivity"); }
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
}
