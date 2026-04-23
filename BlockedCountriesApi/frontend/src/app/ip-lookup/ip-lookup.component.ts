import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ApiService } from '../api.service';
import { ToastService } from '../shared/toast.service';

@Component({
  selector: 'app-ip-lookup',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="page-block fade-in-up">
    <div class="row align-items-center mb-4">
      <div class="col-lg-8">
        <h2 class="fw-bold mb-1"><i class="bi bi-grid-1x2-fill text-primary me-2"></i>Operations Dashboard</h2>
        <p class="text-muted mb-0">Investigate IP geography and validate live access policy outcomes.</p>
      </div>
      <div class="col-lg-4 text-lg-end mt-3 mt-lg-0">
        <button class="btn btn-sm btn-outline-secondary" (click)="loadOverview()" [disabled]="overviewLoading">
          <span *ngIf="!overviewLoading"><i class="bi bi-arrow-repeat me-1"></i>Refresh Overview</span>
          <span *ngIf="overviewLoading"><span class="spinner-border spinner-border-sm me-1"></span>Refreshing...</span>
        </button>
      </div>
    </div>

    <div class="kpi-grid mb-4">
      <article class="kpi-card">
        <span class="kpi-icon kpi-icon-brand"><i class="bi bi-shield-lock"></i></span>
        <div>
          <p class="kpi-label">Active Blocks</p>
          <p class="kpi-value">{{overview.activeBlocks}}</p>
        </div>
      </article>

      <article class="kpi-card">
        <span class="kpi-icon kpi-icon-teal"><i class="bi bi-journal-richtext"></i></span>
        <div>
          <p class="kpi-label">Attempt Logs</p>
          <p class="kpi-value">{{overview.totalAttempts}}</p>
        </div>
      </article>

      <article class="kpi-card">
        <span class="kpi-icon kpi-icon-violet"><i class="bi bi-geo"></i></span>
        <div>
          <p class="kpi-label">Caller Network</p>
          <p class="kpi-value fs-6 mb-0">{{overview.callerIp}}</p>
          <small class="text-muted">{{overview.callerCountry}}</small>
        </div>
      </article>
    </div>
    <div class="row align-items-stretch g-4 mt-2">
      <!-- IP Lookup Widget -->
      <div class="col-md-6 mb-4">
        <h3 class="fw-bold mb-3"><i class="bi bi-geo-alt-fill text-primary"></i> Geolocation Scanner</h3>
        <p class="text-muted">Enter an IP to locate it globally and discover network ISP details.</p>

        <div class="card dashboard-card border-0 rounded-4 overflow-hidden mt-4">
          <div class="card-header bg-primary text-white pt-3 pb-3 border-0">
            <strong class="fs-6 d-flex align-items-center">IP Address Lookup</strong>
          </div>
          <div class="card-body p-4 bg-light">
            <div class="input-group mb-3 shadow-sm rounded-3 overflow-hidden">
               <input class="form-control border-0 py-3 ps-4 fs-5" [(ngModel)]="customIp" placeholder="e.g. 8.8.8.8" (keyup.enter)="lookupIp()"/>
               <button class="btn btn-primary px-4 fw-bold" (click)="lookupIp()" [disabled]="!customIp || lookupLoading">
                 <span *ngIf="!lookupLoading"><i class="bi bi-search me-1"></i> Scan</span>
                 <span *ngIf="lookupLoading"><span class="spinner-border spinner-border-sm me-1"></span>Scanning...</span>
               </button>
            </div>

            <div *ngIf="lookupLoading" class="subtle-panel mb-3">
              <div class="loading-shimmer mb-2" style="height: 14px;"></div>
              <div class="loading-shimmer mb-2" style="height: 14px; width: 85%;"></div>
              <div class="loading-shimmer" style="height: 14px; width: 70%;"></div>
            </div>
            
            <!-- Result Box -->
            <div class="alert alert-light border border-2 shadow-sm rounded-3 p-4 mt-4" *ngIf="lookupResult">
               <h4 class="alert-heading text-primary fw-bold mb-3 d-flex align-items-center">
                 <i class="bi bi-map me-2 fs-3"></i> {{lookupResult.city}}, {{lookupResult.countryName}} ({{lookupResult.countryCode}})
               </h4>
               <hr>
               <div class="row g-3">
                 <div class="col-6">
                   <div class="text-secondary small fw-bold text-uppercase">Target IPv4</div>
                   <div class="fs-5 font-monospace text-dark">{{lookupResult.ip}}</div>
                 </div>
                 <div class="col-6">
                   <div class="text-secondary small fw-bold text-uppercase">ISP Org</div>
                   <div class="fw-semibold text-dark text-truncate">{{lookupResult.isp || 'Unknown'}}</div>
                 </div>
               </div>
            </div>
            <div class="alert alert-danger shadow-sm border-0 d-flex align-items-center mt-3" *ngIf="lookupError && !lookupLoading">
              <i class="bi bi-exclamation-triangle-fill fs-4 me-3"></i>
              <div>
                <strong>Error resolving IP</strong><br/>
                Cannot retrieve geo-data for the provided IP address.
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Access Status Widget -->
      <div class="col-md-6 mb-4">
        <h3 class="fw-bold mb-3"><i class="bi bi-person-bounding-box text-success"></i> Identity Validation</h3>
        <p class="text-muted">Test local IP access against active conditional rulesets.</p>
        
        <div class="card dashboard-card border-0 rounded-4 mt-4 text-center h-100 d-flex flex-column">
          <div class="card-body p-5 d-flex flex-column justify-content-center flex-grow-1">
             <i class="bi bi-ethernet fs-1 text-muted mb-3 d-block"></i>
             <h4 class="card-title fw-bold">Test My Network Security</h4>
             <p class="text-muted mb-5">Click below to assess if your current connection is permitted by the BlockGuard API system.</p>
             
             <button class="btn btn-success btn-lg rounded-pill shadow-sm" (click)="checkMyIp()" *ngIf="!checkingIP">
                Verify My Access
             </button>
             <button class="btn btn-secondary btn-lg rounded-pill shadow-sm" disabled *ngIf="checkingIP">
                <span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> Scanning Protocol...
             </button>
             
             <!-- Access Result -->
             <div *ngIf="accessResult" class="mt-4 pt-4 border-top text-start">
                <div class="alert" [ngClass]="accessResult.isBlocked ? 'alert-danger' : 'alert-success'">
                  <div class="d-flex">
                    <i class="bi fs-1 me-3" [ngClass]="accessResult.isBlocked ? 'bi-shield-lock-fill' : 'bi-shield-check-fill'"></i>
                    <div>
                      <h4 class="alert-heading m-0">{{accessResult.isBlocked ? 'Access Denied!' : 'Access Granted'}}</h4>
                      <p class="mb-0 mt-1">
                        We detected you from <strong>{{accessResult.countryCode}}</strong>.
                        {{accessResult.isBlocked ? "Your country is restricted currently." : "Your network rules are clean."}}
                      </p>
                    </div>
                  </div>
                </div>
             </div>
          </div>
        </div>
      </div>
    </div>
    </section>
  `
})
export class IpLookupComponent implements OnInit {
  customIp = '';
  lookupResult: any = null;
  lookupError = false;
  lookupLoading = false;

  checkingIP = false;
  accessResult: any = null;

  overviewLoading = true;
  overview = {
    activeBlocks: 0,
    totalAttempts: 0,
    callerIp: '--',
    callerCountry: 'Detecting...'
  };

  constructor(private api: ApiService, private toast: ToastService) { }

  ngOnInit(): void {
    this.loadOverview();
  }

  loadOverview(): void {
    this.overviewLoading = true;

    forkJoin({
      countries: this.api.getBlockedCountries(1, 1),
      logs: this.api.getBlockLogs(1, 1),
      caller: this.api.lookupCallerIp()
    }).subscribe({
      next: ({ countries, logs, caller }) => {
        this.overview = {
          activeBlocks: countries?.totalCount ?? 0,
          totalAttempts: logs?.totalCount ?? 0,
          callerIp: caller?.ip ?? '--',
          callerCountry: caller?.countryName ?? caller?.countryCode ?? 'Unknown'
        };
        this.overviewLoading = false;
      },
      error: () => {
        this.overviewLoading = false;
        this.toast.info('Overview Partial', 'Some dashboard metrics are temporarily unavailable.');
      }
    });
  }

  lookupIp() {
    if (!this.customIp) {
      this.toast.info('IP Required', 'Enter an IP address to run geolocation lookup.');
      return;
    }

    this.lookupLoading = true;
    this.lookupError = false;
    this.lookupResult = null;
    this.api.lookupIp(this.customIp).subscribe({
      next: (res) => {
        this.lookupResult = res;
        this.lookupLoading = false;
        this.toast.success('Lookup Completed', `Resolved ${res?.countryCode ?? 'country'} successfully.`);
      },
      error: () => {
        this.lookupError = true;
        this.lookupLoading = false;
        this.toast.error('Lookup Failed', 'Unable to resolve this IP at the moment.');
      }
    });
  }

  checkMyIp() {
    this.checkingIP = true;
    this.accessResult = null;
    this.api.checkBlock().subscribe({
      next: (res) => {
        this.checkingIP = false;
        this.accessResult = res;
        this.toast.info('Check Complete', `Caller resolved to ${res?.countryCode ?? 'Unknown'}.`);
      },
      error: (err) => {
        this.checkingIP = false;
        this.accessResult = err.error || err;
        this.toast.error('Check Failed', 'Could not verify current caller access.');
      }
    });
  }
}