# Architecture

This page explains how the system is put together, why the main design decisions were made, and what each component is responsible for. It is intended to be practical reference material rather than a formal architecture report.

## Quick Links

- [System Overview](#system-overview)
- [Why This Architecture](#why-this-architecture)
- [Component Responsibilities](#component-responsibilities)
- [Backend Request Flow](#backend-request-flow)
- [Domain Modules](#domain-modules)
- [Data Model Summary](#data-model-summary)
- [Security Notes](#security-notes)

## System Overview

The platform is split into a frontend client and a backend API.

| Component | Responsibility | Technology |
|---|---|---|
| Frontend | Renders the UI, handles routing, manages auth state, and calls the API | React 18, TypeScript, Vite, React Router |
| Backend API | Validates requests, enforces business rules, scopes data to a household, and exposes REST endpoints | ASP.NET Core 9, C#, Entity Framework Core |
| Database | Stores users, households, categories, budgets, bills, transactions, and savings data | PostgreSQL |

> Diagram placeholder: Browser -> React frontend -> HTTP API -> ASP.NET Core services -> PostgreSQL

## Why This Architecture

The architecture is intentionally simple and practical.

| Decision | Reason |
|---|---|
| Separate frontend and backend | Allows each side to be developed and deployed independently |
| Service-based backend | Keeps business rules out of controllers and makes household scoping consistent |
| DTO-based API contracts | Prevents entity leakage and keeps the API stable even if the database model changes |
| JWT auth with household claims | Gives each request enough context to enforce household isolation server-side |
| PostgreSQL with EF Core | Fits the relational data model well and supports migrations cleanly |

This approach keeps the codebase understandable while still covering real application concerns such as authentication, authorization, and data ownership.

## Component Responsibilities

### Frontend

The frontend is responsible for:

- route handling
- login and registration flows
- token persistence in local storage
- showing protected pages only to authenticated users
- rendering data returned by the API

### Backend API

The backend is responsible for:

- validating inputs
- issuing and validating JWT tokens
- enforcing household-level access control
- implementing domain rules for budgets, bills, and savings
- returning API responses through DTOs

### Service Layer

The service layer is the main business boundary.

- Controllers read the request and return HTTP responses.
- Services perform business operations and household checks.
- `ApplicationDbContext` handles persistence.

This is important because household scoping should not be repeated differently in multiple controllers.

## Backend Request Flow

```text
HTTP request
  -> authentication middleware validates JWT
  -> controller reads route, body, and user claims
  -> service applies business rules and household filtering
  -> EF Core queries or updates PostgreSQL
  -> controller returns DTO response
```

In practice, a protected request works like this:

1. the client sends `Authorization: Bearer <token>`
2. ASP.NET Core validates the token
3. the controller extracts `userId` and `householdId` from claims
4. the service performs the requested operation for that household only
5. the result is mapped to a DTO and returned to the client

## Domain Modules

| Module | Route base | Responsibility |
|---|---|---|
| Auth | `/api/auth` | Register, log in, and get current user |
| Households | `/api/households` | Return household details and members |
| Categories | `/api/categories` | Manage default and household-specific categories |
| Expenses | `/api/expenses` | Record and manage spending transactions |
| Income | `/api/income` | Record and manage income transactions |
| Budgets | `/api/budgets` | Manage monthly category budgets |
| Bills | `/api/bills` | Track recurring bills and mark them as paid |
| Savings goals | `/api/savings-goals` | Manage goals and progress |
| Goal contributions | `/api/goals/{goalId}/contributions` | Record contributions against a goal |
| Dashboard | `/api/dashboard` | Return monthly finance summaries |

## Data Model Summary

The core data model is built around household ownership.

| Entity | Role |
|---|---|
| User | Authenticated person who belongs to one household |
| Household | Ownership boundary for all financial data |
| Category | Shared or household-specific classification for financial records |
| Expense | Spending record tied to a category |
| Income | Income record tied to the household |
| Budget | Monthly limit for a category |
| Bill | Recurring or one-time bill with due date and payment state |
| SavingsGoal | Named savings target with amount and optional date |
| GoalContribution | Individual contribution toward a savings goal |

Key constraints:

- one user belongs to one household
- email addresses must be unique
- one budget exists per household, category, and month
- all household finance records are filtered by household ownership
- audit timestamps are stored on entities

> Diagram placeholder: `Household` is the ownership root. `User`, `Expense`, `Income`, `Budget`, `Bill`, and `SavingsGoal` belong to a household. `GoalContribution` belongs to a savings goal. `Expense` and `Budget` link to `Category`.

## Technology Summary

| Layer | Technology |
|---|---|
| Frontend | React 18, TypeScript, Vite |
| Routing | React Router 7 |
| Backend | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | PostgreSQL |
| Authentication | JWT bearer tokens |
| Password hashing | BCrypt |
| Object mapping | AutoMapper |

## Security Notes

| Control | Current approach |
|---|---|
| Authentication | JWT tokens issued on login and validated on protected routes |
| Authorization | Household scope enforced in backend services |
| Password storage | BCrypt hashing |
| CORS | Allowed origins configured in application settings |
| Error handling | Global exception middleware |

For more detail, see [Security and Privacy](./security-privacy.html).

## Related Pages

- [Business Overview](./business-overview.html)
- [API Reference](./api-reference.html)
- [Frontend Guide](./frontend-guide.html)
- [Deployment](./deployment.html)
