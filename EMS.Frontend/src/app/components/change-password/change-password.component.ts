// src/app/components/change-password/change-password.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { ChangePasswordRequest } from '../../models/user.model';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {
  passwordForm: FormGroup;
  loading = false;
  submitted = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private userService: UserService
  ) {
    this.passwordForm = this.formBuilder.group({
      oldPassword: ['', [Validators.required, Validators.minLength(4)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', Validators.required]
    }, {
      validators: this.confirmPasswordValidator
    });
  }

  ngOnInit(): void {}

  get f() {
    return this.passwordForm.controls;
  }

  confirmPasswordValidator(form: FormGroup) {
    const newPassword = form.get('newPassword')?.value;
    const confirmNewPassword = form.get('confirmNewPassword')?.value;
    return newPassword && confirmNewPassword && newPassword !== confirmNewPassword ? { passwordMismatch: true } : null;
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = null;
    this.successMessage = null;

    if (this.passwordForm.invalid) {
      return;
    }

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser?.emailId) {
      this.errorMessage = 'Unable to determine the current user.';
      return;
    }

    this.loading = true;
    const request: ChangePasswordRequest = this.passwordForm.value;

    this.userService.changePassword(currentUser.emailId, request).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.successMessage = 'Password changed successfully.';
          this.passwordForm.reset();
          this.submitted = false;
        } else {
          this.errorMessage = response.message || 'Failed to change password.';
        }
      },
      error: (error) => {
        this.loading = false;
        // Display backend validation errors
        if (error.error?.errors && error.error.errors.length > 0) {
          this.errorMessage = error.error.errors.join('. ');
        } else {
          this.errorMessage = error.error?.message || 'Failed to change password.';
        }
      }
    });
  }
}
