import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  showNavbar = false;

  constructor(private router: Router) {}

  isActive(route: string): boolean {
    return this.router.url === route;
  }

  toggleSidebar() {
    this.showNavbar = !this.showNavbar;
  }
}
