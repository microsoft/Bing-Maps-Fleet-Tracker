import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DispatchingShowComponent } from './dispatching-show.component';

describe('DispatchingShowComponent', () => {
  let component: DispatchingShowComponent;
  let fixture: ComponentFixture<DispatchingShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DispatchingShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DispatchingShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
