import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string;
  unique_name: string;
  role: string | string[];
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string | string[];
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = 'http://localhost:5244/api/auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.base}/login`, { username, password }).pipe(
      tap(res => {
        localStorage.removeItem('shopLogoUrl');
        localStorage.setItem('token', res.token);
      })
    );
  }

  register(data: any): Observable<any> {
    return this.http.post<any>(`${this.base}/register`, data);
  }

  verifyEmail(email: string, otp: string): Observable<any> {
    return this.http.post<any>(`${this.base}/verify-email`, { email, otp }).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('shopId');
    localStorage.removeItem('shopLogoUrl');
    this.router.navigate(['/login']);
  }

  getToken(): string | null { return localStorage.getItem('token'); }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return decoded.exp * 1000 > Date.now();
    } catch { return false; }
  }

  getRole(): string {
    const token = this.getToken();
    if (!token) return '';
    const decoded = jwtDecode<JwtPayload>(token);
    const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? decoded.role;
    return Array.isArray(role) ? role[0] : (role ?? '');
  }

  getUserId(): string {
    const token = this.getToken();
    if (!token) return '';
    const decoded = jwtDecode<JwtPayload>(token);
    return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? decoded.nameid ?? '';
  }

  getUsername(): string {
    const token = this.getToken();
    if (!token) return '';
    const decoded = jwtDecode<JwtPayload>(token);
    return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? decoded.unique_name ?? '';
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>(`${this.base}/forgot-password`, { email });
  }

  resetPassword(email: string, otp: string, newPassword: string): Observable<any> {
    return this.http.post<any>(`${this.base}/reset-password`, { email, otp, newPassword });
  }

  getShopId(): number { return Number(localStorage.getItem('shopId') || 0); }
  setShopId(id: number) { localStorage.setItem('shopId', id.toString()); }
}
