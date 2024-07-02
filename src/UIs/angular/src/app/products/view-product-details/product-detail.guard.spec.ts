import { TestBed, inject, waitForAsync } from "@angular/core/testing";

import { ProductDetailGuard } from "./product-detail.guard";
import { RouterTestingModule } from "@angular/router/testing";
import { provideHttpClientTesting } from "@angular/common/http/testing";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";

describe("ProductDetailGuard", () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
    imports: [RouterTestingModule],
    providers: [ProductDetailGuard, provideHttpClient(withInterceptorsFromDi()), provideHttpClientTesting()]
});
  });

  it("should ...", inject([ProductDetailGuard], (guard: ProductDetailGuard) => {
    expect(guard).toBeTruthy();
  }));
});
