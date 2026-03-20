---
title: Household Budget Planner
---

Shared household budgeting and finance tracking for income, expenses, bills, budgets, and savings goals.

This documentation is the main entry point for the project. It is designed for stakeholders, developers, and reviewers who need a clear view of what the app does, what has already been built, and where to look next.

## Quick Links

- [Start Here](#start-here)
- [Documentation Map](#documentation-map)
- [Current Status](#current-status)
- [What Is Implemented](#what-is-implemented)
- [Key Links](#key-links)

> Current status: Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned. See the [Roadmap](./roadmap.html) for details.

## What This App Is

Household Budget Planner is a shared finance platform for small households such as couples, families, or flatmates. It gives one household a single place to manage day-to-day financial activity instead of splitting information across spreadsheets, notes, and banking apps.

The platform supports:

- expense tracking
- income tracking
- monthly budgets by category
- recurring bill management
- savings goals and contributions
- household-scoped dashboards and summaries

## Start Here

| If you are... | Start with | Why |
|---|---|---|
| New to the project | [Business Overview](./business-overview.html) | Understand the product problem, target users, and scope |
| A developer setting up locally | [Getting Started](./getting-started.html) | Install dependencies, configure the backend, and run the app |
| Working on the UI | [Frontend Guide](./frontend-guide.html) | Review frontend structure, auth flow, and integration priorities |
| Working on backend integration | [API Reference](./api-reference.html) | Find endpoints, auth rules, and example requests |
| Reviewing technical design | [Architecture](./architecture.html) | See the system layout, responsibilities, and design decisions |
| Assessing progress | [Roadmap](./roadmap.html) | Check what is complete, in progress, and planned |
| Reviewing controls and quality | [Security and Privacy](./security-privacy.html) and [Testing](./testing.html) | See the current security posture and test coverage guidance |

## Documentation Map

| Page | Purpose | Primary Audience |
|---|---|---|
| [Business Overview](./business-overview.html) | Product context, users, business value, scope, and success criteria | Stakeholders, assessors |
| [Getting Started](./getting-started.html) | Local setup, prerequisites, and first-run verification | Developers |
| [Architecture](./architecture.html) | System overview, component responsibilities, and key design choices | Developers, reviewers |
| [API Reference](./api-reference.html) | Auth model, endpoint groups, examples, and status codes | Developers |
| [Frontend Guide](./frontend-guide.html) | Frontend structure, backend integration approach, and current gaps | Frontend developers |
| [Deployment](./deployment.html) | Environment setup, deployment flow, and release checklist | Developers, reviewers |
| [Roadmap](./roadmap.html) | Delivery status across complete, in-progress, and planned work | Stakeholders, team members |

## Current Status

| Area | Status | Notes |
|---|---|---|
| Backend API | Complete | Core modules are implemented and exposed through REST endpoints |
| Database | Complete | PostgreSQL schema, migrations, and category seeding are in place |
| Authentication | Complete | Register, login, JWT issuance, and current-user lookup are working |
| Frontend shell | In progress | Routing, auth context, route protection, and feature pages exist |
| Frontend feature integration | In progress | Screens still need broader API wiring, refinement, and validation |
| Production hardening | Planned | Expanded tests, operational checks, and deployment safeguards remain |

## What Is Implemented

The current backend supports the main household finance workflows:

- household-scoped authentication
- categories
- expenses
- income
- budgets
- bills and bill payment tracking
- savings goals and contributions
- dashboard summaries by month

The current frontend already includes:

- a landing page
- login and registration flows
- auth context with token persistence
- protected routing for app pages
- feature areas for dashboard, transactions, budget, bills, savings, household, and settings

## Key Links

| Item | Value |
|---|---|
| Repository | [GitHub Repository](https://github.com/JYOshiro/HouseholdBudgetPlanner) |
| Documentation site | [GitHub Pages Site](https://jyoshiro.github.io/HouseholdBudgetPlanner/) |
| Frontend dev server | `http://localhost:5173` |
| Backend API | `http://localhost:5000/api` |
| Swagger UI | `http://localhost:5000/swagger` |
| Backend stack | ASP.NET Core 9, Entity Framework Core, PostgreSQL |
| Frontend stack | React 18, TypeScript, Vite |
| Auth model | JWT bearer tokens |

## Next Recommended Reads

1. Read the [Business Overview](./business-overview.html) if you want the shortest route to product understanding.
2. Read the [Getting Started](./getting-started.html) and [API Reference](./api-reference.html) if you plan to run or extend the system.
3. Read the [Architecture](./architecture.html) and [Roadmap](./roadmap.html) if you are reviewing delivery quality and technical direction.

---

Last updated: March 2026
