import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-admin',
  imports: [CommonModule],
  template: `<h1>Admin Panel</h1>
   <button (click)="logout()">Logout</button>`
})
export class AdminComponent {
 constructor(private auth: AuthService, private router: Router) {}

    logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
