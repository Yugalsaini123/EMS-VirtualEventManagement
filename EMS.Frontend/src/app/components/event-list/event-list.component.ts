// src/app/components/event-list/event-list.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { EventService } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';
import { Event } from '../../models/event.model';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css']
})
export class EventListComponent implements OnInit {
  events: Event[] = [];
  loading = true;
  errorMessage = '';
  successMessage = '';
  currentPage = 1;
  pageSize = 10;
  totalPages = 1;
  isAdmin = false;
  searchText = '';
  sortBy = 'date';
  sortOrder = 'asc';

  constructor(
    private eventService: EventService,
    private authService: AuthService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    this.isAdmin = this.authService.isAdmin();
    await this.loadEvents();
  }

  loadEvents(): Promise<void> {
    return new Promise((resolve) => {
      this.loading = true;
      this.eventService.getAllEvents(this.currentPage, this.pageSize, this.searchText, this.sortBy, this.sortOrder).subscribe({
        next: (response) => {
          if (response.success) {
            this.events = (response.data.data || []).map((event: any) => {
              const status = event.status || event.Status || 'Active';

              return {
                eventId: event.eventId || event.EventId,
                eventName: event.eventName || event.EventName,
                eventCategory: event.eventCategory || event.EventCategory,
                eventDate: event.eventDate || event.EventDate,
                description: event.description || event.Description,
                status: status,
                location: event.location || event.Location,
                maxParticipants: event.maxParticipants ?? event.MaxParticipants,
                participantCount: event.participantCount ?? event.ParticipantCount
              };
            });
            this.totalPages = response.data.totalPages;
          }
          this.loading = false;
          resolve();
        },
        error: (err) => {
          this.errorMessage = 'Failed to load events';
          this.loading = false;
          resolve();
        }
      });
    });
  }

  editEvent(eventId: string): void {
    this.router.navigate(['/admin/event-form', eventId]);
  }

  deleteEvent(eventId: string): void {
    if (confirm('Are you sure you want to delete this event?')) {
      this.eventService.deleteEvent(eventId).subscribe({
        next: (res) => {
          if (res.success) {
            this.successMessage = 'Event deleted successfully';
            this.loadEvents();
            setTimeout(() => (this.successMessage = ''), 3000);
          }
        },
        error: () => (this.errorMessage = 'Delete failed')
      });
    }
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadEvents();
  }

  onSortChange(): void {
    this.currentPage = 1;
    this.loadEvents();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadEvents();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadEvents();
    }
  }
}
