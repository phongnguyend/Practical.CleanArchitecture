import { TestBed } from "@angular/core/testing";
import {
  HttpClientTestingModule,
  HttpTestingController,
  TestRequest,
} from "@angular/common/http/testing";
import { ProductService } from "./product.service";
import { IProduct } from "./product";
import { environment } from "src/environments/environment";
import { GuidEmpty } from "../shared/constants";

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
      imports: [HttpClientTestingModule],
      providers: [ProductService],
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
