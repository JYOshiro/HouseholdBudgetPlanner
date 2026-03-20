---
title: FAQ
---

This page answers common project questions for stakeholders, reviewers, and developers.

## Quick Links

- [Product and Scope](#product-and-scope)
- [Implementation Status](#implementation-status)
- [Setup and Testing](#setup-and-testing)
- [Documentation](#documentation)

## Product and Scope

### Is the backend already implemented?
Yes. Core financial modules, authentication, and dashboard endpoints are implemented in the current baseline.

### Is the frontend fully integrated with backend APIs?
Not yet. The frontend shell is available, and full feature-level API integration remains in progress.

### Is household data isolated per user context?
Yes. Authentication uses JWT bearer tokens, and household scope is derived from JWT claims. Clients must not send `householdId` in request bodies.

## Implementation Status

### Is the frontend fully complete?
Not yet. The frontend includes routing, auth flows, and protected pages, and still needs additional feature-level API integration and UX hardening.

### Which parts are most production-ready today?
Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned.

## Setup and Testing

### How do I test endpoints quickly?
Use Swagger in development mode after running database migration and backend startup.

### What environment variable should the frontend use for API calls?
Use `VITE_API_URL`. For local development, set it to `http://localhost:5000/api`.

### Where should I start if I am new to the project?
Start with [Homepage](./index.html), then [Business Overview](./business-overview.html), then [Getting Started](./getting-started.html).

## Documentation

### Is this documentation suitable for GitHub Pages?
Yes. The docs folder is structured for publishing through GitHub Pages from the repository docs directory.

## Related Pages

- [Business Overview](./business-overview.html)
- [Getting Started](./getting-started.html)
- [Architecture](./architecture.html)
- [Roadmap](./roadmap.html)
