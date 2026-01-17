import { Component, OnInit, TemplateRef } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { ActivatedRoute, Router } from "@angular/router";

import { IProduct } from "../product";
import { ProductService } from "../product.service";
import { MatDialog } from "@angular/material/dialog";
import { IAuditLogEntry } from "../../auditlogs/audit-log";
import { StarComponent } from "../../shared/star.component";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  templateUrl: "./product-detail.component.html",
  styleUrls: ["./product-detail.component.css"],
  standalone: true,
  imports: [CommonModule, RouterModule, StarComponent, MatDialogModule],
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
    private dialog: MatDialog
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
        this.dialog.open(template, { width: "90%", maxWidth: "1200px" });
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
