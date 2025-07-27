import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  template: `
    <form (ngSubmit)="login()">
      <input [(ngModel)]="username" name="username" placeholder="Username" />
      <input [(ngModel)]="password" name="password" type="password" placeholder="Password" />
      <button type="submit">Login</button>
    </form>
  `
})
export class LoginComponent {
  username = '';
  password = '';

  constructor(private auth: AuthService, private router: Router) {}

  login() {
    this.auth.login({ username: this.username, password: this.password }).subscribe(() => {
      const role = localStorage.getItem(this.auth.roleKey);
      const normalizedRole = role?.trim().toLowerCase();
      if (normalizedRole === 'admin') {
        this.router.navigate(['/admin']);
      } else if (normalizedRole === 'user') {
        this.router.navigate(['/home']);
      } else {
        this.router.navigate(['/unauthorized']);
      }
    });
  }
}
