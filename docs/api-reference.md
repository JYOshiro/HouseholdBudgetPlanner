# API Reference

<p class="page-intro">This page provides a controller-level endpoint catalog for integration planning, validation, and test execution.</p>

## Purpose
This section documents the implemented backend endpoint surface at the controller level to support integration and test planning.

## Conventions
- Base URL in local development is provided by ASP.NET runtime output and launch settings.
- All routes are under /api.
- Protected routes require Authorization: Bearer <token> header.

## Authentication

### AuthController
Base route: /api/auth

- POST /register
  - Registers a user and establishes household context.
  - Authentication not required.

- POST /login
  - Authenticates and returns JWT token.
  - Authentication not required.

- GET /me
  - Returns current authenticated user.
  - Authentication required.

## Household

### HouseholdsController
Base route: /api/households

- GET /
  - Returns household details for the authenticated user.
  - Authentication required.

- GET /members
  - Returns household members.
  - Authentication required.

## Categories

### CategoriesController
Base route: /api/categories

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}

All category routes require authentication.

## Expenses

### ExpensesController
Base route: /api/expenses

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}

All expense routes require authentication.

## Income

### IncomeController
Base route: /api/income

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}

All income routes require authentication.

## Budgets

### BudgetsController
Base route: /api/budgets

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}

All budget routes require authentication.

## Bills

### BillsController
Base route: /api/bills

- GET /
- GET /upcoming
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}
- POST /{id}/pay

All bill routes require authentication.

## Savings Goals

### SavingsGoalsController
Base route: /api/savings-goals

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}

All savings goal routes require authentication.

## Goal Contributions

### GoalContributionsController
Base route: /api/goals/{goalId}/contributions

- GET /
- GET /{id}
- POST /
- PUT /{id}
- DELETE /{id}

All goal contribution routes require authentication.

## Dashboard

### DashboardController
Base route: /api/dashboard

- GET /summary
  - Returns period-based aggregate dashboard data.
  - Authentication required.

## Notes for Integration Teams
- Use Swagger in development to inspect request and response schema details per endpoint.
- DTO definitions are located in backend/DTOs and should be used as source of truth for frontend typing.
- Household scoping is enforced server side; client applications should not assume cross-household visibility.
