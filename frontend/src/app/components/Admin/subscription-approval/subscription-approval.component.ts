import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../services/admin.service';

@Component({
  selector: 'app-subscription-approval',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './subscription-approval.component.html'
})
export class SubscriptionApprovalComponent implements OnInit {

  subscriptions: any[] = [];

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadSubscriptions();
  }

  loadSubscriptions() {
    this.adminService.getSubscriptions().subscribe((res: any) => {
      this.subscriptions = res;
    });
  }

  approve(id: number) {
    this.adminService.approveSubscription(id).subscribe(() => {
      alert('Approved!');
      this.loadSubscriptions();
    });
  }
}