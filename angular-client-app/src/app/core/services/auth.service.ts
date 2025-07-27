import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';
import { API_ENDPOINTS } from '../../shared/constants/api-endpoints';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'access_token';
  private refreshKey = 'refresh_token';
  private readonly _roleKey = 'user_role';

  get roleKey(): string {
    return this._roleKey;
  }

  isLoggedIn$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {
    this.isLoggedIn$.next(!!localStorage.getItem(this.tokenKey));
  }

  login(credentials: { username: string; password: string }) {
    return this.http.post<any>(API_ENDPOINTS.auth.login, credentials).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.accessToken);
        localStorage.setItem(this.refreshKey, res.refreshToken);
        localStorage.setItem(this.roleKey, res.role);
        this.isLoggedIn$.next(true);
      })
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshKey);
    localStorage.removeItem(this.roleKey);
    this.isLoggedIn$.next(false);
  }

  getAccessToken() {
    return localStorage.getItem(this.tokenKey);
  }

  getCurrentUser() {
    const token = this.getAccessToken();
    const role = localStorage.getItem(this.roleKey);
    return token ? { token, role } : null;
  }

  refreshToken() {
    const refreshToken = localStorage.getItem(this.refreshKey);
    return this.http.post<any>(API_ENDPOINTS.auth.refresh, { refreshToken }).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.accessToken);
      })
    );
  }
}
