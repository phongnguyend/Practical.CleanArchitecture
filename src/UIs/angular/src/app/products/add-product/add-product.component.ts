import { Component, OnInit } from "@angular/core";
import { IProduct } from "../product";
import { NgModel, NgForm } from "@angular/forms";
import { ProductService } from "../product.service";
import { Router } from "@angular/router";

@Component({
  templateUrl: "./add-product.component.html",
  styleUrls: ["./add-product.component.css"]
})
export class AddProductComponent implements OnInit {
  product: IProduct = {
    id: "00000000-0000-0000-0000-000000000000",
    name: null,
    code: null,
    description: null,
    imageUrl: null,
    price: null,
    releaseDate: null,
    starRating: null
  };
  postErrorMessage: string = "";
  postError = false;
  isDirty = false;

  constructor(private productService: ProductService, private router: Router) {}

  ngOnInit(): void {}

  onBlur(field: NgModel) {
    if (field.dirty) {
      this.isDirty = true;
    }
    console.log("in onBlur: ", field.valid);
  }

  onSubmit(form: NgForm) {
    console.log("in onSubmit: ", form.value, this.product);

    if (form.valid) {
      this.productService.addProduct(this.product).subscribe(
        result => {
          console.log("success: ", result);
          this.isDirty = false;
          this.router.navigate(["/products", result.id]);
        },
        error => this.onHttpError(error)
      );
    } else {
      this.postError = true;
      this.postErrorMessage = "Please fix the errors";
    }
  }

  onHttpError(errorResponse: any) {
    console.log("error: ", errorResponse);
    this.postError = true;
    this.postErrorMessage = errorResponse.error.errorMessage;
  }
}
