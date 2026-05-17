import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class ParticipantService {
  private apiUrl: string;

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = `${this.config.getApiUrl()}/participants`;
  }

  registerForEvent(eventId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, { eventId });
  }

  getMyEvents(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/my-events`);
  }

  unregisterFromEvent(eventId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/unregister`, { eventId });
  }

  markAttendance(registrationId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${registrationId}/mark-attendance`, {});
  }

  submitFeedback(registrationId: string, rating: number, feedback: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${registrationId}/feedback`, {
      registrationId,
      rating,
      feedback
    });
  }

  getEventParticipants(eventId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/event/${eventId}/participants`);
  }

  checkRegistration(eventId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/check-registration/${eventId}`);
  }

  // Get attendance statistics for event (Admin only)
  getAttendanceStats(eventId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/event/${eventId}/stats`);
  }
}

