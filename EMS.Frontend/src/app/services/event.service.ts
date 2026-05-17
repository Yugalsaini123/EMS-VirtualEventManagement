import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEventRequest, UpdateEventRequest, EventListResponse, EventResponse } from '../models/event.model';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class EventService {
  private apiUrl: string;

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = `${this.config.getApiUrl()}/events`;
  }

  getAllEvents(pageNumber: number = 1, pageSize: number = 10, search: string = '', sortBy: string = 'date', sortOrder: string = 'asc'): Observable<EventListResponse> {
    let query = `${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    if (search) {
      query += `&search=${encodeURIComponent(search)}`;
    }
    if (sortBy) {
      query += `&sortBy=${encodeURIComponent(sortBy)}`;
    }
    if (sortOrder) {
      query += `&sortOrder=${encodeURIComponent(sortOrder)}`;
    }
    return this.http.get<EventListResponse>(query);
  }

  // Get all events including inactive (Admin only)
  getAllEventsAdmin(): Observable<EventListResponse> {
    return this.http.get<EventListResponse>(`${this.apiUrl}/admin/all`);
  }

  getEventById(eventId: string): Observable<EventResponse> {
    return this.http.get<EventResponse>(`${this.apiUrl}/${eventId}`);
  }

  createEvent(event: CreateEventRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, event);
  }

  updateEvent(eventId: string, event: UpdateEventRequest): Observable<any> {
    // Send only the event fields, eventId is in the URL
    const { eventId: _, ...eventData } = event;
    return this.http.put<any>(`${this.apiUrl}/${eventId}`, eventData);
  }

  deleteEvent(eventId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${eventId}`);
  }

  // Toggle event status between Active/Inactive (Admin only)
  toggleEventStatus(eventId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${eventId}/toggle-status`, {});
  }

  getCategories(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/categories/all`);
  }
}