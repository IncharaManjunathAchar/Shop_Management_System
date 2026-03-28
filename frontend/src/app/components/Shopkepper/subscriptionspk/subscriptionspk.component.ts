import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-subscriptionspk',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './subscriptionspk.component.html',
  styleUrl: './subscriptionspk.component.css'
})
export class SubscriptionspkComponent implements OnInit {

  current: any = null;
  plans: any[] = [];
  selectedPlan: any = null;
  showConfirm = false;
  loading = false;
  sent = false;
  errorMsg = '';

  constructor(private api: ApiService, private auth: AuthService, private cdr: ChangeDetectorRef, public router: Router) {}

  ngOnInit() {
    this.loadCurrent();
    this.loadPlans();
  }

  loadCurrent() {
    const userId = this.auth.getUserId();
    this.api.getActiveSubscription(userId).subscribe({
      next: (res: any) => {
        this.current = res.subscription ?? null;
        this.cdr.detectChanges();
      },
      error: () => this.current = null
    });
  }

  loadPlans() {
    this.api.getPlans().subscribe({
      next: res => {
        this.plans = [...res];
        this.cdr.detectChanges();
      },
      error: err => console.error('getPlans error:', err)
    });
  }

  selectPlan(plan: any) {
    this.selectedPlan = plan;
    this.showConfirm = true;
  }

  cancelConfirm() {
    this.selectedPlan = null;
    this.showConfirm = false;
  }

  confirmSubscribe() {
    this.loading = true;
    this.errorMsg = '';
    const userId = this.auth.getUserId();
    this.api.subscribe(userId, this.selectedPlan.planId).subscribe({
      next: () => {
        this.loading = false;
        this.showConfirm = false;
        this.sent = true;
        this.loadCurrent();
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.loading = false;
        this.errorMsg = err?.error || 'Subscription request failed.';
        this.cdr.detectChanges();
      }
    });
  }

  get trialDaysLeft(): number {
    if (!this.current?.expiryDate || this.current.status !== 'Approved') return 0;
    const diff = new Date(this.current.expiryDate).getTime() - Date.now();
    return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
  }

  get statusClass(): string {
    const s = this.current?.status?.toLowerCase();
    if (s === 'approved') return 'status-active';
    if (s === 'pending') return 'status-pending';
    if (s === 'expired' || s === 'rejected') return 'status-expired';
    return '';
  }
}
