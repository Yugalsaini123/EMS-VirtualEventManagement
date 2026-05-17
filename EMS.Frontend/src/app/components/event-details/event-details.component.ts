//src/app/components/event-details/event-details.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EventService } from '../../services/event.service';
import { ParticipantService } from '../../services/participant.service';
import { SessionService } from '../../services/session.service';
import { AuthService } from '../../services/auth.service';
import { Event } from '../../models/event.model';
import { Session } from '../../models/session.model';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.css']
})
export class EventDetailsComponent implements OnInit {
  event: Event | null = null;
  sessions: Session[] = [];
  loading = true;
  loadingSession = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isRegistered = false;
  eventId: string = '';

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private sessionService: SessionService,
    private participantService: ParticipantService,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.eventId = params['id'];
      this.loadEventDetails();
      this.loadSessions();
      if (this.authService.isAuthenticated()) {
        this.checkRegistrationStatus();
      }
    });
  }

  loadEventDetails(): void {
    this.eventService.getEventById(this.eventId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.event = response.data;
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Failed to load event details';
      }
    });
  }

  loadSessions(): void {
    this.loadingSession = true;
    this.sessionService.getSessionsByEvent(this.eventId).subscribe({
      next: (response) => {
        this.loadingSession = false;
        if (response.success) {
          this.sessions = response.data;
        }
      },
      error: (error) => {
        this.loadingSession = false;
      }
    });
  }

  checkRegistrationStatus(): void {
    this.participantService.checkRegistration(this.eventId).subscribe({
      next: (response) => {
        if (response.success) {
          this.isRegistered = response.data.isRegistered;
        }
      },
      error: () => {
        this.isRegistered = false;
      }
    });
  }

  isEventInactive(): boolean {
    if (!this.event) return false;
    return this.event.status === 'Inactive';
  }

  registerForEvent(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: `/event-details/${this.eventId}` } 
      });
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    this.participantService.registerForEvent(this.eventId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.successMessage = 'Successfully registered for the event!';
          this.isRegistered = true;
          this.loadEventDetails();
          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        }
      },
      error: (error) => {
        this.loading = false;
        if (error.error?.errors && Array.isArray(error.error.errors)) {
          this.errorMessage = error.error.errors.join(', ');
        } else if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors && typeof error.error.errors === 'object') {
          this.errorMessage = Object.values(error.error.errors).flat().join(', ');
        } else {
          this.errorMessage = 'Failed to register for event. Please try again.';
        }
      }
    });
  }

  unregisterFromEvent(): void {
    if (confirm('Are you sure you want to unregister from this event?')) {
      this.loading = true;
      this.errorMessage = null;

      this.participantService.unregisterFromEvent(this.eventId).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            this.successMessage = 'Successfully unregistered from the event';
            this.isRegistered = false;
            this.loadEventDetails();
            setTimeout(() => {
              this.successMessage = null;
            }, 3000);
          }
        },
        error: (error) => {
          this.loading = false;
          if (error.error?.errors && Array.isArray(error.error.errors)) {
            this.errorMessage = error.error.errors.join(', ');
          } else if (error.error?.message) {
            this.errorMessage = error.error.message;
          } else if (error.error?.errors && typeof error.error.errors === 'object') {
            this.errorMessage = Object.values(error.error.errors).flat().join(', ');
          } else {
            this.errorMessage = 'Failed to unregister from event. Please try again.';
          }
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/home']);
  }
}
