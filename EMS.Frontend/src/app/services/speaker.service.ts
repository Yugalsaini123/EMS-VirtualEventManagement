//src/app/services/speaker.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateSpeakerRequest, SpeakerListResponse, SpeakerWorkloadDto } from '../models/speaker.model';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class SpeakerService {
  private apiUrl: string;

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = `${this.config.getApiUrl()}/speakers`;
  }

  getAllSpeakers(): Observable<SpeakerListResponse> {
    return this.http.get<SpeakerListResponse>(this.apiUrl);
  }

  getSpeakerById(speakerId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${speakerId}`);
  }

  createSpeaker(speaker: CreateSpeakerRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, speaker);
  }

  updateSpeaker(speakerId: string, speaker: CreateSpeakerRequest): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${speakerId}`, speaker);
  }

  deleteSpeaker(speakerId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${speakerId}`);
  }

  getActiveSpeakers(): Observable<SpeakerListResponse> {
    return this.http.get<SpeakerListResponse>(`${this.apiUrl}/active/all`);
  }

  searchSpeakers(name: string): Observable<SpeakerListResponse> {
    return this.http.get<SpeakerListResponse>(`${this.apiUrl}/search/${name}`);
  }

  // Get all speakers with their session count/workload (Admin only)
  getSpeakersWithWorkload(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/workload/all`);
  }
}
