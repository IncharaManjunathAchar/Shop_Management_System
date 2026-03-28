import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-subscriptions',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './subscriptions.component.html',
  styleUrl: './subscriptions.component.css'
})
export class SubscriptionsComponent implements OnInit {

  all: any[] = [];
  filter: string = 'all';
  search = '';
  selected: any = null;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() { this.load(); }

  load() {
    this.api.getAllSubscriptions().subscribe({
      next: res => {
        console.log('subscriptions:', res);
        this.all = [...res]; this.cdr.detectChanges();
      },
      error: err => console.error('subscriptions error:', err)
    });
  }

  get stats() {
    return {
      active:  this.all.filter(s => s.status === 'Approved').length,
      pending: this.all.filter(s => s.status === 'Pending').length,
      revenue: this.all.filter(s => s.status === 'Approved').reduce((sum, s) => sum + (s.price ?? 0), 0)
    };
  }

  get filtered() {
    return this.all.filter(s => {
      const matchFilter = this.filter === 'all' || s.status?.toLowerCase() === this.filter;
      const matchSearch = !this.search ||
        s.username?.toLowerCase().includes(this.search.toLowerCase()) ||
        s.planName?.toLowerCase().includes(this.search.toLowerCase());
      return matchFilter && matchSearch;
    });
  }

  approve(id: number) {
    this.api.approveSubscription(id).subscribe({ next: () => this.load(), error: err => console.error(err) });
  }

  reject(id: number) {
    this.api.rejectSubscription(id).subscribe({ next: () => this.load(), error: err => console.error(err) });
  }

  badgeClass(status: string) {
    const s = status?.toLowerCase();
    if (s === 'approved') return 'status-approved';
    if (s === 'pending')  return 'status-pending';
    return 'status-expired';
  }

  select(s: any) { this.selected = s; }
  close() { this.selected = null; }
}
