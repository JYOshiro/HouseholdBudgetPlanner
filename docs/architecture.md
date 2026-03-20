# Architecture

<p class="page-intro">System structure, component responsibilities, key design decisions, and data model — the main technical reference for understanding how the platform is built and why.</p>

**Quick links:**
- [System Overview](#system-overview)
- [Backend Layer Structure](#backend-layer-structure)
- [Why This Architecture?](#why-this-architecture)
- [Domain Modules](#domain-modules)
- [Data Model](#data-model)
- [Request Lifecycle](#request-lifecycle)
- [Security Architecture](#security-architecture)

## System Overview

The platform is split into two independently deployable components that communicate over HTTP:

<div class="feature-grid">
	<article class="feature-card">
		<h4>Frontend Client</h4>
		<p>React 18 + TypeScript application built with Vite. Renders the UI, manages local state, and communicates with the backend API using authenticated HTTP requests.</p>
	</article>
	<article class="feature-card">
		<h4>Backend API</h4>
		<p>ASP.NET Core 9 REST API. Handles authentication, enforces business rules and household isolation, and persists all financial data in PostgreSQL via Entity Framework Core.</p>
	</article>
</div>

<div class="diagram-placeholder">
	<strong>System Context</strong>
	Browser → React SPA → HTTPS/REST → ASP.NET Core API → PostgreSQL
	<br><em>Diagram placeholder — see component descriptions below</em>
</div>

## Technology Stack

| Layer | Technology | Version |
|---|---|---|
| Frontend framework | React | 18.x |
| Frontend language | TypeScript | 5.x |
| Build tool | Vite | 5.x |
| UI components | Shadcn/UI + Tailwind CSS | — |
| Routing | React Router | v7 |
| Backend framework | ASP.NET Core | 9.0 |
| Backend language | C# | 13 |
| ORM | Entity Framework Core | 9.0 |
| Database | PostgreSQL | 14+ |
| Authentication | JWT Bearer | HS256 |
| Password hashing | BCrypt | Work factor 12 |
| Object mapping | AutoMapper | — |

## Backend Layer Structure

The backend follows a clean layered architecture. Each request flows through these layers in order:

```
HTTP Request
    ↓
Controller     — reads route/body, extracts claims, returns HTTP response
    ↓
Service        — enforces business rules and household scoping
    ↓
DbContext      — queries and persists entities via EF Core
    ↓
PostgreSQL
```

Each domain module has a dedicated **Controller**, **Service**, **DTO set**, and **Entity**. No business logic lives in controllers. No data access logic lives outside the service/DbContext layer.

Global exception middleware catches all unhandled exceptions and returns consistent error responses — controllers do not contain try/catch blocks.

## Why This Architecture?

**Separate frontend and backend runtimes**
Keeping the two components separate allows independent deployment, independent development, and the ability to host them on different infrastructure (e.g. CDN for frontend, app service for API).

**Layered backend with service layer**
The service layer is the single enforcement point for household isolation and business rules. This prevents household scoping from being accidentally bypassed and makes services independently testable.

**DTOs for all API contracts**
Requests and responses go through DTOs, never raw entities. This prevents accidental data leakage (e.g. password hashes in responses) and decouples the API contract from the database schema.

**AutoMapper**
Keeps controller code clean — entity-to-DTO mapping is declarative and centralized in `MappingProfile.cs`.

**JWT with household claim**
Embedding `householdId` in the token means every request carries its own household context. Services trust this claim rather than accepting `householdId` from request bodies, which prevents cross-household access.

## Domain Modules

| Module | Route Base | Description |
|---|---|---|
| Auth | `/api/auth` | Registration, login, current user |
| Households | `/api/households` | Household details and members |
| Categories | `/api/categories` | Expense categories — system defaults and household-custom |
| Expenses | `/api/expenses` | Spending transaction records |
| Income | `/api/income` | Household income records |
| Budgets | `/api/budgets` | Monthly spending limits per category |
| Bills | `/api/bills` | Recurring bills with due dates and payment tracking |
| Savings Goals | `/api/savings-goals` | Savings targets and contribution tracking |
| Goal Contributions | `/api/goals/{id}/contributions` | Individual contributions toward a savings goal |
| Dashboard | `/api/dashboard` | Aggregated period summaries |

## Data Model

Nine entities make up the core data model:

<div class="feature-grid">
	<article class="feature-card">
		<h4>User</h4>
		<p>Authenticated platform user. Belongs to one Household. Stores hashed password (BCrypt) and email (unique).</p>
	</article>
	<article class="feature-card">
		<h4>Household</h4>
		<p>The organizational unit. All financial data is scoped to a Household. Created automatically at registration.</p>
	</article>
	<article class="feature-card">
		<h4>Category</h4>
		<p>Classification label for Expenses and Budgets. Null <code>HouseholdId</code> = system default (visible to all). Non-null = household-custom.</p>
	</article>
	<article class="feature-card">
		<h4>Expense</h4>
		<p>A spending transaction. Linked to a Category. Scoped to Household.</p>
	</article>
	<article class="feature-card">
		<h4>Income</h4>
		<p>An income record. Tracks amount, date, and source description. Scoped to Household.</p>
	</article>
	<article class="feature-card">
		<h4>Budget</h4>
		<p>A monthly spending limit per Category. Unique constraint: one per Household + Category + Month.</p>
	</article>
	<article class="feature-card">
		<h4>Bill</h4>
		<p>A recurring bill with a due date and paid/unpaid status. Supports a payment action endpoint.</p>
	</article>
	<article class="feature-card">
		<h4>SavingsGoal</h4>
		<p>A savings target with a defined amount and optional deadline. Tracks total contributions.</p>
	</article>
	<article class="feature-card">
		<h4>GoalContribution</h4>
		<p>A contribution payment linked to a SavingsGoal. Represents a single deposit toward the target amount.</p>
	</article>
</div>

<div class="diagram-placeholder">
	<strong>Entity Relationship Diagram</strong>
	User → Household ← Expense, Income, Budget, Bill, SavingsGoal
	<br>SavingsGoal ← GoalContribution
	<br>Expense/Budget → Category
	<br><em>Full ER diagram placeholder — entities and relationships described above</em>
</div>

**Key constraints:**
- Email must be unique across all users
- One Budget per Household + Category + Calendar month
- GoalContributions must reference a SavingsGoal in the same Household
- All entities include `CreatedAt` and `UpdatedAt` audit fields

## Request Lifecycle

1. The client sends an HTTP request with `Authorization: Bearer <token>`
2. ASP.NET Core middleware validates the JWT and populates `HttpContext.User` claims
3. The Controller reads `userId` and `householdId` from claims via `ClaimsPrincipalExtensions`
4. The Controller delegates to the relevant Service, passing the household context
5. The Service enforces household isolation — it only queries or mutates data for the correct household
6. Entity Framework Core executes the query against PostgreSQL
7. The Controller maps the entity result to a DTO via AutoMapper and returns the HTTP response

## Security Architecture

| Control | Implementation |
|---|---|
| Authentication | JWT HS256 tokens, issued at login, validated on every protected request |
| Token claims | `userId`, `email`, `householdId` embedded at token creation |
| Password storage | BCrypt with work factor 12 — passwords are never stored in plain text |
| Household isolation | Enforced in the service layer — not in controllers |
| CORS | Restricted to configured frontend origin(s) |
| Secrets | JWT secret and DB credentials stored in configuration/environment — never hard-coded |

See [Security and Privacy](./security-privacy.html) for the full security posture.
