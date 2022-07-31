import { Component, OnInit, TemplateRef } from "@angular/core";

import { IProduct } from "../product";
import { ProductService } from "../product.service";
import { Title } from "@angular/platform-browser";
import { IAuditLogEntry } from "../../auditlogs/audit-log";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgForm } from "@angular/forms";

@Component({
  templateUrl: "./list-products.component.html",
  styleUrls: ["./list-products.component.css"],
})
export class ListProductsComponent implements OnInit {
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

  importCsvModalRef: BsModalRef;
  importingFile;

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

  exportAsPdf() {
    this.productService.exportAsPdf().subscribe({
      next: (rs) => {
        const url = window.URL.createObjectURL(rs);
        const element = document.createElement("a");
        element.href = url;
        element.download = "Products.pdf";
        document.body.appendChild(element);
        element.click();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  exportAsCsv() {
    this.productService.exportAsCsv().subscribe({
      next: (rs) => {
        const url = window.URL.createObjectURL(rs);
        const element = document.createElement("a");
        element.href = url;
        element.download = "Products.csv";
        document.body.appendChild(element);
        element.click();
      },
      error: (err) => (this.errorMessage = err),
    });
  }

  openImportCsvModal(template: TemplateRef<any>) {
    this.importingFile = null;
    this.importCsvModalRef = this.modalService.show(template, {
      class: "modal-sm",
    });
  }

  handleFileInput(files: FileList) {
    this.importingFile = files.item(0);
  }

  confirmImportCsvFile(form: NgForm) {
    if (form.invalid || !this.importingFile) {
      return;
    }

    var request = this.productService.importCsvFile(this.importingFile);

    request.subscribe({
      next: (rs) => {
        console.log(rs);
        this.importCsvModalRef.hide();
        this.ngOnInit();
      },
      error: (err) => (this.errorMessage = err),
    });
  }
}
