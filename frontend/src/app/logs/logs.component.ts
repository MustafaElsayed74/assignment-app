import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-logs',
  standalone: true,
  imports: [CommonModule],
  template: `
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
    <div class="card shadow-sm border-0 rounded-3 overflow-hidden">
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
        <small class="text-muted fw-bold">Showing latest {{logs.length}} attempt entries</small>
        <nav aria-label="Page navigation" *ngIf="logs.length > 0">
          <ul class="pagination pagination-sm mb-0">
            <li class="page-item disabled"><a class="page-link shadow-none" href="#">Previous</a></li>
            <li class="page-item active"><a class="page-link shadow-none" href="#">1</a></li>
            <li class="page-item disabled"><a class="page-link shadow-none" href="#">Next</a></li>
          </ul>
        </nav>
      </div>
    </div>
  `
})
export class LogsComponent implements OnInit {
  logs: any[] = [];

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs() {
    this.api.getBlockLogs(1, 100).subscribe({
      next: (res: any) => this.logs = res.data?.items || res.items || res,
      error: (err: any) => console.error('Failed fetching logs', err)
    });
  }
}
