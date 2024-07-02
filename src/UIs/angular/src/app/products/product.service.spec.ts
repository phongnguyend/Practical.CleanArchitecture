import { TestBed } from "@angular/core/testing";
import { HttpTestingController, TestRequest, provideHttpClientTesting } from "@angular/common/http/testing";
import { ProductService } from "./product.service";
import { IProduct } from "./product";
import { environment } from "src/environments/environment";
import { GuidEmpty } from "../shared/constants";
import { provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";

describe("ProductService Tests", () => {
  let productService: ProductService;
  let httpTestingController: HttpTestingController;

  let testProducts: IProduct[] = [
    {
      id: GuidEmpty,
      name: null,
      code: null,
      description: null,
      imageUrl: null,
      price: null,
      releaseDate: null,
      starRating: null,
    },
    {
      id: GuidEmpty,
      name: null,
      code: null,
      description: null,
      imageUrl: null,
      price: null,
      releaseDate: null,
      starRating: null,
    },
    {
      id: GuidEmpty,
      name: null,
      code: null,
      description: null,
      imageUrl: null,
      price: null,
      releaseDate: null,
      starRating: null,
    },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
    imports: [],
    providers: [ProductService, provideHttpClient(withInterceptorsFromDi()), provideHttpClientTesting()]
});

    productService = TestBed.get(ProductService);
    httpTestingController = TestBed.get(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it("should GET all products", () => {
    productService.getProducts().subscribe((data: IProduct[]) => {
      expect(data.length).toBe(3);
    });

    let booksRequest: TestRequest = httpTestingController.expectOne(
      environment.ResourceServer.Endpoint + "products"
    );
    expect(booksRequest.request.method).toEqual("GET");

    booksRequest.flush(testProducts);
  });
});
