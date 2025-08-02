import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { provideHttpClient } from "@angular/common/http";
import { provideHttpClientTesting } from "@angular/common/http/testing";
import { BsModalService } from 'ngx-bootstrap/modal';

import { ListFilesComponent } from "./list-files.component";

describe("ListFileComponent", () => {
  let component: ListFilesComponent;
  let fixture: ComponentFixture<ListFilesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ListFilesComponent],
      providers: [
        provideHttpClient(),
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
    fixture = TestBed.createComponent(ListFilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
