//src/app/components/home/home.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { Event } from '../../models/event.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  events: Event[] = [];
  loading = false;
  errorMessage: string | null = null;
  currentPage = 1;
  pageSize = 6;
  totalPages = 1;
  searchText = '';
  sortBy = 'date';
  sortOrder = 'asc';

  constructor(
    private eventService: EventService,
    public authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.loading = true;
    this.errorMessage = null;

    this.eventService.getAllEvents(this.currentPage, this.pageSize, this.searchText, this.sortBy, this.sortOrder).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          const normalizedEvents = (response.data.data || []).map((event: any) => {
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

          this.events = this.authService.isAdmin()
            ? normalizedEvents
            : normalizedEvents.filter(e => e.status === 'Active');

          this.totalPages = response.data.totalPages;
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Failed to load events. Please try again.';
      }
    });
  }

  viewDetails(eventId: string): void {
    this.router.navigate(['/event-details', eventId]);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadEvents();
      window.scrollTo(0, 0);
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadEvents();
      window.scrollTo(0, 0);
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

  getEventDate(date: any): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
