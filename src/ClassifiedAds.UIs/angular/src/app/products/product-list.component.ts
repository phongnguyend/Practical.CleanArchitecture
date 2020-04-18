import { Component, OnInit, TemplateRef } from "@angular/core";

import { IProduct } from "./product";
import { ProductService } from "./product.service";
import { Title } from "@angular/platform-browser";
import { IAuditLogEntry } from "../auditlogs/audit-log";
import { BsModalService } from "ngx-bootstrap";

@Component({
  templateUrl: "./product-list.component.html",
  styleUrls: ["./product-list.component.css"],
})
export class ProductListComponent implements OnInit {
  pageTitle = "Product List";
  imageWidth = 50;
  imageMargin = 2;
  showImage = false;
  errorMessage = "";
  auditLogs: IAuditLogEntry[];

  _listFilter = "";
  get listFilter(): string {
    return this._listFilter;
  }
  set listFilter(value: string) {
    this._listFilter = value;
    this.filteredProducts = this.listFilter
      ? this.performFilter(this.listFilter)
      : this.products;
  }

  filteredProducts: IProduct[] = [];
  products: IProduct[] = [];

  constructor(
    private productService: ProductService,
    private titleService: Title,
    private modalService: BsModalService
  ) {
    this.titleService.setTitle("ClassifiedAds Angular - Product");
  }

  onRatingClicked(message: string): void {
    this.pageTitle = "Product List: " + message;
  }

  onDeleteConfirmed(product: IProduct): void {
    this.productService.deleteProduct(product).subscribe({
      next: (rs) => {
        console.log(rs);
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  performFilter(filterBy: string): IProduct[] {
    filterBy = filterBy.toLocaleLowerCase();
    return this.products.filter(
      (product: IProduct) =>
        product.name.toLocaleLowerCase().indexOf(filterBy) !== -1
    );
  }

  toggleImage(): void {
    this.showImage = !this.showImage;
  }

  ngOnInit(): void {
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.filteredProducts = this.products;
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  viewAuditLogs(template: TemplateRef<any>, id: string) {
    this.productService.getAuditLogs(id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.modalService.show(template, { class: "modal-xl" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
