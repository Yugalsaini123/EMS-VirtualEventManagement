//src/app/components/event-form/event-form.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EventService } from '../../services/event.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css']
})
export class EventFormComponent implements OnInit {
  eventForm: FormGroup;
  loading = false;
  submitted = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  isEditMode = false;
  eventId: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    public router: Router
  ) {
    this.eventForm = this.formBuilder.group({
      eventName: ['', [Validators.required, Validators.minLength(3)]],
      eventCategory: ['', Validators.required],
      eventDate: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(10)]],
      location: ['', Validators.required],
      maxParticipants: ['', [Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.eventId = params['id'];
        this.loadEventData();
      }
    });
  }

  loadEventData(): void {
    if (this.eventId) {
      this.eventService.getEventById(this.eventId).subscribe({
        next: (response) => {
          if (response.success) {
            const event = response.data;
            this.eventForm.patchValue({
              eventName: event.eventName,
              eventCategory: event.eventCategory,
              eventDate: this.toDateTimeLocalString(event.eventDate),
              description: event.description,
              location: event.location,
              maxParticipants: event.maxParticipants
            });
          }
        },
        error: (error) => {
          this.errorMessage = 'Failed to load event data';
        }
      });
    }
  }

  get f() {
    return this.eventForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = null;
    this.successMessage = null;

    if (this.eventForm.invalid) {
      this.errorMessage = 'Please fill all required fields correctly.';
      return;
    }

    this.loading = true;
    const formValue = this.eventForm.value;
    const eventDate = new Date(formValue.eventDate);
    const now = new Date();

    if (eventDate <= now) {
      this.loading = false;
      this.errorMessage = 'Event date must be in the future.';
      return;
    }

    if (!formValue.maxParticipants || formValue.maxParticipants === '') {
      formValue.maxParticipants = null;
    } else if (formValue.maxParticipants < 1) {
      this.loading = false;
      this.errorMessage = 'Maximum participants must be at least 1 if specified.';
      return;
    }

    if (this.isEditMode && this.eventId) {
      this.eventService.updateEvent(this.eventId, formValue).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            this.successMessage = 'Event updated successfully!';
            setTimeout(() => {
              this.router.navigate(['/admin/events']);
            }, 1500);
          }
        },
        error: (error) => {
          this.loading = false;
          this.handleError(error);
        }
      });
    } else {
      this.eventService.createEvent(formValue).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            this.successMessage = 'Event created successfully!';
            setTimeout(() => {
              this.router.navigate(['/admin/events']);
            }, 1500);
          }
        },
        error: (error) => {
          this.loading = false;
          this.handleError(error);
        }
      });
    }
  }

  private handleError(error: any): void {
    if (error.error?.errors && Array.isArray(error.error.errors)) {
      this.errorMessage = error.error.errors.join(', ');
    } else if (error.error?.message) {
      this.errorMessage = error.error.message;
    } else if (error.error?.errors && typeof error.error.errors === 'object') {
      this.errorMessage = Object.values(error.error.errors).flat().join(', ');
    } else {
      this.errorMessage = 'An error occurred. Please try again.';
    }
  }

  private toDateTimeLocalString(value: string | Date): string {
    const date = new Date(value);
    const pad = (num: number) => num.toString().padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}
