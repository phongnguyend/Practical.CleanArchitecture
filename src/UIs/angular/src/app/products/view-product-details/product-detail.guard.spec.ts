import { TestBed, inject, waitForAsync } from "@angular/core/testing";

import { ProductDetailGuard } from "./product-detail.guard";
import { RouterTestingModule } from "@angular/router/testing";
import { HttpClientTestingModule } from "@angular/common/http/testing";

describe("ProductDetailGuard", () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule],
      providers: [ProductDetailGuard]
    });
  });

  it("should ...", inject([ProductDetailGuard], (guard: ProductDetailGuard) => {
    expect(guard).toBeTruthy();
  }));
});
