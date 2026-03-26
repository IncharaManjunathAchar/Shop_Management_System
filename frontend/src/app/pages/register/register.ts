import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {

  username = '';
  email = '';
  password = '';
  shopName = '';
  shopAddress = '';
  contactNumber = '';

  constructor(private auth: AuthService, private router: Router) {}

  register() {
    if (!this.username || !this.email || !this.password) {
      alert("Please fill all required fields");
      return;
    }

    this.auth.register({
      username: this.username,
      email: this.email,
      password: this.password,
      shopName: this.shopName,
      shopAddress: this.shopAddress,
      contactNumber: this.contactNumber
    }).subscribe({
      next: () => {
        alert("Registered successfully");
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert(err.error || "Registration failed");
      }
    });
  }
}