import { Component, OnInit, TemplateRef, Input, Output, EventEmitter } from "@angular/core";

import { MatDialog, MatDialogRef } from "@angular/material/dialog";
import { IProduct } from "../product";
import { MatDialogModule } from "@angular/material/dialog";

@Component({
  selector: "app-delete-product",
  templateUrl: "./delete-product.component.html",
  styleUrls: ["./delete-product.component.css"],
  standalone: true,
  imports: [MatDialogModule],
})
export class DeleteProductComponent implements OnInit {
  @Input() product: IProduct;
  @Output() confirmed: EventEmitter<IProduct> = new EventEmitter<IProduct>();

  modalRef: MatDialogRef<any>;
  message: string;
  constructor(private dialog: MatDialog) {}

  ngOnInit(): void {}

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.dialog.open(template, { width: "400px" });
  }

  confirm(): void {
    this.confirmed.emit(this.product);
    this.modalRef.close();
  }

  decline(): void {
    this.modalRef.close();
  }
}
