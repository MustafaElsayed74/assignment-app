# Blocked Countries API & Dashboard

A comprehensive full-stack solution for geo-blocking, IP geolocation, and network access management. Built with a robust **.NET 8** backend and a dynamic **Angular 17** frontend, this platform allows administrators to restrict access from specific countries, view live geolocation data, and automatically manage temporal (temporary) blocklists.

## 🚀 Live Demos
- **Frontend Dashboard (Vercel)**: [https://block-ip-orcin.vercel.app](https://block-ip-orcin.vercel.app)
- **Backend API (runasp.net)**: [http://blockip.runasp.net](http://blockip.runasp.net)
- **Swagger Documentation**: [http://blockip.runasp.net/swagger/index.html](http://blockip.runasp.net/swagger/index.html)

---

## 🛠️ Tech Stack
### Backend
- **Framework**: .NET 8 Web API
- **Architecture**: N-Tier, Dependency Injection
- **Testing**: xUnit & Moq (100% Core Business Logic Coverage)
- **External API**: `ip-api.com` for precise IP Geolocation
- **Hosting**: runasp.net (IIS)

### Frontend
- **Framework**: Angular 17
- **Styling**: Bootstrap 5 + Vanilla CSS (Dynamic Glassmorphism & Custom Palettes)
- **Features**: Standalone Components, Reactive Forms, Observables
- **Hosting**: Vercel

---

## ✨ Features
1. **Dynamic Country Blocking**: Add and remove country codes from a centralized blocklist.
2. **Temporal Blocks**: Apply temporary IP blocks that automatically expire after a set time via an intelligent Background Worker.
3. **Identity & Network Validation**: Test caller IPs directly to assess their geolocation and block status.
4. **IP Geolocation Scanner**: Lookup details (City, Country, Region, ISP/Organization) for any target IPv4 address globally.
5. **Analytics & Logging**: Track denied access attempts and operational metrics via a live operations dashboard.
6. **Polished UI/UX**: Features an ultra-premium, dark-mode ready dashboard with sleek micro-animations and data-rich grids.

---

## ⚙️ Deployment Instructions

### Frontend (Vercel)
The Angular frontend is automatically deployed via Vercel. 
- It uses a custom `vercel.json` configuration file to proxy `/api` calls to the backend using reverse rewrites, completely circumventing cross-domain issues.
- **Command used for production build:** `npm run build` targeting Angular's modern `browser` builder structure.

### Backend (runasp.net)
The backend is published as a standalone .NET 8 application.
1. Clean and build the solution: `dotnet publish -c Release -o ./publish`
2. Zip the contents of the `./publish` directory (not the directory itself, just the contents).
3. Upload the resulting zip (or `backend-deploy.zip`) to the `runasp.net` File Manager inside the `\wwwroot` directory.
4. Extract the zip file contents directly into `\wwwroot`. The application automatically spins up with an open CORS policy to handle the Vercel traffic.

---

## 🧪 Running Locally

### Prerequisites
- .NET 8 SDK
- Node.js & npm
- Angular CLI

### Backend
```bash
cd backend
dotnet restore
dotnet run
```
The API will launch at `http://localhost:5068`.

### Frontend
```bash
cd frontend
npm install
npm start
```
The application will launch at `http://localhost:4200` with the proxy configured to hit the backend automatically.

---

## 🏗️ Architecture & Decisions
- **In-Memory Thread-Safe Repositories**: Utilizes `ConcurrentDictionary` and `ConcurrentBag` to provide a fast, lock-free data store without requiring an external database, perfectly suited for the scope of the assignment.
- **Background Cleanup Service**: Implements `IHostedService` to scrub expired temporal blocks quietly in the background without affecting API response times.
- **Resilient Geolocation Service**: Safely maps external IP API properties using internal DTOs (`IpApiResult`) to ensure the Angular frontend always receives consistent, properly camelCased JSON fields.
