import { Routes } from '@angular/router';
import { CountriesComponent } from './countries/countries.component';
import { IpLookupComponent } from './ip-lookup/ip-lookup.component';
import { LogsComponent } from './logs/logs.component';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: IpLookupComponent },
    { path: 'countries', component: CountriesComponent },
    { path: 'logs', component: LogsComponent }
];
