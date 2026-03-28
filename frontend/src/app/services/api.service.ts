import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = 'http://localhost:5244/api';

  constructor(private http: HttpClient) {}

  // Items
  getItems(shopId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/items/shop/${shopId}`);
  }

  createItem(item: any): Observable<any> {
    return this.http.post(`${this.base}/items`, item);
  }

  updateItem(id: number, item: any): Observable<any> {
    return this.http.put(`${this.base}/items/${id}`, item);
  }

  deleteItem(id: number): Observable<any> {
    return this.http.delete(`${this.base}/items/${id}`);
  }

  getExpiringItems(shopId: number, days = 30): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/items/shop/${shopId}/expiring?days=${days}`);
  }

  // Transactions
  createTransaction(tx: any): Observable<any> {
    return this.http.post(`${this.base}/transactions`, tx);
  }

  getTransactionsByShop(shopId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/transactions/shop/${shopId}`);
  }

  // Profit & Loss
  getProfitLoss(shopId: number): Observable<any> {
    return this.http.get(`${this.base}/profitloss/${shopId}`);
  }

  getProfitLossFiltered(shopId: number, from: string, to: string): Observable<any> {
    return this.http.get(`${this.base}/profitloss/${shopId}/filter?from=${from}&to=${to}`);
  }

  // Shops
  getShopsByUser(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/shops/my`);
  }

  createShop(form: FormData): Observable<any> {
    return this.http.post(`${this.base}/shops`, form);
  }

  // Subscriptions
  getPlans(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/subscriptions/plans`);
  }

  subscribe(userId: string, planId: number): Observable<any> {
    return this.http.post(`${this.base}/subscriptions/users/${userId}/subscribe/${planId}`, {});
  }

  getActiveSubscription(userId: string): Observable<any> {
    return this.http.get(`${this.base}/subscriptions/users/${userId}/active`);
  }

  // Admin
  getDashboard(): Observable<any> {
    return this.http.get(`${this.base}/admin/dashboard`);
  }

  getAllUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/admin/users`);
  }

  getAllShops(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/admin/shops`);
  }

  getAllSubscriptions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/admin/subscriptions`);
  }

  getPendingSubscriptions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/subscriptions/pending`);
  }

  approveSubscription(id: number): Observable<any> {
    return this.http.put(`${this.base}/subscriptions/${id}/approve`, {});
  }

  rejectSubscription(id: number): Observable<any> {
    return this.http.put(`${this.base}/subscriptions/${id}/reject`, {});
  }

  createPlan(plan: any): Observable<any> {
    return this.http.post(`${this.base}/subscriptions/plans`, plan);
  }

  updatePlan(id: number, plan: any): Observable<any> {
    return this.http.put(`${this.base}/subscriptions/plans/${id}`, plan);
  }

  deletePlan(id: number): Observable<any> {
    return this.http.delete(`${this.base}/subscriptions/plans/${id}`);
  }

  // Profile
  getProfile(): Observable<any> {
    return this.http.get(`${this.base}/profile`);
  }

  updateProfile(form: FormData): Observable<any> {
    return this.http.put(`${this.base}/profile`, form);
  }

  cancelSubscription(): Observable<any> {
    return this.http.put(`${this.base}/profile/cancel-subscription`, {});
  }
}
