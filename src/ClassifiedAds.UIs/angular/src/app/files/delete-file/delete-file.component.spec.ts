import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteFileComponent } from './delete-file.component';

describe('DeleteFileComponent', () => {
  let component: DeleteFileComponent;
  let fixture: ComponentFixture<DeleteFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeleteFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
