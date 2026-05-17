//src/app/components/dashboard/dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EventService } from '../../services/event.service';
import { SpeakerService } from '../../services/speaker.service';
import { ParticipantService } from '../../services/participant.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  totalEvents = 0;
  upcomingEvents = 0;
  totalSpeakers = 0;
  totalParticipants = 0;
  loading = true;
  error: string | null = null;

  // Admin features
  recentEvents: any[] = [];
  speakersWorkload: any[] = [];
  eventStats: any = null;

  constructor(
    private eventService: EventService,
    private speakerService: SpeakerService,
    private participantService: ParticipantService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    // Load all events
    this.eventService.getAllEvents(1, 100).subscribe({
      next: (res) => {
        if (res && res.success && res.data) {
          this.totalEvents = res.data.totalCount;
          const now = new Date();
          this.upcomingEvents = res.data.data.filter(e => new Date(e.eventDate) > now).length;
          this.recentEvents = res.data.data.slice(0, 5);
        }
        this.checkLoadingComplete();
      },
      error: (err) => {
        console.error('Error loading events:', err);
        this.checkLoadingComplete();
      }
    });

    // Load admin: all events (including inactive)
    this.eventService.getAllEventsAdmin().subscribe({
      next: (res) => {
        console.log('Admin events loaded:', res);
        this.checkLoadingComplete();
      },
      error: (err) => {
        console.error('Error loading admin events:', err);
        this.checkLoadingComplete();
      }
    });

    // Load speakers with workload
    this.speakerService.getSpeakersWithWorkload().subscribe({
      next: (res) => {
        if (res && res.success && res.data) {
          this.speakersWorkload = res.data;
          this.totalSpeakers = res.data.length;
        }
        this.checkLoadingComplete();
      },
      error: (err) => {
        console.error('Error loading speaker workload:', err);
        this.checkLoadingComplete();
      }
    });

    // Load all participants
    this.userService.getAllParticipants().subscribe({
      next: (res) => {
        if (res && res.success && res.data) {
          this.totalParticipants = res.data.length;
        }
        this.checkLoadingComplete();
      },
      error: (err) => {
        console.error('Error loading participants:', err);
        this.checkLoadingComplete();
      }
    });

    // Load stats for first event (if available)
    if (this.recentEvents.length > 0) {
      this.participantService.getAttendanceStats(this.recentEvents[0].eventId).subscribe({
        next: (res) => {
          if (res && res.success && res.data) {
            this.eventStats = res.data;
          }
          this.checkLoadingComplete();
        },
        error: (err) => {
          console.error('Error loading event stats:', err);
          this.checkLoadingComplete();
        }
      });
    } else {
      this.checkLoadingComplete();
    }
  }

  private loadingCount = 0;
  private totalLoadingTasks = 5;

  private checkLoadingComplete(): void {
    this.loadingCount++;
    if (this.loadingCount >= this.totalLoadingTasks) {
      this.loading = false;
    }
  }

  toggleEventStatus(eventId: string): void {
    this.eventService.toggleEventStatus(eventId).subscribe({
      next: (res) => {
        if (res && res.success) {
          alert('Event status toggled successfully');
          this.loadDashboardData();
        }
      },
      error: (err) => {
        console.error('Error toggling event status:', err);
        alert('Failed to toggle event status');
      }
    });
  }
}
