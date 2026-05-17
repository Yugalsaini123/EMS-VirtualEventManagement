//src/app/services/session.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateSessionRequest, SessionListResponse } from '../models/session.model';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private apiUrl: string;

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = `${this.config.getApiUrl()}/sessions`;
  }

  getSessionsByEvent(eventId: string): Observable<SessionListResponse> {
    return this.http.get<SessionListResponse>(`${this.apiUrl}/event/${eventId}`);
  }

  searchSessionsByEvent(eventId: string, searchTerm: string): Observable<SessionListResponse> {
    let query = `${this.apiUrl}/event/${eventId}/search`;
    if (searchTerm) {
      query += `?searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    return this.http.get<SessionListResponse>(query);
  }

  getSessionById(sessionId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${sessionId}`);
  }

  createSession(session: CreateSessionRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, session);
  }

  updateSession(sessionId: string, session: CreateSessionRequest): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${sessionId}`, session);
  }

  deleteSession(sessionId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${sessionId}`);
  }

  assignSpeaker(sessionId: string, speakerId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${sessionId}/assign-speaker`, { sessionId, speakerId });
  }

  removeSpeaker(sessionId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${sessionId}/remove-speaker`, {});
  }
}

