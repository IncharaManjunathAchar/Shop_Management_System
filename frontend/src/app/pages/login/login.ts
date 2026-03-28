import { Component, ChangeDetectorRef } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {

  username = '';
  password = '';
  showPassword = false;
  errorMsg = '';

  constructor(private auth: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  login() {
    if (!this.username || !this.password) {
      this.errorMsg = 'Please enter username and password.';
      return;
    }
    this.errorMsg = '';
    this.auth.login(this.username, this.password).subscribe({
      next: () => {
        const role = this.auth.getRole();
        if (role === 'Admin') this.router.navigate(['/admin/dashboard']);
        else this.router.navigate(['/shopkeeper/subscription']);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMsg = err.error || 'Invalid credentials.';
        this.cdr.detectChanges();
      }
    });
  }
}
