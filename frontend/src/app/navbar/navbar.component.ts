import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [RouterModule],
    template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark mb-4 shadow-sm">
      <div class="container">
        <a class="navbar-brand fw-bold" routerLink="/dashboard"><i class="bi bi-shield-lock-fill text-warning me-2"></i>BlockGuard</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <li class="nav-item">
              <a class="nav-link" routerLink="/dashboard" routerLinkActive="active">Dashboard</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/countries" routerLinkActive="active">Blocked List</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" routerLink="/logs" routerLinkActive="active">Security Logs</a>
            </li>
          </ul>
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent { }
