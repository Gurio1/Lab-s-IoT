import { Routes } from '@angular/router';
import { LoginComponent } from '../features/identity/login/login.component';
import { ServerTemperatureComponent } from '../features/serverTemperature/server-temperature/server-temperature.component';
import { DoorComponent } from '../features/door/door.component';
import { HomeComponent } from '../features/home/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, data: { hideNavbar: true } },
  { path: 'temperature', component: ServerTemperatureComponent },
  { path: 'door', component: DoorComponent },
  { path: 'home', component: HomeComponent },
];
