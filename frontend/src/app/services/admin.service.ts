import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private baseUrl = 'https://localhost:5001/api/admin';

  constructor(private http: HttpClient) {}

  // Get all subscriptions
  getSubscriptions() {
    return this.http.get(`${this.baseUrl}/usersubscriptions`);
  }

  // Approve subscription (you can customize API later)
  approveSubscription(id: number) {
    return this.http.put(`${this.baseUrl}/approve/${id}`, {});
  }
}