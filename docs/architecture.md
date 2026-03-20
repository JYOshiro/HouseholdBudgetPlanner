---
title: Architecture
---

This page explains how the system is built, why each decision was made, and what each component does. It's written as practical reference material, not a formal governance document.

## Quick Links

- [System Overview](#system-overview)
- [Why This Approach](#why-this-approach)
- [Component Responsibilities](#component-responsibilities)
- [Backend Request Lifecycle](#backend-request-lifecycle)
- [Modules and Data](#modules-and-data)
- [Security and Isolation](#security-and-isolation)

## System Overview

Three independently deployable components:

| Component | Role | Stack |
|---|---|---|
| Frontend client | Renders the UI, handles routing, manages auth state | React 18, TypeScript, Vite |
| Backend API | Validates requests, enforces rules, scopes data to households | ASP.NET Core 9, C#, Entity Framework Core |
| Database | Source of truth for all user and financial data | PostgreSQL 14+ |

These components communicate via REST over HTTPS. The frontend needs the backend URL. The backend needs database credentials and JWT settings.

**System Context Diagram:**

This diagram shows how requests flow through all three layers:

```
┌─────────────────┐       ┌──────────────────┐       ┌──────────────┐
│  Browser/React  │ HTTP  │  ASP.NET Core    │ SQL   │  PostgreSQL  │
│  (Port 5173)    │◄────►│  API (Port 5000) │◄────►│  (Port 5432) │
└─────────────────┘       └──────────────────┘       └──────────────┘
    JWT Token                Service Layer           Financial Data
    Auth State              Business Rules            Entities
    UI State                Household Scoping         Relationships
```

All communication is synchronous request-response. The backend is stateless—each request carries its own authorization context via JWT claims. The database is the single source of truth for all persistent state.

## Why This Approach

Five core architectural decisions shaped this design:

1. **Separate frontend and backend**  
   Each layer can be changed, tested, and deployed independently. Frontend goes to Netlify, backend to your hosting provider. Database upgrades don't block frontend pushes.

2. **Service-layer business logic**  
   All household scoping, budget rules, and authentication checks happen in the same place. No scattered logic. This prevents bugs where one controller forgets to check household membership.

3. **DTOs for API contracts**  
   The API speaks DTOs, not database entities. Frontend never sees password fields, internal IDs, or other leaky details. DTOs also let you evolve the database schema without breaking integrations.

4. **JWT tokens with household claims**  
   Each request carries its own authorization context in the token. The backend doesn't need to look up household membership on every call. Faster, simpler, and the token is self-verifying via the JWT signature.

5. **PostgreSQL with Entity Framework Core**  
   Relational model maps cleanly to C# entities. Migrations handle schema evolution automatically. No impedance mismatch or complex ORM configs.

Together, these decisions keep the codebase maintainable while solving real problems: auth, authorization, data isolation, and consistency. The result is code that's easy to reason about.

## Component Responsibilities

### Frontend

The React app handles:
- client-side routing and page navigation
- login and logout flows
- storing and refreshing JWT tokens
- enforcing protected routes
- fetching and displaying API data
- basic form validation before submission

### Backend API

The ASP.NET Core API handles:
- validating all incoming data
- issuing, signing, and validating JWT tokens
- enforcing household-level access control
- implementing domain rules (budget uniqueness, bill state transitions, etc.)
- mapping entities to DTOs before returning to clients
- structured error responses

### Service Layer

The most important architectural boundary:

- **Controllers** are thin. They read requests and return responses. No business logic here.
- **Services** perform the actual work: they validate, apply rules, and enforce household scoping.
- **DbContext** handles persistence and queries.

Household scoping happens *once* in the service layer, not repeated in controllers or data access methods. This prevents accidental cross-household leaks.

### Authentication Middleware

- Validates JWT signature and expiration
- Extracts `userId` and `householdId` claims
- Populates `HttpContext.User` for the request

Controllers and services then read from claims, not from request bodies.

## Backend Request Lifecycle

```
1. HTTP handler validates JWT signature and populates claims
                    ↓
2. Controller receives request, extracts user/household from claims
                    ↓
3. Controller delegates to Service
                    ↓
4. Service filters by household, applies business rules
                    ↓
5. Service uses DbContext to query or update PostgreSQL
                    ↓
6. Service maps entity result to DTO
                    ↓
7. Controller returns HTTP response (200, 400, 401, etc.)
```

**Real example:** Create an expense
1. Client sends `POST /api/expenses` with `Authorization: Bearer <token>`
2. Auth middleware validates token, extracts `householdId` claim
3. ExpensesController calls `expenseService.Create(dto, householdId)`
4. Service validates the DTO, checks household membership, creates entity
5. Entity Framework saves to PostgreSQL
6. Service maps entity to response DTO (no database IDs, no passwords, etc.)
7. Controller returns `201 Created` with the DTO

The important part: the household is never taken from the request body. It comes from the token.

## Modules and Data

### Domain Modules

| Module | Route | Purpose |
|---|---|---|
| Auth | `/api/auth` | Register, log in, get current user |
| Households | `/api/households` | Retrieve household details and members |
| Categories | `/api/categories` | Manage expense categories |
| Expenses | `/api/expenses` | Record and manage transactions |
| Income | `/api/income` | Record and manage income entries |
| Budgets | `/api/budgets` | Set and track monthly category limits |
| Bills | `/api/bills` | Track recurring bills and payment status |
| Savings Goals | `/api/savings-goals` | Define goals and track progress |
| Contributions | `/api/goals/{id}/contributions` | Record deposits toward goals |
| Dashboard | `/api/dashboard` | Month-based financial summary |

### Data Ownership Model

**Entity Relationship Diagram:**

The household is the root of data ownership. All financial entities belong to a household. This is enforced at both database level (foreign keys) and application level (service-layer scoping).

```
                    ┌──────────────┐
                    │  Household   │◄──── Ownership root
                    └──────┬───────┘
           ┌────────────────┼────────────────┐
           │                │                │
      ┌────▼───┐       ┌───▼─────┐    ┌────▼──────┐
      │  Users │       │ Category │    │ Expenses  │
      └────────┘       └──────────┘    └───────────┘
                             │
                        ┌────▼──────┐
                        │  Budgets  │
                        └───────────┘
           
      ┌─────────────┐  ┌──────────┐  ┌───────────────┐
      │   Income    │  │  Bills   │  │ SavingsGoals  │
      └─────────────┘  └──────────┘  └───────┬───────┘
                                              │
                                      ┌───────▼──────┐
                                      │Contributions │
                                      └──────────────┘
```

Key principle: **All queries are scoped to the household.** You cannot load an Expense without also checking its household membership first. This happens in the service layer, not in repos or controllers.

### Key Entities

| Entity | Role |
|---|---|
| User | Authenticated person, belongs to exactly one Household |
| Household | Boundary for all financial data and access control |
| Category | Expense classification, can be system-default or household-custom |
| Expense | Spending record, tied to a Category |
| Income | Income record |
| Budget | Monthly spending limit per Category, unique per household/category/month |
| Bill | Recurring or one-time bill with due date and paid status |
| SavingsGoal | Named savings target with amount and optional deadline |
| GoalContribution | Individual deposit toward a SavingsGoal |

### Constraints

- Email is globally unique
- One Budget per Household + Category + Calendar month
- All entities include `CreatedAt` and `UpdatedAt` audit timestamps
- GoalContribution references only goals in the same household

## Technology Stack

| Layer | Technology | Version |
|---|---|---|
| **Frontend** |
| Framework | React | 18.x |
| Language | TypeScript | 5.x |
| Build tool | Vite | 5.x |
| Routing | React Router | 7.x |
| Styling | Tailwind CSS, Shadcn/UI | — |
| **Backend** |
| Framework | ASP.NET Core | 9.0 |
| Language | C# | 13 |
| ORM | Entity Framework Core | 9.0 |
| Validation | FluentValidation | — |
| Mapping | AutoMapper | — |
| Auth | JWT / HS256 | — |
| Password hashing | BCrypt | work factor 12 |
| **Infrastructure** |
| Database | PostgreSQL | 14+ |
| Logging | ASP.NET Core built-in | — |
| Error handling | Global middleware | — |

## Security and Isolation

### Authentication

JWT bearer tokens issued at login. Token includes `userId`, `email`, and `householdId` claims. No refresh token rotation in current baseline; tokens are issued for a fixed duration (default 60 minutes).

### Authorization

Household scope is derived from JWT claims, not from request bodies. The backend enforces: "You can only access data belonging to your household."

### Password Storage

Passwords are hashed with BCrypt (work factor 12) and never stored in plain text. Password fields are never returned in API responses.

### Error Responses

Cross-household access attempts return `404 Not Found`, not `403 Forbidden`. This prevents enum attacks where someone tries to guess valid resource IDs across households.

### CORS

Frontend origins are configured in backend settings. Only those origins can make cross-origin requests.

### Logging and Monitoring

Global exception middleware catches unhandled errors and returns consistent error objects. No stack traces are sent to clients in production.

### What's Not Yet Hardened

- No rate limiting on auth endpoints (brute-force risk)
- No audit logging for sensitive operations
- No session expiry refresh mechanism
- No integration tests for household isolation

See [Security and Privacy](./security-privacy.html) for current controls and gaps.

## Related Pages

- [Business Overview](./business-overview.html)
- [API Reference](./api-reference.html)
- [Frontend Guide](./frontend-guide.html)
- [Deployment](./deployment.html)
