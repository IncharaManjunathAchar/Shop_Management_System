import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = 'http://localhost:5244/api';

  constructor(private http: HttpClient) {}

  private headers() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  // Items
  getItems(shopId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/items/shop/${shopId}`, { headers: this.headers() });
  }

  createItem(item: any): Observable<any> {
    return this.http.post(`${this.base}/items`, item, { headers: this.headers() });
  }

  updateItem(id: number, item: any): Observable<any> {
    return this.http.put(`${this.base}/items/${id}`, item, { headers: this.headers() });
  }

  deleteItem(id: number): Observable<any> {
    return this.http.delete(`${this.base}/items/${id}`, { headers: this.headers() });
  }

  getExpiringItems(shopId: number, days = 30): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/items/shop/${shopId}/expiring?days=${days}`, { headers: this.headers() });
  }

  // Transactions
  createTransaction(tx: any): Observable<any> {
    return this.http.post(`${this.base}/transactions`, tx, { headers: this.headers() });
  }

  getTransactionsByShop(shopId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/transactions/shop/${shopId}`, { headers: this.headers() });
  }

  // Profit & Loss
  getProfitLoss(shopId: number): Observable<any> {
    return this.http.get(`${this.base}/profitloss/${shopId}`, { headers: this.headers() });
  }

  getProfitLossFiltered(shopId: number, from: string, to: string): Observable<any> {
    return this.http.get(`${this.base}/profitloss/${shopId}/filter?from=${from}&to=${to}`, { headers: this.headers() });
  }

  // Shops
  getShopsByUser(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/shops/my`, { headers: this.headers() });
  }

  // Subscriptions
  getPlans(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/subscriptions/plans`, { headers: this.headers() });
  }

  subscribe(userId: string, planId: number): Observable<any> {
    return this.http.post(`${this.base}/subscriptions/users/${userId}/subscribe/${planId}`, {}, { headers: this.headers() });
  }

  getActiveSubscription(userId: string): Observable<any> {
    return this.http.get(`${this.base}/subscriptions/users/${userId}/active`, { headers: this.headers() });
  }
}
