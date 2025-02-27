import { Component } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  NavigationEnd,
  Router,
  RouterOutlet,
} from '@angular/router';
import { NgIf, CommonModule } from '@angular/common';
import { HeaderComponent } from '../shared/header/header.component';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgIf, CommonModule, RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'ClientApp';
  showNavbar: boolean = true;
  router: Router;

  constructor(router: Router) {
    this.router = router;
  }

  ngOnInit() {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        const currentRoute = this.router.routerState.root.snapshot;
        this.showNavbar = !this.getHideNavbar(currentRoute);
      });
  }

  getHideNavbar(route: ActivatedRouteSnapshot): boolean {
    if (route.data && route.data['hideNavbar']) {
      return true;
    }
    return route.firstChild ? this.getHideNavbar(route.firstChild) : false;
  }
}
