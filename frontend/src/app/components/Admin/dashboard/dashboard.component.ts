import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  subscriptions: any[] = [];

  private apiUrl = 'https://localhost:5001/api/admin/usersubscriptions';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.getSubscriptions();
  }

  getSubscriptions() {
    this.http.get<any[]>(this.apiUrl).subscribe({
      next: (res) => {
        this.subscriptions = res;
      },
      error: (err) => {
        console.error('Error fetching subscriptions', err);
      }
    });
  }
}