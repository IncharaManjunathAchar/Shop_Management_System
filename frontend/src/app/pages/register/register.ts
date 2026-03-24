import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router'; // ✅ ADD THIS
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule], // ✅ ADD RouterModule
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

  constructor(private http: HttpClient, private router: Router) {}

  register() {

    // ✅ validation
    if (!this.username || !this.email || !this.password) {
      alert("Please fill all required fields");
      return;
    }

    this.http.post<any>('https://localhost:5001/api/auth/register', {
      username: this.username,
      email: this.email,
      password: this.password,
      shopName: this.shopName,
      shopAddress: this.shopAddress,
      contactNumber: this.contactNumber
    }).subscribe({
      next: (res) => {
        alert("Registered successfully");

        // 👉 redirect to login
        this.router.navigate(['/login']);
      },
      error: (err) => {
        alert(err.error || "Registration failed");
      }
    });
  }
}