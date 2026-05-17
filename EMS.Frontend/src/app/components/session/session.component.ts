//src/app/components/session/session.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SessionService } from '../../services/session.service';
import { EventService } from '../../services/event.service';
import { SpeakerService } from '../../services/speaker.service';
import { Session } from '../../models/session.model';
import { Event } from '../../models/event.model';
import { Speaker } from '../../models/speaker.model';

@Component({
  selector: 'app-session',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './session.component.html',
  styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit {
  sessions: Session[] = [];
  events: Event[] = [];
  speakers: Speaker[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  selectedEventId: string = '';
  sessionForm: FormGroup;
  showForm = false;
  submitted = false;
  searchTerm: string = '';
  eventSearchTerm: string = '';
  filteredEvents: Event[] = [];

  constructor(
    private sessionService: SessionService,
    private eventService: EventService,
    private speakerService: SpeakerService,
    private formBuilder: FormBuilder
  ) {
    this.sessionForm = this.formBuilder.group({
      eventId: ['', Validators.required],
      title: ['', [Validators.required, Validators.minLength(3)]],         
      description: ['', [Validators.required, Validators.minLength(10)]],
      startTime: ['', Validators.required],                               
      endTime: ['', Validators.required],                                 
      location: ['', Validators.required],
      speakerId: [''],
      sessionUrl: ['']                               
    });
  }

  ngOnInit(): void {
    this.loadEvents();
    this.loadSpeakers();
  }

  loadEvents(): void {
    this.eventService.getAllEvents(1, 100).subscribe({
      next: (response) => {
        if (response.success) {
          this.events = response.data.data;
          this.filteredEvents = this.events;
          this.loading = false;
          
          // Auto-select first active event and load its sessions
          if (this.events && this.events.length > 0) {
            const activeEvent = this.events.find(e => e.status === 'Active') || this.events[0];
            if (activeEvent) {
              this.selectedEventId = activeEvent.eventId;
              // Set the form value to reflect the selection
              this.sessionForm.patchValue({ eventId: activeEvent.eventId });
              this.loadSessions();
            }
          }
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Failed to load events';
      }
    });
  }

  filterEvents(): void {
    if (!this.eventSearchTerm.trim()) {
      this.filteredEvents = this.events;
    } else {
      this.filteredEvents = this.events.filter(event =>
        event.eventName.toLowerCase().includes(this.eventSearchTerm.toLowerCase()) ||
        event.eventCategory.toLowerCase().includes(this.eventSearchTerm.toLowerCase())
      );
    }
  }

  onEventSearchChange(): void {
    this.filterEvents();
  }

  onEventSelected(event: any): void {
    const selectedEventName = event.target.value;
    const selectedEvent = this.events.find(e => e.eventName === selectedEventName);
    if (selectedEvent) {
      this.selectedEventId = selectedEvent.eventId;
      this.loadSessions();
    } else {
      this.selectedEventId = '';
      this.sessions = [];
    }
  }

  clearEventSelection(): void {
    this.selectedEventId = '';
    this.eventSearchTerm = '';
    this.sessions = [];
    this.filterEvents();
  }

  loadSessions(): void {
    if (!this.selectedEventId) return;
 
    this.sessionService.getSessionsByEvent(this.selectedEventId).subscribe({
      next: (response) => {
        if (response.success) {
          this.sessions = response.data;
          this.searchTerm = '';
        }
      },
      error: (error) => {
        this.errorMessage = 'Failed to load sessions';
      }
    });
  }

  searchSessions(): void {
    if (!this.selectedEventId) return;

    if (!this.searchTerm.trim()) {
      this.loadSessions();
      return;
    }

    this.sessionService.searchSessionsByEvent(this.selectedEventId, this.searchTerm).subscribe({
      next: (response) => {
        if (response.success) {
          this.sessions = response.data;
        }
      },
      error: (error) => {
        this.errorMessage = 'Failed to search sessions';
      }
    });
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.loadSessions();
  }

  loadSpeakers(): void {
    this.speakerService.getAllSpeakers().subscribe({
      next: (response) => {
        if (response.success) {
          this.speakers = response.data;
        }
      },
      error: (error) => {}
    });
  }

  deleteSession(sessionId: string): void {
    if (confirm('Are you sure you want to delete this session?')) {
      this.sessionService.deleteSession(sessionId).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Session deleted successfully';
            this.loadSessions();
            setTimeout(() => {
              this.successMessage = null;
            }, 2000);
          }
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete session';
        }
      });
    }
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = null;
    
    if (this.sessionForm.invalid) {
      this.errorMessage = 'Please fill all required fields correctly.';
      return;
    }
 
    const formValue = this.sessionForm.value;
    const startTime = new Date(formValue.startTime);
    const endTime = new Date(formValue.endTime);
    const now = new Date();
 
    if (startTime <= now) {
      this.errorMessage = 'Session start time must be in the future.';
      return;
    }
 
    if (startTime >= endTime) {
      this.errorMessage = 'End time must be after start time.';
      return;
    }
 
    const durationMinutes = (endTime.getTime() - startTime.getTime()) / (1000 * 60);
    if (durationMinutes < 15) {
      this.errorMessage = 'Session must be at least 15 minutes long.';
      return;
    }
 
    if (durationMinutes > 480) {
      this.errorMessage = 'Session cannot be longer than 8 hours.';
      return;
    }
 
    formValue.startTime = startTime;
    formValue.endTime = endTime;
 
    if (!formValue.sessionUrl || formValue.sessionUrl.trim() === '') {
      formValue.sessionUrl = null;
    }
 
    this.sessionService.createSession(formValue).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = 'Session created successfully!';
          this.sessionForm.reset();
          this.showForm = false;
          this.submitted = false;
          this.loadSessions();
          setTimeout(() => {
            this.successMessage = null;
          }, 2000);
        }
      },
      error: (error) => {
        if (error.error?.errors && Array.isArray(error.error.errors)) {
          this.errorMessage = error.error.errors.join(', ');
        } else if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors && typeof error.error?.errors === 'object') {
          this.errorMessage = Object.values(error.error.errors).flat().join(', ');
        } else {
          this.errorMessage = 'Failed to create session. Please try again.';
        }
      }
    });
  }

  get f() {
    return this.sessionForm.controls;
  }
}
