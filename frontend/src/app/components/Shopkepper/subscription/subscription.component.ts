import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../services/api.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-subscription',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './subscription.component.html',
  styleUrl: './subscription.component.css'
})
export class SubscriptionComponent implements OnInit {

  current: any = null;
  plans: any[] = [];
  selectedPlan: any = null;
  showConfirm = false;
  loading = false;

  constructor(private api: ApiService, private auth: AuthService) {}

  ngOnInit() {
    this.loadCurrent();
    this.loadPlans();
  }

  loadCurrent() {
    const userId = this.auth.getUserId();
    this.api.getActiveSubscription(userId).subscribe({
      next: res => this.current = res,
      error: () => this.current = null
    });
  }

  loadPlans() {
    this.api.getPlans().subscribe(res => this.plans = res);
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
    const userId = this.auth.getUserId();
    this.api.subscribe(userId, this.selectedPlan.id).subscribe({
      next: () => {
        this.showConfirm = false;
        this.loading = false;
        this.loadCurrent();
      },
      error: () => this.loading = false
    });
  }

  get trialDaysLeft(): number {
    if (!this.current?.expiryDate) return 0;
    const diff = new Date(this.current.expiryDate).getTime() - Date.now();
    return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
  }

  get statusClass(): string {
    const s = this.current?.status?.toLowerCase();
    if (s === 'active') return 'status-active';
    if (s === 'pending') return 'status-pending';
    if (s === 'expired' || s === 'rejected') return 'status-expired';
    return '';
  }
}
