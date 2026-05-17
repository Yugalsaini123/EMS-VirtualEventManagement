//src/app/components/speaker/speaker.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SpeakerService } from '../../services/speaker.service';
import { Speaker } from '../../models/speaker.model';

@Component({
  selector: 'app-speaker',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './speaker.component.html',
  styleUrls: ['./speaker.component.css']
})
export class SpeakerComponent implements OnInit {
  speakers: Speaker[] = [];
  loading = true;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  speakerForm: FormGroup;
  showForm = false;
  submitted = false;

  constructor(
    private speakerService: SpeakerService,
    private formBuilder: FormBuilder
  ) {
    this.speakerForm = this.formBuilder.group({
      speakerName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      designation: ['', Validators.required],
      organization: ['', Validators.required],
      bio: ['', [Validators.required, Validators.minLength(10)]],
      phoneNumber: ['', Validators.required],
      linkedInUrl: [null, Validators.pattern('https?://.+')]
    });
  }

  private getErrorMessage(error: any): string {
    if (error?.error?.errors) {
      const firstKey = Object.keys(error.error.errors)[0];
      if (firstKey && error.error.errors[firstKey]?.length) {
        return error.error.errors[firstKey][0];
      }
    }
    return error?.error?.message || 'Failed to create speaker';
  }

  ngOnInit(): void {
    this.loadSpeakers();
  }

  loadSpeakers(): void {
    this.speakerService.getAllSpeakers().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.speakers = response.data;
        }
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'Failed to load speakers';
      }
    });
  }

  deleteSpeaker(speakerId: string): void {
    if (confirm('Are you sure you want to delete this speaker?')) {
      this.speakerService.deleteSpeaker(speakerId).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Speaker deleted successfully';
            this.loadSpeakers();
            setTimeout(() => {
              this.successMessage = null;
            }, 2000);
          }
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete speaker';
        }
      });
    }
  }

  onSubmit(): void {
    this.submitted = true;
    if (this.speakerForm.invalid) {
      return;
    }

    const formValue = this.speakerForm.value;

    this.speakerService.createSpeaker(formValue).subscribe({
      next: (response) => {
        if (response.success) {
          this.successMessage = 'Speaker created successfully';
          this.speakerForm.reset();
          this.showForm = false;
          this.submitted = false;
          this.loadSpeakers();
          setTimeout(() => {
            this.successMessage = null;
          }, 2000);
        }
      },
      error: (error) => {
        this.errorMessage = this.getErrorMessage(error);
      }
    });
  }

  get f() {
    return this.speakerForm.controls;
  }
}
