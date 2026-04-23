# Subject: Submission for Full-Stack Developer Assessment - Blocked Countries API

Dear Hiring Team,

Please find attached my submission for the Full-Stack Developer assessment. I am excited to present the **Blocked Countries API & Dashboard**, a comprehensive solution designed to handle geo-blocking, IP geolocation, and network access management.

I approached this assignment with a focus on building a robust, production-ready system. Below is a summary of how I tackled the requirements and the technical decisions I made throughout the process:

## Architecture & Technical Decisions
- **Backend (.NET 8 Web API):** I implemented a clean, N-tier architecture utilizing dependency injection. To handle data without an external database (as permitted by the scope), I utilized thread-safe memory collections (`ConcurrentDictionary` and `ConcurrentBag`) to ensure the API remains extremely fast and completely lock-free under concurrent loads.
- **Frontend (Angular 17):** The dashboard is built with Angular 17's modern standalone components. I focused heavily on UI/UX, implementing a responsive, premium dark-mode aesthetic with custom color palettes, smooth micro-animations, and dynamic real-time widgets.
- **Background Processing:** For the temporal blocking feature, I integrated an intelligent `IHostedService` Background Worker. This quietly scrubs expired temporal blocks in the background without degrading the performance of the main API threads.
- **Testing:** The core business logic is heavily tested using xUnit and Moq, achieving 100% test coverage on the critical paths.

## Key Features Implemented
- **Dynamic Blocklist Management:** Real-time adding and removing of country codes.
- **Temporal Blocking:** Time-limited blocks that automatically expire.
- **Identity & Network Validation:** Endpoint testing to evaluate if a caller's IP is permitted by current rules.
- **IP Geolocation Scanner:** Integrated with `ip-api.com` to look up granular details (City, Region, ISP, Timezone) for any global IPv4 address.
- **Operations Dashboard:** A centralized, visual interface to track active blocks and denied access attempts.

## Live Environment
To make reviewing as seamless as possible, I have deployed the application to live environments. You can interact with the live project via the links below:
- **Live Dashboard:** [https://block-ip-orcin.vercel.app](https://block-ip-orcin.vercel.app)
- **Live API & Swagger:** [http://blockip.runasp.net/swagger/index.html](http://blockip.runasp.net/swagger/index.html)
- **GitHub Repository:** [https://github.com/MustafaElsayed74/assignment-app](https://github.com/MustafaElsayed74/assignment-app)

Detailed instructions for running the application locally, along with explanations of the project structure, can be found in the `README.md` at the root of the repository.

Thank you for the opportunity to work on this assignment. I thoroughly enjoyed the challenge and look forward to discussing my technical choices with the team.

Best regards,

**Mustafa Elsayed**
*(Email / Phone / LinkedIn)*
