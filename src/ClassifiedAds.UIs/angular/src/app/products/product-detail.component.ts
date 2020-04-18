import { Component, OnInit, TemplateRef } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";

import { IProduct } from "./product";
import { ProductService } from "./product.service";
import { BsModalService } from "ngx-bootstrap";
import { IAuditLogEntry } from "../auditlogs/audit-log";

@Component({
  templateUrl: "./product-detail.component.html",
  styleUrls: ["./product-detail.component.css"],
})
export class ProductDetailComponent implements OnInit {
  pageTitle = "Product Detail";
  errorMessage = "";
  product: IProduct | undefined;
  auditLogs: IAuditLogEntry[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private modalService: BsModalService
  ) {}

  ngOnInit() {
    const param = this.route.snapshot.paramMap.get("id");
    if (param) {
      const id = param;
      this.getProduct(id);
    }
  }

  getProduct(id: string) {
    this.productService.getProduct(id).subscribe({
      next: (product) => (this.product = product),
      error: (err) => (this.errorMessage = err),
    });
  }

  onBack(): void {
    this.router.navigate(["/products"]);
  }

  viewAuditLogs(template: TemplateRef<any>) {
    const id = this.route.snapshot.paramMap.get("id");
    this.productService.getAuditLogs(id).subscribe({
      next: (logs) => {
        this.auditLogs = logs;
        this.modalService.show(template, { class: "modal-xl" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
