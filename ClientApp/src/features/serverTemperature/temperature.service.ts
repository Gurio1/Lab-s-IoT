import { Injectable } from '@angular/core';
import { ServerTemperatureData } from './dto/websocketResponse';
import { HttpClient } from '@angular/common/http';
import { API_URL, JWT_TOKEN } from '../../constants';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TemperatureService {
  private maxEntries = 20;
  private temperatureData: ServerTemperatureData[] = [];

  constructor(private http: HttpClient) {}

  public fetchLastTemperatureData(
    count: number
  ): Observable<ServerTemperatureData[]> {
    return this.http
      .post<ServerTemperatureData[]>(API_URL + `temperature`, { Count: count })
      .pipe(
        tap((response: ServerTemperatureData[]) => {
          console.log('Fetched temperature data:', response);
          this.temperatureData = response;
          return response;
        })
      );
  }

  public storeTemperatureData(data: ServerTemperatureData) {
    this.temperatureData.push(data);
    this.checkMaxEntries();
  }

  public getTemperatureData(): ServerTemperatureData[] {
    return this.temperatureData;
  }

  private checkMaxEntries() {
    if (this.temperatureData.length > this.maxEntries) {
      this.temperatureData.shift();
    }
  }
}
