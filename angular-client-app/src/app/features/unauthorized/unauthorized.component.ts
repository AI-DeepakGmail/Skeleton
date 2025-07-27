import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-unauthorized',
  imports: [CommonModule],
  template: `
    <h2>Access Denied</h2>
    <p>You do not have permission to view this page.</p>
  `
})
export class UnauthorizedComponent {}
