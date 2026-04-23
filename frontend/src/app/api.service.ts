import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    private readonly baseUrl = environment.apiBaseUrl;

    constructor(private http: HttpClient) { }

    getBlockedCountries(page: number = 1, pageSize: number = 10, search?: string): Observable<any> {
        let url = `${this.baseUrl}/countries/blocked?page=${page}&pageSize=${pageSize}`;
        if (search) url += `&search=${search}`;
        return this.http.get(url);
    }

    addBlockedCountry(countryCode: string): Observable<any> {
        return this.http.post(`${this.baseUrl}/countries/block`, { countryCode });
    }

    deleteBlockedCountry(countryCode: string): Observable<any> {
        return this.http.delete(`${this.baseUrl}/countries/block/${countryCode}`);
    }

    addTemporalBlock(countryCode: string, durationMinutes: number): Observable<any> {
        return this.http.post(`${this.baseUrl}/countries/temporal-block`, { countryCode, durationMinutes });
    }

    getBlockLogs(page: number = 1, pageSize: number = 10): Observable<any> {
        return this.http.get(`${this.baseUrl}/logs/blocked-attempts?page=${page}&pageSize=${pageSize}`);
    }

    lookupIp(ipAddress: string): Observable<any> {
        return this.http.get(`${this.baseUrl}/ip/lookup?ipAddress=${ipAddress}`);
    }

    checkBlock(): Observable<any> {
        return this.http.get(`${this.baseUrl}/ip/check-block`);
    }
}
