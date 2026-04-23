import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent],
  template: `
    <app-navbar></app-navbar>
    <main class="container-xxl py-4 app-shell">
      <router-outlet></router-outlet>
    </main>
  `
})
export class AppComponent {
  title = 'BlockedCountries Dashboard';
}