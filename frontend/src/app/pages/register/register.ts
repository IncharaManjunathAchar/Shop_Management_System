import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {

  step: 'form' | 'otp' = 'form';
  username = '';
  email = '';
  password = '';
  shopName = '';
  shopAddress = '';
  contactNumber = '';
  otp = '';
  showPassword = false;
  loading = false;
  errorMsg = '';

  constructor(private auth: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  register() {
    if (!this.username || !this.email || !this.password || !this.shopName || !this.shopAddress || !this.contactNumber) {
      this.errorMsg = 'Please fill all required fields.'; return;
    }
    this.loading = true; this.errorMsg = '';
    this.auth.register({
      username: this.username, email: this.email, password: this.password,
      shopName: this.shopName, shopAddress: this.shopAddress, contactNumber: this.contactNumber
    }).subscribe({
      next: () => {
        this.loading = false;
        this.step = 'otp';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error || 'Registration failed.';
        this.cdr.detectChanges();
      }
    });
  }

  verifyOtp() {
    if (!this.otp) { this.errorMsg = 'Please enter the OTP.'; return; }
    this.loading = true; this.errorMsg = '';
    this.auth.verifyEmail(this.email, this.otp).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/shopkeeper/subscription']);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error || 'Invalid OTP.';
        this.cdr.detectChanges();
      }
    });
  }
}
