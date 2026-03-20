---
title: Household Budget Planner
---

Shared household budgeting and finance tracking for income, expenses, bills, budgets, and savings goals.

Household Budget Planner is a shared finance platform for small households such as couples, families, or flatmates. It gives one household a single place to manage day-to-day financial activity instead of splitting information across spreadsheets, notes, and banking apps.

## Quick Start

| I want to... | Start with |
|---|---|
| Understand the product | [Business Overview](./business-overview.html) |
| Set up and run locally | [Getting Started](./getting-started.html) |
| Build the frontend | [Frontend Guide](./frontend-guide.html) |
| Integrate the API | [API Reference](./api-reference.html) |
| Understand the design | [Architecture](./architecture.html) |
| Check delivery status | [Roadmap](./roadmap.html) |
| Review security | [Security and Privacy](./security-privacy.html) |

## Project Status

Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned. See the [Roadmap](./roadmap.html) for details.

| Area | Status |
|---|---|
| Backend API | ✓ Complete |
| Database | ✓ Complete |
| Authentication | ✓ Complete |
| Frontend shell | ⏳ In progress |
| Frontend features | ⏳ In progress |
| Production readiness | 🔜 Planned |

## What's Implemented

**Backend:** All core finance modules are ready to use.
- household-scoped authentication and authorization
- expense, income, budget, and bill management
- savings goals with contribution tracking
- dashboard summaries by period

**Frontend:** The app shell and auth flow are ready.
- landing page, login, and registration pages
- auth context with token persistence
- protected routing and feature page scaffolds
- responsive layout and component library

## Documentation

| Page | For | Purpose |
|---|---|---|
| [Business Overview](./business-overview.html) | Stakeholders, assessors | Product goals, scope, and delivery state |
| [Getting Started](./getting-started.html) | Developers | Local setup and first-run verification |
| [Architecture](./architecture.html) | Technical reviewers | Design decisions and component roles |
| [API Reference](./api-reference.html) | Backend integrators | Endpoints, auth, examples, and status codes |
| [Frontend Guide](./frontend-guide.html) | Frontend developers | Structure, auth flow, and integration roadmap |
| [Deployment](./deployment.html) | DevOps, reviewers | Configuration, deployment sequence, and release checklist |
| [Roadmap](./roadmap.html) | Project stakeholders | Complete, in-progress, and planned work |
| [Security and Privacy](./security-privacy.html) | Security reviewers | Auth model, isolation, and controls |
| [Testing](./testing.html) | QA, reviewers | Test posture and quality gates |

## Key Resources

| Resource | URL |
|---|---|
| Repository | [github.com/JYOshiro/HouseholdBudgetPlanner](https://github.com/JYOshiro/HouseholdBudgetPlanner) |
| Frontend dev | `http://localhost:5173` |
| Backend API | `http://localhost:5000/api` |
| Swagger UI | `http://localhost:5000/swagger` |
| Tech stack | React 18, TypeScript, Vite, ASP.NET Core 9, PostgreSQL |

---

Last updated: March 2026
