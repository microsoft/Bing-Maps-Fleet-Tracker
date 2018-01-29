import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DispatchingEditorComponent } from './dispatching-editor.component';

describe('DispatchingEditorComponent', () => {
  let component: DispatchingEditorComponent;
  let fixture: ComponentFixture<DispatchingEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DispatchingEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DispatchingEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
