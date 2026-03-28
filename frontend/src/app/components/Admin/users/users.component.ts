import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {

  users: any[] = [];
  filter: 'all' | 'active' | 'pending' | 'none' = 'all';
  search = '';
  selected: any = null;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef) {}

  ngOnInit() { this.load(); }

  load() {
    this.api.getAllUsers().subscribe({
      next: res => { this.users = [...res]; this.cdr.detectChanges(); },
      error: err => console.error(err)
    });
  }

  get filtered() {
    return this.users.filter(u => {
      const matchSearch = !this.search ||
        u.userName?.toLowerCase().includes(this.search.toLowerCase()) ||
        u.email?.toLowerCase().includes(this.search.toLowerCase());
      const status = u.subscription?.status?.toLowerCase() ?? 'none';
      const matchFilter =
        this.filter === 'all' ||
        (this.filter === 'active' && status === 'approved') ||
        (this.filter === 'pending' && status === 'pending') ||
        (this.filter === 'none' && status === 'none');
      return matchSearch && matchFilter;
    });
  }

  badgeClass(status: string) {
    const s = status?.toLowerCase();
    if (s === 'approved') return 'status-approved';
    if (s === 'pending') return 'status-pending';
    if (s === 'expired' || s === 'rejected') return 'status-expired';
    return 'status-none';
  }

  select(u: any) { this.selected = u; }
  close() { this.selected = null; }
}
