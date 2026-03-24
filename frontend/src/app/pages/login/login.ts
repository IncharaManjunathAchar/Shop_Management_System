import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router'; // ✅ ADD THIS
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule], // ✅ ADD RouterModule
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {

  username = '';
  password = '';

  constructor(private http: HttpClient, private router: Router) {}

  login() {
    if (!this.username || !this.password) {
      alert("Please enter username and password");
      return;
    }

    this.http.post<any>('https://localhost:5001/api/auth/login', {
      username: this.username,
      password: this.password
    }).subscribe({
      next: (res) => {
        // ✅ store token
        localStorage.setItem('token', res.token);

        alert("Login successful");

        // 👉 redirect to inventory (IMPORTANT CHANGE)
        this.router.navigate(['/inventory']);
      },
      error: (err) => {
        alert(err.error || "Invalid credentials");
      }
    });
  }
}