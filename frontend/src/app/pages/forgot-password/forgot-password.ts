import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {

  step: 'email' | 'otp' = 'email';
  email = '';
  otp = '';
  newPassword = '';
  confirmPassword = '';
  loading = false;
  successMsg = '';
  errorMsg = '';
  showNew = false;
  showConfirm = false;

  constructor(private auth: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  sendOtp() {
    if (!this.email) { this.errorMsg = 'Please enter your email.'; return; }
    this.loading = true; this.errorMsg = '';
    this.auth.forgotPassword(this.email).subscribe({
      next: () => {
        this.loading = false;
        this.step = 'otp';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err?.error || 'Failed to send OTP. Try again.';
        this.cdr.detectChanges();
      }
    });
  }

  resetPassword() {
    if (!this.otp) { this.errorMsg = 'Please enter the OTP.'; return; }
    if (!this.newPassword) { this.errorMsg = 'Please enter a new password.'; return; }
    if (this.newPassword !== this.confirmPassword) { this.errorMsg = 'Passwords do not match.'; return; }
    this.loading = true; this.errorMsg = '';
    this.auth.resetPassword(this.email, this.otp, this.newPassword).subscribe({
      next: () => {
        this.loading = false;
        this.successMsg = 'Password reset successful! Redirecting to login...';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err?.error || 'Invalid OTP or request failed.';
        this.cdr.detectChanges();
      }
    });
  }
}
