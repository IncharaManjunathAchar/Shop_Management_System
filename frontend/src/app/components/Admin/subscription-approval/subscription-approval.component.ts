import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../services/admin.service';

@Component({
  selector: 'app-subscription-approval',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './subscription-approval.component.html',
  styleUrl: './subscription-approval.component.css'
})
export class SubscriptionApprovalComponent implements OnInit {

  subscriptions: any[] = [];

  constructor(private adminService: AdminService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadSubscriptions();
  }

  loadSubscriptions() {
    this.adminService.getSubscriptions().subscribe({
      next: res => { this.subscriptions = [...res]; this.cdr.detectChanges(); },
      error: err => console.error('Load subscriptions error', err)
    });
  }

  approve(id: number) {
    this.adminService.approveSubscription(id).subscribe(() => this.loadSubscriptions());
  }

  reject(id: number) {
    this.adminService.rejectSubscription(id).subscribe(() => this.loadSubscriptions());
  }
}