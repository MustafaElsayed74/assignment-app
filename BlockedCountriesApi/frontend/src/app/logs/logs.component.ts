import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../api.service';
import { ToastService } from '../shared/toast.service';

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
        <div class="d-flex gap-2">
          <button class="btn btn-outline-secondary btn-sm" (click)="toggleBlockedOnly()">
            <i class="bi bi-funnel me-1"></i>{{showBlockedOnly ? 'Show All' : 'Blocked Only'}}
          </button>
          <button class="btn btn-outline-secondary btn-sm" (click)="exportVisibleLogs()" [disabled]="visibleLogs.length === 0">
            <i class="bi bi-download me-1"></i>Export
          </button>
          <button class="btn btn-outline-secondary btn-sm" (click)="loadLogs()" [disabled]="isLoading">
            <i class="bi bi-arrow-repeat me-1"></i>Refresh Logs
          </button>
        </div>
      </div>
    </div>

    <div class="subtle-panel mb-3 d-flex justify-content-between align-items-center flex-wrap gap-2">
      <span class="text-muted small fw-bold">Displaying {{visibleLogs.length}} of {{logs.length}} loaded entries</span>
      <span class="badge rounded-pill" [ngClass]="showBlockedOnly ? 'bg-danger' : 'bg-secondary'">
        {{showBlockedOnly ? 'Blocked attempts only' : 'All attempts'}}
      </span>
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
            <tr *ngIf="isLoading">
              <td colspan="5" class="py-4 px-4">
                <div class="loading-shimmer mb-2" style="height: 14px;"></div>
                <div class="loading-shimmer mb-2" style="height: 14px; width: 90%;"></div>
                <div class="loading-shimmer" style="height: 14px; width: 74%;"></div>
              </td>
            </tr>
            <tr *ngFor="let log of visibleLogs">
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
            <tr *ngIf="visibleLogs.length === 0 && !isLoading">
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
        <div class="btn-group" *ngIf="visibleLogs.length > 0">
          <button class="btn btn-sm btn-outline-secondary" (click)="previousPage()" [disabled]="page <= 1 || isLoading">
            <i class="bi bi-chevron-left"></i> Previous
          </button>
          <button class="btn btn-sm btn-outline-secondary" (click)="nextPage()" [disabled]="page >= totalPages || isLoading">
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
  visibleLogs: any[] = [];
  page = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;
  isLoading = false;
  showBlockedOnly = false;

  constructor(private api: ApiService, private toast: ToastService) { }

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs() {
    this.isLoading = true;
    this.api.getBlockLogs(this.page, this.pageSize).subscribe({
      next: (res: any) => {
        this.logs = res.items || res.data?.items || [];
        this.totalCount = res.totalCount ?? this.logs.length;
        this.totalPages = res.totalPages ?? 1;
        this.page = res.page ?? this.page;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed fetching logs', err);
        this.isLoading = false;
        this.toast.error('Load Failed', 'Could not fetch log history.');
      }
    });
  }

  toggleBlockedOnly() {
    this.showBlockedOnly = !this.showBlockedOnly;
    this.applyFilter();
  }

  exportVisibleLogs() {
    const blob = new Blob([JSON.stringify(this.visibleLogs, null, 2)], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = `block-attempt-logs-page-${this.page}.json`;
    anchor.click();
    window.URL.revokeObjectURL(url);

    this.toast.success('Export Created', 'Current log slice has been downloaded as JSON.');
  }

  applyFilter() {
    this.visibleLogs = this.showBlockedOnly ? this.logs.filter((log: any) => log.isBlocked) : this.logs;
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
