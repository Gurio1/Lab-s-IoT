import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../../constants';
import { HealthStatus } from './models/HealthStatus';

@Component({
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  healthStatus!: HealthStatus;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchHealthStatus();
  }

  fetchHealthStatus(): void {
    this.http.get<HealthStatus>(API_URL + 'health').subscribe(
      (data) => {
        this.healthStatus = data;
      },
      (error) => {
        console.error('Error fetching health status:', error);
      }
    );
  }

  isHealthy(): boolean {
    return this.healthStatus?.status !== 'Unhealthy';
  }
  getDeviceKeys(): string[] {
    return this.healthStatus?.entries?.['iot-connections']?.data
      ? Object.keys(this.healthStatus.entries['iot-connections'].data)
      : [];
  }
}
