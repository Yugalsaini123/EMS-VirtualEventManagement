import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangePasswordRequest } from '../models/user.model';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl: string;

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = `${this.config.getApiUrl()}/users`;
  }

  changePassword(email: string, request: ChangePasswordRequest): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${encodeURIComponent(email)}/change-password`, request);
  }

  // Get all participants (Admin only)
  getAllParticipants(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/participants/all`);
  }

  // Get users by role (Admin only)
  getUsersByRole(role: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?role=${encodeURIComponent(role)}`);
  }
}
