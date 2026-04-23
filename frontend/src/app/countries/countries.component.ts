import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-countries',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="row g-4 mb-4">
      <div class="col-md-12">
        <h2 class="fw-bold"><i class="bi bi-globe-americas me-2"></i>Manage Blocked Countries</h2>
        <p class="text-muted">Control access by specifying country codes permanently or temporarily.</p>
      </div>

      <!-- Add Blocks Cards -->
      <div class="col-md-6">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-header bg-white pt-3 pb-2 border-0">
            <h5 class="card-title fw-bold text-danger m-0">Permanent Block</h5>
          </div>
          <div class="card-body mt-2">
            <div class="input-group">
              <input type="text" class="form-control bg-light" placeholder="Country Code (e.g. US)" [(ngModel)]="newCountryCode" maxlength="2" />
              <button class="btn btn-danger px-4" (click)="addCountry()">Block</button>
            </div>
            <small class="text-muted mt-2 d-block">Indefinitely block this country.</small>
          </div>
        </div>
      </div>
      
      <div class="col-md-6">
        <div class="card shadow-sm border-0 h-100">
          <div class="card-header bg-white pt-3 pb-2 border-0">
            <h5 class="card-title fw-bold text-warning m-0">Temporal Block</h5>
          </div>
          <div class="card-body mt-2">
            <div class="input-group mb-2">
              <input type="text" class="form-control bg-light w-50" placeholder="Country Code" [(ngModel)]="tempCountryCode" maxlength="2" />
              <input type="number" class="form-control bg-light w-25" placeholder="Mins" [(ngModel)]="tempDuration" min="1" />
              <button class="btn btn-warning w-25" (click)="addTemporal()">Temp Block</button>
            </div>
            <small class="text-muted mt-1 d-block">Block automatically lifts after duration expires.</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Data Table -->
    <div class="card shadow-sm border-0 mt-4 rounded-3 overflow-hidden">
      <div class="card-header bg-dark text-white p-3 d-flex justify-content-between align-items-center">
        <h5 class="m-0"><i class="bi bi-list-ul me-2"></i>Active Blocks ({{blockedCountries.length}})</h5>
        <div class="d-flex gap-2">
           <input type="text" class="form-control form-control-sm" placeholder="Search..." [(ngModel)]="searchQuery" (keyup.enter)="loadCountries()">
           <button class="btn btn-sm btn-light" (click)="loadCountries()"><i class="bi bi-arrow-repeat"></i></button>
        </div>
      </div>
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="table-light text-muted">
            <tr>
              <th class="ps-4">Code</th>
              <th>Name</th>
              <th>Block Type</th>
              <th>Added At</th>
              <th>Expiry</th>
              <th class="text-end pe-4">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let country of blockedCountries">
              <td class="ps-4 fw-bold">{{country.countryCode}}</td>
              <td>{{country.countryName || 'Unknown'}}</td>
              <td>
                <span class="badge rounded-pill px-3 py-2" [ngClass]="country.durationMinutes ? 'bg-warning text-dark' : 'bg-danger'">
                  <i class="bi" [ngClass]="country.durationMinutes ? 'bi-clock-history' : 'bi-shield-slash'"></i>
                  {{country.durationMinutes ? 'Temporal' : 'Permanent'}}
                </span>
              </td>
              <td class="text-muted"><small>{{country.blockedAt | date:'short'}}</small></td>
              <td><small class="text-secondary fw-semibold">{{country.expiresAt ? (country.expiresAt | date:'mediumTime') : 'Never'}}</small></td>
              <td class="text-end pe-4">
                <button class="btn btn-sm btn-outline-secondary" (click)="removeCountry(country.countryCode)">
                  <i class="bi bi-unlock"></i> Unblock
                </button>
              </td>
            </tr>
            <tr *ngIf="blockedCountries.length === 0">
              <td colspan="6" class="text-center py-5">
                <i class="bi bi-inbox fs-1 text-muted d-block mb-3"></i>
                <p class="text-muted fw-bold mb-0">No active blocks found.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class CountriesComponent implements OnInit {
  blockedCountries: any[] = [];
  newCountryCode = '';
  tempCountryCode = '';
  tempDuration = 10;
  searchQuery = '';

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.loadCountries();
  }

  loadCountries() {
    this.api.getBlockedCountries(1, 100, this.searchQuery).subscribe({
      next: (res: any) => this.blockedCountries = res.data?.items || res.items || res,
      error: (err: any) => console.error('Failed fetching', err)
    });
  }

  addCountry() {
    if (!this.newCountryCode) return;
    this.api.addBlockedCountry(this.newCountryCode.toUpperCase()).subscribe({
      next: () => {
        this.loadCountries();
        this.newCountryCode = '';
      },
      error: (err: any) => alert(err.error?.message || 'Error occurred')
    });
  }

  addTemporal() {
    if (!this.tempCountryCode || !this.tempDuration) return;
    this.api.addTemporalBlock(this.tempCountryCode.toUpperCase(), this.tempDuration).subscribe({
      next: () => {
        this.loadCountries();
        this.tempCountryCode = '';
      },
      error: (err: any) => alert(err.error?.message || 'Error occurred')
    });
  }

  removeCountry(code: string) {
    if (!confirm(`Are you sure you want to completely unblock ${code}?`)) return;
    this.api.deleteBlockedCountry(code).subscribe({
      next: () => this.loadCountries(),
      error: (err: any) => console.error(err)
    });
  }
}
