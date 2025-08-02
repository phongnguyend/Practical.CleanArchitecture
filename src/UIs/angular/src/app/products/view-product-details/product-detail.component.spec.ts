import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { BsModalService } from 'ngx-bootstrap/modal';

import { ProductDetailComponent } from "./product-detail.component";
import { RouterTestingModule } from "@angular/router/testing";
import { provideHttpClientTesting } from "@angular/common/http/testing";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";

describe("ProductDetailComponent", () => {
  let component: ProductDetailComponent;
  let fixture: ComponentFixture<ProductDetailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
    declarations: [ProductDetailComponent],
    imports: [RouterTestingModule],
    providers: [
      provideHttpClient(withInterceptorsFromDi()), 
      provideHttpClientTesting(),
      {
        provide: BsModalService,
        useValue: {
          show: jest.fn(),
          hide: jest.fn()
        }
      }
    ]
}).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
