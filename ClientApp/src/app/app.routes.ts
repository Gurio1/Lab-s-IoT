import { Routes } from '@angular/router';
import { LoginComponent } from '../identity/login/login.component';
import { ServerTemperatureComponent } from '../serverTemperature/server-temperature/server-temperature.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'temperature', component: ServerTemperatureComponent },
];
