import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private base = 'http://localhost:5244/api';

  constructor(private http: HttpClient) {}

  getSubscriptions() {
    return this.http.get<any[]>(`${this.base}/subscriptions/pending`);
  }

  approveSubscription(id: number) {
    return this.http.put(`${this.base}/subscriptions/${id}/approve`, {});
  }

  rejectSubscription(id: number) {
    return this.http.put(`${this.base}/subscriptions/${id}/reject`, {});
  }
}