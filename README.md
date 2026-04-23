# Blocked Countries API

A .NET 8 Web API to manage blocked countries and validate IP addresses using third-party geolocation. 
This application relies on thread-safe in-memory collections and does not require an external database.

## Features

1. **Add Blocked Country**: Store permanent country blocks by ISO 3166-1 alpha-2 code.
2. **Delete Blocked Country**: Unblock countries.
3. **Get All Blocked Countries**: Paginated, searchable listing of all blocked countries.
4. **Find My Country via IP Lookup**: Resolves an IP address to geolocation data using `ipapi.co`.
5. **Verify Blocked IP**: Checks if the caller's IP resolves to a blocked country and logs the attempt.
6. **Log Attempts**: Paginated history of IP block-check attempts.
7. **Temporal Blocks**: Block a country for a specific duration (1-1440 minutes). Automatically expires via background worker.

## Architecture

- **Clean Architecture Principles**: Organized into Controllers, Services, and Repositories.
- **In-Memory Storage**: Uses `ConcurrentDictionary` and `ConcurrentBag` for thread-safe operations.
- **Background Service**: Periodically cleans up expired temporal blocks.
- **Global Error Handling**: Custom middleware ensuring standardized `ApiErrorResponse` structures.
- **Validation**: Strict ISO 3166-1 alpha-2 country code validation natively without extra API calls.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Running the Project

1. Navigate to the project directory:
   ```bash
   cd BlockedCountriesApi
   ```
2. Build and run the API:
   ```bash
   dotnet run
   ```
3. Open a browser and navigate to:
   - Swagger UI: `http://localhost:5xxx/swagger`
   
*(Port `5xxx` will be displayed in the terminal output).*

## Third-Party API Configuration

The API uses [ipapi.co](https://ipapi.co) for IP geolocation. The free tier does not strictly require an API key for up to ~1,000 requests/day. 

If you have a paid tier or wish to configure it, modify `appsettings.json`:

```json
"IpApi": {
  "BaseUrl": "https://ipapi.co",
  "ApiKey": "YOUR_API_KEY_HERE"
}
```

## Testing

A suite of unit tests covers core business logic, validation, and simulated HttpClient responses.

1. Navigate to the test project directory:
   ```bash
   cd BlockedCountriesApi.Tests
   ```
2. Run tests:
   ```bash
   dotnet test
   ```

## Author
Developed as part of a .NET Developer Test Assignment.
