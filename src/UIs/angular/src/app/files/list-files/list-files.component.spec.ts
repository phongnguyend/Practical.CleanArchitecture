import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { ListFilesComponent } from "./list-files.component";

describe("ListFileComponent", () => {
  let component: ListFilesComponent;
  let fixture: ComponentFixture<ListFilesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ListFilesComponent],
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
