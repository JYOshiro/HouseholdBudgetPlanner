# Architecture

## Architecture Intent
The architecture is designed to balance delivery speed with operational control. It prioritizes clear module boundaries, predictable data ownership, and straightforward maintainability.

## Solution Structure
The solution is organized into two major runtime components:
- Frontend client (Vite + React + TypeScript).
- Backend API (ASP.NET Core + Entity Framework + PostgreSQL).

## Backend Layering
- Controllers expose API endpoints and request contracts.
- Services implement business rules and household scoping.
- Data layer persists entities through ApplicationDbContext.
- Middleware centralizes exception handling and response consistency.

## Domain Modules
- Auth
- Households
- Categories
- Expenses
- Income
- Budgets
- Bills
- Savings Goals
- Goal Contributions
- Dashboard

## Data Flow (Request Lifecycle)
1. Client sends authenticated request.
2. ASP.NET authentication validates bearer token.
3. Controller delegates operation to domain service.
4. Service validates rules and performs data operations.
5. DbContext executes queries against PostgreSQL.
6. Controller returns mapped response DTO.

## Architectural Control Points
- JWT claims establish user and household context at request time.
- Service layer enforces domain policies and household isolation.
- DTO boundaries provide stable contracts between backend and frontend.
- Centralized exception handling improves consistency and supportability.

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

Key constraint examples:
- Email uniqueness for users.
- Budget uniqueness per household/category/month.
- GoalContribution linked to valid SavingsGoal.
