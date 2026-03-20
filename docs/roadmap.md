---
title: Roadmap
---

This roadmap shows what is complete, what is actively being worked on, and what is planned next. It is written to make the delivery state easy to scan without having to infer progress from scattered notes.

## Quick Links

- [Status Summary](#status-summary)
- [Complete](#complete)
- [In Progress](#in-progress)
- [Planned](#planned)
- [Long-Term Direction](#long-term-direction)

## Status Summary

| Workstream | Status | Summary |
|---|---|---|
| Backend foundation | Complete | Core API, data model, auth, and database setup are in place |
| Frontend application | In progress | App shell and auth exist; broader feature wiring is still underway |
| Quality and release hardening | Planned | Test coverage, operational readiness, and production safeguards are next |

> Current status: Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned.

## Complete

### Backend and platform baseline

| Item | Notes |
|---|---|
| Project scaffolding | Backend and frontend repositories are set up |
| API structure | Controllers, services, DTOs, and middleware are in place |
| PostgreSQL schema | Entity Framework Core migrations are created and applied |
| Authentication | Register, login, and current-user endpoints are implemented |
| Household isolation | Household scoping is enforced across protected modules |
| Categories | Default and household-specific categories are supported |
| Expenses | Full CRUD is implemented |
| Income | Full CRUD is implemented |
| Budgets | Full CRUD with uniqueness rules is implemented |
| Bills | Full CRUD and bill payment action are implemented |
| Savings goals | Full CRUD is implemented |
| Goal contributions | Full CRUD is implemented |
| Dashboard summary | Month-based summary endpoint is implemented |
| Seed data | Default categories are seeded on startup |

## In Progress

### Frontend delivery

| Item | Current state |
|---|---|
| Public landing page | Implemented |
| Login and registration pages | Implemented |
| Auth context and token persistence | Implemented |
| Protected routing | Implemented |
| Shared API client | Implemented, but environment configuration should be standardized |
| Dashboard integration | Partial |
| Transactions, budget, bills, savings, household, and settings pages | Present as feature areas, with additional API wiring still needed |
| UX states | Loading, error, and empty states need broader consistency |

### Active priorities

1. align frontend environment configuration with the documented backend URL
2. complete feature-level API integration for existing pages
3. improve UX resilience around auth expiry, failures, and empty data states

## Planned

### Quality and release readiness

| Item | Why it matters |
|---|---|
| Backend automated tests | Protects service rules and household isolation |
| Frontend tests | Protects auth flow and navigation behavior |
| End-to-end coverage | Verifies critical workflows across the whole stack |
| Health checks | Improves production monitoring |
| Deployment hardening | Reduces release risk |
| Better operational guidance | Makes the project easier to assess and deploy |

### Product and UX expansion

| Item | Why it matters |
|---|---|
| Richer dashboard insights | Makes the app more useful for monthly review |
| Budget variance and forecasting | Improves planning value |
| Reporting exports | Supports sharing and record keeping |
| Monitoring and alerting | Supports reliability in hosted environments |

## Long-Term Direction

These items are possible future directions, not current delivery commitments.

- bank or CSV import integrations
- mobile-first client experience after web feature parity
- multi-household membership for a single user
- audit history and richer reporting archives

## Related Pages

- [Business Overview](./business-overview.html)
- [Frontend Guide](./frontend-guide.html)
- [Deployment](./deployment.html)
- [Testing](./testing.html)
