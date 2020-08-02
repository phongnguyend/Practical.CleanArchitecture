import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import OidcLoginRedirect from "../auth/OidcLoginRedirect.vue";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/oidc-login-redirect",
    name: "OidcLoginRedirect",
    component: OidcLoginRedirect
  },
  {
    path: "/files",
    name: "File",
    component: () => import("../views/files/List.vue")
  },
  {
    path: "/files/upload",
    name: "UploadFile",
    component: () => import("../views/files/Upload.vue")
  },
  {
    path: "/files/edit/:id",
    name: "EditFile",
    component: () => import("../views/files/Edit.vue")
  },
  {
    path: "/products",
    name: "Product",
    component: () => import("../views/products/List.vue")
  },
  {
    path: "/products/add",
    name: "AddProduct",
    component: () => import("../views/products/Edit.vue")
  },
  {
    path: "/products/edit/:id",
    name: "EditProduct",
    component: () => import("../views/products/Edit.vue")
  },
  {
    path: "/products/:id",
    name: "ProductDetail",
    component: () => import("../views/products/Detail.vue")
  },
  {
    path: "/auditlogs",
    name: "AuditLogs",
    component: () => import("../views/auditlogs/List.vue")
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

export default router;
