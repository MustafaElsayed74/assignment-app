import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { ToastService } from './shared/toast.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent],
  template: `
    <app-navbar></app-navbar>
    <main class="container-xxl py-4 app-shell">
      <router-outlet></router-outlet>
    </main>

    <section class="toast-stack" aria-live="polite" aria-atomic="true">
      <article
        class="toast-card"
        *ngFor="let toast of (toasts$ | async)"
        [ngClass]="'toast-' + toast.type">
        <div class="toast-icon">
          <i class="bi" [ngClass]="{
            'bi-check2-circle': toast.type === 'success',
            'bi-exclamation-octagon': toast.type === 'error',
            'bi-info-circle': toast.type === 'info'
          }"></i>
        </div>
        <div class="toast-content">
          <h6>{{toast.title}}</h6>
          <p>{{toast.message}}</p>
        </div>
        <button class="toast-dismiss" type="button" (click)="dismiss(toast.id)" aria-label="Dismiss notification">
          <i class="bi bi-x"></i>
        </button>
      </article>
    </section>
  `
})
export class AppComponent {
  title = 'BlockedCountries Dashboard';

  readonly toasts$ = this.toastService.toasts$;

  constructor(private toastService: ToastService) { }

  dismiss(id: number): void {
    this.toastService.dismiss(id);
  }
}