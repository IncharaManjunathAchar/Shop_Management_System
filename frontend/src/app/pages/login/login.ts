import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {

  username = '';
  password = '';

  constructor(private auth: AuthService, private router: Router) {}

  login() {
    if (!this.username || !this.password) {
      alert("Please enter username and password");
      return;
    }

    this.auth.login(this.username, this.password).subscribe({
      next: () => {
        alert("Login successful");
        const role = this.auth.getRole();
        this.router.navigate([role === 'Admin' ? '/admin/dashboard' : '/inventory']);
      },
      error: (err) => {
        alert(err.error || "Invalid credentials");
      }
    });
  }
}