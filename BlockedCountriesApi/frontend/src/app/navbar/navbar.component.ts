import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule],
  template: `
    <nav class="navbar navbar-expand-lg topbar mb-4">
      <div class="container-xxl">
        <a class="navbar-brand fw-bold d-flex align-items-center gap-2" routerLink="/dashboard">
          <span class="brand-mark"><i class="bi bi-shield-lock-fill"></i></span>
          <span class="brand-text">
            BlockGuard
            <small>Security Console</small>
          </span>
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto align-items-lg-center gap-lg-2">
            <li class="nav-item">
              <a class="nav-link" routerLink="/dashboard" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">
                <i class="bi bi-speedometer2 me-1"></i>Dashboard
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/countries" routerLinkActive="active">
                <i class="bi bi-globe2 me-1"></i>Blocked List
              </a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/logs" routerLinkActive="active">
                <i class="bi bi-journal-text me-1"></i>Security Logs
              </a>
            </li>
            <li class="nav-item ms-lg-2">
              <a class="btn btn-sm topbar-btn" [href]="environment.swaggerUrl" target="_blank" rel="noreferrer">
                <i class="bi bi-box-arrow-up-right me-1"></i>API Docs
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {
  readonly environment = environment;
}
