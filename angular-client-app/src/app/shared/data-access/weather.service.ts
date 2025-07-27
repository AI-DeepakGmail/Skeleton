import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api-endpoints';
import { ApiClientService } from '../api-client/api-client.service';


export class WeatherService {
  private api = inject(ApiClientService);
  getForecast(): Observable<WeatherForecast[]> {
    return this.api.get<WeatherForecast[]>(API_ENDPOINTS.weather.forecast);
  }
}

export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
