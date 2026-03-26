import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubscriptionApprovalComponent } from './subscription-approval.component';

describe('SubscriptionApprovalComponent', () => {
  let component: SubscriptionApprovalComponent;
  let fixture: ComponentFixture<SubscriptionApprovalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubscriptionApprovalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubscriptionApprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
