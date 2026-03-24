import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string;
  unique_name: string;
  role: string | string[];
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = 'http://localhost:5244/api/auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.base}/login`, { username, password }).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  register(data: any): Observable<any> {
    return this.http.post<any>(`${this.base}/register`, data).pipe(
      tap(res => localStorage.setItem('token', res.token))
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('shopId');
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
    return Array.isArray(decoded.role) ? decoded.role[0] : decoded.role;
  }

  getUserId(): string {
    const token = this.getToken();
    if (!token) return '';
    return jwtDecode<JwtPayload>(token).nameid;
  }

  getUsername(): string {
    const token = this.getToken();
    if (!token) return '';
    return jwtDecode<JwtPayload>(token).unique_name;
  }

  getShopId(): number { return Number(localStorage.getItem('shopId') || 0); }
  setShopId(id: number) { localStorage.setItem('shopId', id.toString()); }
}
