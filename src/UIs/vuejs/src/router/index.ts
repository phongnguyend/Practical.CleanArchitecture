import { createRouter, createWebHistory } from "vue-router";
import Home from "../views/Home.vue";
import OidcLoginRedirect from "../auth/OidcLoginRedirect.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
  {
    path: "/oidc-login-redirect",
    name: "OidcLoginRedirect",
    component: OidcLoginRedirect,
  },
  {
    path: "/settings",
    name: "Settings",
    component: () => import("../views/settings/List.vue"),
  },
  {
    path: "/files",
    name: "Files",
    component: () => import("../views/files/List.vue"),
  },
  {
    path: "/files/upload",
    name: "UploadFile",
    component: () => import("../views/files/Upload.vue"),
  },
  {
    path: "/files/edit/:id",
    name: "EditFile",
    component: () => import("../views/files/Edit.vue"),
  },
  {
    path: "/products",
    name: "Products",
    component: () => import("../views/products/List.vue"),
  },
  {
    path: "/products/add",
    name: "AddProduct",
    component: () => import("../views/products/Edit.vue"),
  },
  {
    path: "/products/edit/:id",
    name: "EditProduct",
    component: () => import("../views/products/Edit.vue"),
  },
  {
    path: "/products/:id",
    name: "ProductDetail",
    component: () => import("../views/products/Detail.vue"),
  },
  {
    path: "/users",
    name: "Users",
    component: () => import("../views/users/List.vue"),
  },
  {
    path: "/users/add",
    name: "AddUser",
    component: () => import("../views/users/Edit.vue"),
  },
  {
    path: "/users/edit/:id",
    name: "EditUser",
    component: () => import("../views/users/Edit.vue"),
  },
  {
    path: "/users/:id",
    name: "UserDetail",
    component: () => import("../views/users/Detail.vue"),
  },
  {
    path: "/auditlogs",
    name: "AuditLogs",
    component: () => import("../views/auditlogs/List.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
