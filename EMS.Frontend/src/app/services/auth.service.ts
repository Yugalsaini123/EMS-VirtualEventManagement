// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, LoginResponse, AuthResponse, User } from '../models/user.model';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl: string;
  private tokenSubject = new BehaviorSubject<string | null>(this.getToken());
  public token$ = this.tokenSubject.asObservable();
  private userSubject = new BehaviorSubject<User | null>(null);
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = `${this.config.getApiUrl()}/auth`;
    this.loadUserFromToken();
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        if (response && response.success && response.data?.accessToken) {
          localStorage.setItem('token', response.data.accessToken);
          this.tokenSubject.next(response.data.accessToken);
          this.extractUserFromToken(response.data.accessToken);
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request).pipe(
      tap(response => {
        console.log('Registration successful:', response);
      })
    );
  }

  refreshToken(): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/refresh-token`, {}).pipe(
      tap(response => {
        if (response && response.success && response.data?.accessToken) {
          localStorage.setItem('token', response.data.accessToken);
          this.tokenSubject.next(response.data.accessToken);
          this.extractUserFromToken(response.data.accessToken);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.tokenSubject.next(null);
    this.userSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): User | null {
    return this.userSubject.value;
  }

  isAdmin(): boolean {
    const user = this.userSubject.value;
    return user?.role === 'Admin';
  }

  isParticipant(): boolean {
    const user = this.userSubject.value;
    return user?.role === 'Participant';
  }

  hasRole(role: string): boolean {
    const user = this.userSubject.value;
    return user?.role === role;
  }

  private extractUserFromToken(token: string): void {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // Try multiple possible claim names for role
      const role = payload.Role || payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Participant';
      const user: User = {
        emailId: payload.email || payload.sub || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        userName: payload.name || payload.unique_name || 'User',
        role: role,
        isActive: true,
        createdAt: new Date()
      };
      this.userSubject.next(user);
    } catch (error) {
      console.error('Error extracting user from token:', error);
    }
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (token) this.extractUserFromToken(token);
  }
}