import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './api.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Blocked Countries Frontend';
  
  blockedCountries: any[] = [];
  logs: any[] = [];
  
  newCountryCode = '';
  tempDuration = 10;
  
  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadCountries();
    this.loadLogs();
  }

  loadCountries() {
    this.api.getBlockedCountries(1, 100).subscribe({
      next: (res: any) => this.blockedCountries = res.data || res,
      error: (err: any) => console.error(err)
    });
  }

  loadLogs() {
    this.api.getBlockLogs(1, 100).subscribe({
      next: (res: any) => this.logs = res.data || res,
      error: (err: any) => console.error(err)
    });
  }

  addCountry() {
    if (!this.newCountryCode) return;
    this.api.addBlockedCountry(this.newCountryCode).subscribe({
      next: () => {
        this.loadCountries();
        this.newCountryCode = '';
      },
      error: (err: any) => alert('Error adding country: ' + (err.error?.message || err.message))
    });
  }

  addTemporal() {
    if (!this.newCountryCode || !this.tempDuration) return;
    this.api.addTemporalBlock(this.newCountryCode, this.tempDuration).subscribe({
      next: () => {
        this.loadCountries();
        this.newCountryCode = '';
      },
      error: (err: any) => alert('Error adding temporal block: ' + (err.error?.message || err.message))
    });
  }

  removeCountry(code: string) {
    if (!confirm(Are you sure you want to unblock ${code}?)) return;
    this.api.deleteBlockedCountry(code).subscribe({
      next: () => this.loadCountries(),
      error: (err: any) => console.error(err)
    });
  }
}
