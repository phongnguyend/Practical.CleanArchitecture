import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { ModalModule } from "ngx-bootstrap";

import { ProductListComponent } from "./product-list.component";
import { ProductDetailComponent } from "./product-detail.component";
import { ProductDetailGuard } from "./product-detail.guard";
import { SharedModule } from "../shared/shared.module";
import { AddProductComponent } from "./add-product/add-product.component";
import { DeleteProductComponent } from "./delete-product/delete-product.component";
import { EditProductComponent } from "./edit-product/edit-product.component";
import { AddProductGuard } from "./add-product/add-product.guard";
import { EditProductGuard } from "./edit-product/edit-product.guard";

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: "products", component: ProductListComponent },
      {
        path: "products/add",
        component: AddProductComponent,
        canDeactivate: [AddProductGuard]
      },
      {
        path: "products/edit/:id",
        component: EditProductComponent,
        canDeactivate: [EditProductGuard]
      },
      {
        path: "products/:id",
        component: ProductDetailComponent,
        canActivate: [ProductDetailGuard]
      }
    ]),
    ModalModule.forRoot(),
    SharedModule
  ],
  declarations: [
    ProductListComponent,
    ProductDetailComponent,
    AddProductComponent,
    DeleteProductComponent,
    EditProductComponent
  ]
})
export class ProductModule {}
