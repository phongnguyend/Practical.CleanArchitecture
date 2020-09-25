import {
  Component,
  OnInit,
  TemplateRef,
  Input,
  Output,
  EventEmitter
} from "@angular/core";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { IProduct } from "../product";

@Component({
  selector: "app-delete-product",
  templateUrl: "./delete-product.component.html",
  styleUrls: ["./delete-product.component.css"]
})
export class DeleteProductComponent implements OnInit {
  @Input() product: IProduct;
  @Output() confirmed: EventEmitter<IProduct> = new EventEmitter<IProduct>();

  modalRef: BsModalRef;
  message: string;
  constructor(private modalService: BsModalService) {}

  ngOnInit(): void {}

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, { class: "modal-sm" });
  }

  confirm(): void {
    this.confirmed.emit(this.product);
    this.modalRef.hide();
  }

  decline(): void {
    this.modalRef.hide();
  }
}
