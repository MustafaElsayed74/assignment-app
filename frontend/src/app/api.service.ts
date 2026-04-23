import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    private baseUrl = 'http://localhost:5068/api';

    constructor(private http: HttpClient) { }

    getBlockedCountries(page: number = 1, pageSize: number = 10): Observable<any> {
        return this.http.get(`${this.baseUrl}/countries?page=${page}&pageSize=${pageSize}`);
    }

    addBlockedCountry(countryCode: string): Observable<any> {
        return this.http.post(`${this.baseUrl}/countries`, { countryCode });
    }

    deleteBlockedCountry(countryCode: string): Observable<any> {
        return this.http.delete(`${this.baseUrl}/countries/${countryCode}`);
    }

    addTemporalBlock(countryCode: string, durationMinutes: number): Observable<any> {
        return this.http.post(`${this.baseUrl}/countries/temporal`, { countryCode, durationMinutes });
    }

    getBlockLogs(page: number = 1, pageSize: number = 10): Observable<any> {
        return this.http.get(`${this.baseUrl}/logs/blocked-attempts?page=${page}&pageSize=${pageSize}`);
    }
}