# Technical Specification

## 1. Solution Overview
The Household Budget Planner App uses a split architecture:
- Frontend: Vite + React + TypeScript single-page application.
- Backend: ASP.NET Core Web API with layered service design.
- Database: PostgreSQL via Entity Framework Core.

The backend currently represents the strongest implementation baseline and exposes domain APIs consumed by the frontend.

## 2. Architecture Style
- API-first service architecture.
- Layered backend: Controllers -> Services -> Data Access (DbContext).
- DTO-based request/response contracts.
- Middleware-driven error handling.
- Household-scoped multi-tenant data model (single household membership per user in current baseline).

## 3. Technology Baseline

### Frontend
- React 18, TypeScript, Vite.
- UI ecosystem includes Material UI and Radix-based components.
- Routing handled with React Router.

### Backend
- ASP.NET Core target framework: .NET 8.
- Entity Framework Core 9 with Npgsql provider.
- JWT bearer authentication.
- AutoMapper for mapping contracts.
- FluentValidation registered in dependency container.

### Infrastructure
- PostgreSQL as system of record.
- Swagger/OpenAPI enabled in development runtime.

## 4. Backend Components

### 4.1 Entry and Composition Root
- Program configuration includes:
  - JWT settings bootstrap and validation.
  - DbContext registration using configured connection string.
  - Authentication and authorization middleware.
  - CORS policy for frontend origins.
  - Service registration for all domain modules.
  - AutoMapper and FluentValidation wiring.
  - Startup migration execution and default category seeding.

### 4.2 Controllers (Current Domain Surface)
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

### 4.3 Service Layer
Service interfaces and implementations isolate business logic from transport concerns. Controllers are intentionally thin and delegate business operations to services.

### 4.4 Data Access Layer
- ApplicationDbContext defines entity sets and relationships.
- EF migrations in backend/Migrations maintain schema evolution.
- Startup applies migrations through context.Database.Migrate().

## 5. Data Model Summary
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

## 6. Security Model
- JWT claim model includes user and household context.
- Password hashing uses BCrypt (work factor 12).
- Protected endpoints require bearer token authorization.
- Household isolation is enforced at service/query level for data safety.

## 7. API Documentation and Observability
- Swagger UI available in development for endpoint discovery and validation.
- Global exception middleware standardizes error responses and logging behavior.
- Built-in ASP.NET logging configuration supports environment-specific verbosity.

## 8. Deployment and Runtime Configuration

### 8.1 Development
- appsettings.Development.json stores local database and JWT development settings.
- CORS allows local frontend origins.

### 8.2 Production
- appsettings.json defines production defaults.
- JWT secret must be strong (minimum 32 characters).
- Connection strings and secrets should be managed through secure environment configuration.

## 9. Build and Run Baseline

### Backend
1. dotnet restore
2. dotnet ef database update
3. dotnet run

### Frontend
1. npm install
2. npm run dev

## 10. Current Risks and Technical Gaps
- Frontend feature screens are not fully integrated with backend API contracts.
- Automated backend and frontend test suites are limited and should be expanded.
- Operational hardening (CI/CD, deployment policy, production monitoring) should be formalized before broad release.

## 11. Recommended Next Technical Steps
1. Complete frontend API integration for all implemented backend modules.
2. Add integration tests for authentication and at least one full financial CRUD flow.
3. Add frontend tests for routing guards and auth bootstrap behavior.
4. Introduce CI pipeline checks for build, lint, and test quality gates.
