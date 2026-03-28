import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  stats: any = null;

  constructor(private api: ApiService, private cdr: ChangeDetectorRef, private router: Router) {}

  ngOnInit() {
    this.api.getDashboard().subscribe({
      next: res => { this.stats = res; this.cdr.detectChanges(); },
      error: err => console.error('Dashboard error', err)
    });
  }

  go(path: string) { this.router.navigate([path]); }
}
