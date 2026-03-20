# Technical Specification

This page documents the current technical baseline, architecture constraints, runtime configuration, and delivery risks for engineering and review teams.

## Quick Links

- [Solution Overview](#solution-overview)
- [Architecture Style](#architecture-style)
- [Technology Baseline](#technology-baseline)
- [Backend Components](#backend-components)
- [Security Model](#security-model)
- [Runtime Configuration](#runtime-configuration)
- [Build and Run Baseline](#build-and-run-baseline)
- [Risks and Next Steps](#risks-and-next-steps)

## Solution Overview

The Household Budget Planner App uses a split architecture:

- frontend: Vite plus React plus TypeScript single-page application
- backend: ASP.NET Core Web API with layered service design
- database: PostgreSQL via Entity Framework Core

The backend currently represents the strongest implementation baseline and exposes domain APIs consumed by the frontend.

## Architecture Style

- API-first service architecture.
- Layered backend: Controllers -> Services -> Data Access (DbContext).
- DTO-based request/response contracts.
- Middleware-driven error handling.
- Household-scoped multi-tenant data model (single household membership per user in current baseline).

## Technology Baseline

### Frontend

- React 18, TypeScript, Vite.
- UI ecosystem includes Material UI and Radix-based components.
- Routing handled with React Router.

### Backend

- ASP.NET Core target framework: .NET 9.
- Entity Framework Core 9 with Npgsql provider.
- JWT bearer authentication.
- AutoMapper for mapping contracts.
- FluentValidation registered in dependency container.

### Infrastructure

- PostgreSQL as system of record.
- Swagger/OpenAPI enabled in development runtime.

## Backend Components

### Entry and composition root

- Program configuration includes:

  - JWT settings bootstrap and validation
  - DbContext registration using configured connection string
  - authentication and authorization middleware
  - CORS policy for frontend origins
  - service registration for all domain modules
  - AutoMapper and FluentValidation wiring
  - startup migration execution and default category seeding

### Controllers (current domain surface)

- AuthController
- HouseholdsController
- CategoriesController
- ExpensesController
- IncomeController
- BudgetsController
- BillsController
- SavingsGoalsController
- GoalContributionsController
- DashboardController

### Service layer

Service interfaces and implementations isolate business logic from transport concerns. Controllers are intentionally thin and delegate business operations to services.

### Data access layer

- ApplicationDbContext defines entity sets and relationships.
- EF migrations in backend/Migrations maintain schema evolution.
- Startup applies migrations through context.Database.Migrate().

## Data Model Summary

Core entities:

- User
- Household
- Category
- Expense
- Income
- Budget
- Bill
- SavingsGoal
- GoalContribution

Key relationships and constraints:

- User belongs to one household in the current model.
- Household aggregates financial entities.
- Category can be system-level or household-custom.
- Budget includes uniqueness constraints by household/category/month.
- Audit fields are present across core entities.

## Security Model

- Authentication uses JWT bearer tokens.
- Household scope is derived from JWT claims.
- Clients must not send `householdId` in request bodies.
- Password hashing uses BCrypt (work factor 12).
- Protected endpoints require bearer token authorization.
- Household isolation is enforced at service/query level for data safety.

## API Documentation and Observability

- Swagger UI available in development for endpoint discovery and validation.
- Global exception middleware standardizes error responses and logging behavior.
- Built-in ASP.NET logging configuration supports environment-specific verbosity.

## Runtime Configuration

### Development

- appsettings.Development.json stores local database and JWT development settings.
- CORS allows local frontend origins.

### Production

- appsettings.json defines production defaults.
- JWT secret must be strong (minimum 32 characters).
- Connection strings and secrets should be managed through secure environment configuration.

## Build and Run Baseline

### Backend

1. dotnet restore
2. dotnet ef database update
3. dotnet run

### Frontend

1. npm install
2. npm run dev

## Risks and Next Steps

### Current risks and technical gaps

- Frontend feature screens are not fully integrated with backend API contracts.
- Automated backend and frontend test suites are limited and should be expanded.
- Operational hardening (CI/CD, deployment policy, production monitoring) should be formalized before broad release.

### Recommended next technical steps

1. Complete frontend API integration for all implemented backend modules.
2. Add integration tests for authentication and at least one full financial CRUD flow.
3. Add frontend tests for routing guards and auth bootstrap behavior.
4. Introduce CI pipeline checks for build, lint, and test quality gates.

## Related Pages

- [Architecture](./architecture.html)
- [API Reference](./api-reference.html)
- [Deployment](./deployment.html)
- [Testing](./testing.html)
