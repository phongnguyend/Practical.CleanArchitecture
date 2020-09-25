import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { debounceTime } from "rxjs/operators";

import { IProduct } from "../product";
import { ProductService } from "../product.service";

@Component({
  selector: "app-edit-product",
  templateUrl: "./edit-product.component.html",
  styleUrls: ["./edit-product.component.css"]
})
export class EditProductComponent implements OnInit {
  product: IProduct;
  postErrorMessage: string = "";
  postError = false;

  productForm: FormGroup;
  isSubmitted: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.productForm = this.fb.group({
      name: ["", [Validators.required, Validators.minLength(3)]],
      code: ["", [Validators.required, Validators.maxLength(10)]],
      description: ["", [Validators.required, Validators.maxLength(100)]]
    });

    this.productForm
      .get("name")
      .valueChanges.subscribe(value => console.log(value));

    this.productForm
      .get("code")
      .valueChanges.pipe(debounceTime(1000))
      .subscribe(value => console.log(value));

    const id = this.route.snapshot.paramMap.get("id");
    this.productService.getProduct(id).subscribe({
      next: product => {
        this.product = product;
        this.productForm.patchValue({
          name: product.name,
          code: product.code,
          description: product.description
        });
      }
      //error: err => this.errorMessage = err
    });
  }

  onSubmit() {
    this.isSubmitted = true;
    if (this.productForm.valid) {
      var product = Object.assign({}, this.product, this.productForm.value);
      this.productService.updateProduct(product).subscribe(
        result => {
          console.log("success: ", result);
          this.productForm.reset();
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
