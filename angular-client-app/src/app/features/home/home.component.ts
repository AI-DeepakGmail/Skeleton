import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { WeatherForecast, WeatherService } from '../../shared/data-access/weather.service';

@Component({
  standalone: true,
  selector: 'app-home',
  imports: [CommonModule],
   providers: [WeatherService],
  template: `
    <h1>Welcome Home</h1>
    <button (click)="logout()">Logout</button>
    <h2>Weather Forecast</h2>
<table>
  <thead>
    <tr>
      <th>Date</th>
      <th>Temp (°C)</th>
      <th>Temp (°F)</th>
      <th>Summary</th>
    </tr>
  </thead>
  <tbody>
    <tr *ngFor="let forecast of forecasts">
      <td>{{ forecast.date | date }}</td>
      <td>{{ forecast.temperatureC }}</td>
      <td>{{ forecast.temperatureF }}</td>
      <td>{{ forecast.summary }}</td>
    </tr>
  </tbody>
</table>

  `
})
export class HomeComponent {
   forecasts: WeatherForecast[] = [];
  constructor(private auth: AuthService, private router: Router,private weatherService: WeatherService) {}

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

   ngOnInit(): void {
    this.weatherService.getForecast().subscribe({
      next: (data) => this.forecasts = data,
      error: (err) => console.error('Weather API error:', err)
    });
  }
}
