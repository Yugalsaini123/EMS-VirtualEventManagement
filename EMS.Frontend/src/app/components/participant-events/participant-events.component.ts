//src/app/components/participant-events/participant-events.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ParticipantService } from '../../services/participant.service';
import { ParticipantEvent } from '../../models/participant.model';

@Component({
  selector: 'app-participant-events',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './participant-events.component.html',
  styleUrls: ['./participant-events.component.css']
})
export class ParticipantEventsComponent implements OnInit {
  events: ParticipantEvent[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private participantService: ParticipantService) {}

  ngOnInit(): void {
    this.loadMyEvents();
  }

  loadMyEvents(): void {
    this.participantService.getMyEvents().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.events = (response.data || []).map((event: any) => ({
            registrationId: event.registrationId || event.RegistrationId || event.id || event.Id,
            eventId: event.eventId || event.EventId,
            eventName: event.eventName || event.EventName,
            eventDate: event.eventDate || event.EventDate,
            participantEmail: event.participantEmail || event.ParticipantEmail,
            registrationDate: event.registrationDate || event.RegistrationDate,
            isAttended: event.isAttended ?? event.IsAttended ?? false,
            attendanceDate: event.attendanceDate || event.AttendanceDate,
            rating: event.rating ?? event.Rating,
            feedback: event.feedback || event.Feedback || '',
            status: event.status || event.Status || 'Active'
          }));
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Failed to load your events';
      }
    });
  }

  unregister(eventId: string): void {
    if (confirm('Are you sure you want to unregister from this event?')) {
      this.participantService.unregisterFromEvent(eventId).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Unregistered successfully';
            this.loadMyEvents();
            setTimeout(() => {
              this.successMessage = null;
            }, 2000);
          }
        },
        error: (error) => {
          this.errorMessage = 'Failed to unregister from event';
        }
      });
    }
  }

  markAttended(registrationId: string): void {
  if (!registrationId) {
    this.errorMessage = 'Unable to mark attendance: Invalid registration.';
    return;
  }

  const registration = this.events.find(e => e.registrationId === registrationId);
  if (!registration) {
    this.errorMessage = 'Registration not found.';
    return;
  }

  if (confirm('Mark your attendance for this event?')) {
    this.participantService.markAttendance(registrationId).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = 'Attendance marked successfully!';
          this.loadMyEvents();
          setTimeout(() => { this.successMessage = null; }, 2000);
        }
      },
      error: (error) => {
        if (error.error?.errors && Array.isArray(error.error.errors)) {
          this.errorMessage = error.error.errors.join(', ');
        } else if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'Failed to mark attendance. Please try again.';
        }
      }
    });
  }
}

  isEventPassed(eventDate: Date): boolean {
    const now = new Date();
    return new Date(eventDate) < now;
  }

  canMarkAttendance(event: ParticipantEvent): boolean {
    return !event.isAttended;
  }

  canUnregister(event: ParticipantEvent): boolean {
    return !event.isAttended && event.status === 'Active' && !this.isEventPassed(event.eventDate);
  }
}