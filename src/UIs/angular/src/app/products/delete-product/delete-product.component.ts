import { ModalRef, ModalModule, ModalService } from "../../shared/modal/modal";
import {
  Component,
  OnInit,
  TemplateRef,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from "@angular/core";

import { IProduct } from "../product";
import { ActionIconComponent } from "../../shared/action-icon.component";

@Component({
  selector: "app-delete-product",
  templateUrl: "./delete-product.component.html",
  styleUrls: ["./delete-product.component.css"],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [ModalModule, ActionIconComponent],
})
export class DeleteProductComponent implements OnInit {
  @Input() product: IProduct;
  @Output() confirmed: EventEmitter<IProduct> = new EventEmitter<IProduct>();

  modalRef: ModalRef<any>;
  message: string;
  constructor(private dialog: ModalService) {}

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
