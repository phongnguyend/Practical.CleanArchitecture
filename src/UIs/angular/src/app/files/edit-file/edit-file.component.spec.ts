import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalService } from 'ngx-bootstrap/modal';

import { EditFileComponent } from './edit-file.component';

describe('EditFileComponent', () => {
  let component: EditFileComponent;
  let fixture: ComponentFixture<EditFileComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditFileComponent ],
      imports: [ FormsModule, RouterTestingModule ],
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
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
