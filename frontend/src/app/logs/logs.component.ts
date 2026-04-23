import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../api.service';

@Component({
    selector: 'app-logs',
    standalone: true,
    imports: [CommonModule],
    template: `
    <section class="page-block fade-in-up">
    <div class="row mb-4">
      <div class="col-md-12 d-flex justify-content-between align-items-center">
        <div>
          <h2 class="fw-bold mb-1"><i class="bi bi-shield-exclamation text-danger me-2"></i>Security Logs</h2>
          <p class="text-muted mb-0">Monitor blocked attempt history and access traffic.</p>
        </div>
        <button class="btn btn-outline-secondary btn-sm" (click)="loadLogs()"><i class="bi bi-arrow-repeat me-1"></i>Refresh Logs</button>
      </div>
    </div>

    <!-- Data Table -->
    <div class="card dashboard-card border-0 rounded-4 overflow-hidden">
      <div class="table-responsive">
        <table class="table table-hover table-striped align-middle mb-0 text-sm">
          <thead class="table-dark text-uppercase small text-muted" style="letter-spacing: 0.5px">
            <tr>
              <th class="ps-4 py-3">Timestamp</th>
              <th class="py-3">IP Address</th>
              <th class="py-3">Target Country</th>
              <th class="py-3">Resolution</th>
              <th class="py-3 pe-4 text-end">Client / Agent</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let log of logs">
              <td class="ps-4 fw-semibold text-secondary">{{log.timestamp | date:'medium'}}</td>
              <td><span class="badge bg-light text-dark font-monospace py-2">{{log.ipAddress || 'Unknown'}}</span></td>
              <td class="fw-bold">{{log.countryCode}}</td>
              <td>
                <span class="badge rounded-pill" [ngClass]="log.isBlocked ? 'bg-danger' : 'bg-success'">
                  {{log.isBlocked ? 'Access Denied' : 'Allowed'}}
                </span>
              </td>
              <td class="text-truncate text-muted text-end pe-4" style="max-width: 250px" [title]="log.userAgent">
                <small>{{log.userAgent}}</small>
              </td>
            </tr>
            <tr *ngIf="logs.length === 0">
              <td colspan="5" class="text-center py-5">
                <i class="bi bi-clipboard2-x fs-1 text-muted d-block mb-3"></i>
                <p class="text-muted fw-bold mb-0">No log entries found.</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="card-footer bg-white py-3 border-0 d-flex justify-content-between align-items-center">
        <small class="text-muted fw-bold">Page {{page}} / {{totalPages}} · {{totalCount}} total attempts</small>
        <div class="btn-group" *ngIf="logs.length > 0">
          <button class="btn btn-sm btn-outline-secondary" (click)="previousPage()" [disabled]="page <= 1">
            <i class="bi bi-chevron-left"></i> Previous
          </button>
          <button class="btn btn-sm btn-outline-secondary" (click)="nextPage()" [disabled]="page >= totalPages">
            Next <i class="bi bi-chevron-right"></i>
          </button>
        </div>
      </div>
    </div>
    </section>
  `
})
export class LogsComponent implements OnInit {
    logs: any[] = [];
  page = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

    constructor(private api: ApiService) { }

    ngOnInit(): void {
        this.loadLogs();
    }

    loadLogs() {
      this.api.getBlockLogs(this.page, this.pageSize).subscribe({
        next: (res: any) => {
          this.logs = res.items || res.data?.items || [];
          this.totalCount = res.totalCount ?? this.logs.length;
          this.totalPages = res.totalPages ?? 1;
          this.page = res.page ?? this.page;
        },
            error: (err: any) => console.error('Failed fetching logs', err)
        });
    }

    previousPage() {
      if (this.page <= 1) return;
      this.page--;
      this.loadLogs();
    }

    nextPage() {
      if (this.page >= this.totalPages) return;
      this.page++;
      this.loadLogs();
    }
}
