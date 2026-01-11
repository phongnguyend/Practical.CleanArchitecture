import { Routes } from "@angular/router";
import { WelcomeComponent } from "./core/welcome.component";
import { OidcLoginRedirect } from "./auth/oidc-login-redirect.component";

// Products
import { ListProductsComponent } from "./products/list-products/list-products.component";
import { ProductDetailComponent } from "./products/view-product-details/product-detail.component";
import { AddProductComponent } from "./products/add-product/add-product.component";
import { EditProductComponent } from "./products/edit-product/edit-product.component";
import { ProductDetailGuard } from "./products/view-product-details/product-detail.guard";
import { AddProductGuard } from "./products/add-product/add-product.guard";
import { EditProductGuard } from "./products/edit-product/edit-product.guard";

// Users
import { ListUsersComponent } from "./users/list-users/list-users.component";
import { AddEditUserComponent } from "./users/add-edit-user/add-edit-user.component";
import { ViewUserDetailsComponent } from "./users/view-user-details/view-user-details.component";

// Files
import { ListFilesComponent } from "./files/list-files/list-files.component";
import { UploadFileComponent } from "./files/upload-file/upload-file.component";
import { EditFileComponent } from "./files/edit-file/edit-file.component";

// Settings
import { ConfigurationEntryListComponent } from "./settings/configuration-entry-list.component";

// Audit Logs
import { AuditLogListComponent } from "./auditlogs/audit-log-list.component";

export const routes: Routes = [
  { path: "welcome", component: WelcomeComponent },
  { path: "oidc-login-redirect", component: OidcLoginRedirect },

  // Products routes
  { path: "products", component: ListProductsComponent },
  {
    path: "products/add",
    component: AddProductComponent,
    canDeactivate: [AddProductGuard],
  },
  {
    path: "products/edit/:id",
    component: EditProductComponent,
    canDeactivate: [EditProductGuard],
  },
  {
    path: "products/:id",
    component: ProductDetailComponent,
    canActivate: [ProductDetailGuard],
  },

  // Users routes
  { path: "users", component: ListUsersComponent },
  {
    path: "users/add",
    component: AddEditUserComponent,
  },
  {
    path: "users/edit/:id",
    component: AddEditUserComponent,
  },
  {
    path: "users/:id",
    component: ViewUserDetailsComponent,
  },

  // Files routes
  { path: "files", component: ListFilesComponent },
  {
    path: "files/upload",
    component: UploadFileComponent,
  },
  {
    path: "files/edit/:id",
    component: EditFileComponent,
  },

  // Settings route
  { path: "settings", component: ConfigurationEntryListComponent },

  // Audit logs route
  { path: "auditlogs", component: AuditLogListComponent },

  { path: "", redirectTo: "welcome", pathMatch: "full" },
  { path: "**", redirectTo: "welcome", pathMatch: "full" },
];
