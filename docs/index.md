---
title: Household Budget Planner
---

Shared household budgeting and finance tracking for income, expenses, bills, budgets, and savings goals.

Instead of juggling spreadsheets, notes, and multiple banking apps, small households now have a single shared hub for managing day-to-day finances. Household Budget Planner gives you household-wide visibility into income, expenses, budgets, and savings—with clear access controls and reliable isolation between households.

## Get Started

Find what you need based on your role:

| I want to... | Start with |
|---|---|
| Understand the product and scope | [Business Overview](./business-overview.html) |
| Set up and run everything locally | [Getting Started](./getting-started.html) |
| Build and integrate the frontend | [Frontend Guide](./frontend-guide.html) |
| Use or integrate the API | [API Reference](./api-reference.html) |
| Review system design | [Architecture](./architecture.html) |
| Check what's done and what's next | [Roadmap](./roadmap.html) |
| Review security controls | [Security and Privacy](./security-privacy.html) |

## Current Status

Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned. See the [Roadmap](./roadmap.html) for the full timeline and priorities.

| Component | Status |
|---|---|
| **Backend API & database** | ✓ Complete and tested |
| **Authentication & authorization** | ✓ Complete |
| **Frontend shell & routing** | ⏳ In progress |
| **Frontend feature integration** | ⏳ In progress |
| **Production readiness** | 🔜 Planned |

## What's Implemented

### Backend (Ready for integration)

All core financial modules are fully functional and tested through Swagger:
- **auth:** User registration, login, JWT token generation
- **finance modules:** Expenses, income, budgets, bills, savings goals, contributions
- **queries:** Dashboard summary, category lookup, household isolation enforcement
- **persistence:** PostgreSQL schema, migrations, audit fields

### Frontend (Scaffold + auth flow)

The application shell is built with routing and authentication ready:
- **pages:** Landing page, login, registration, protected routes for features
- **auth:** Registration flow, login flow, token persistence in browser storage
- **layout:** Responsive design, navigation structure, component library
- **routing:** React Router with protected route guards
- **integration:** Ready for API binding to feature pages

## Documentation by Role

| Page | Audience | You'll learn |
|---|---|---|
| [Business Overview](./business-overview.html) | Stakeholders, assessors | Product goals, scope, and delivery status in simple terms |
| [Getting Started](./getting-started.html) | Developers | How to set up everything and run it locally |
| [Architecture](./architecture.html) | Technical reviewers | Design decisions, component relationships, and system flow |
| [API Reference](./api-reference.html) | Backend integrators, frontend developers | Endpoints, authentication, example requests/responses, and status codes |
| [Frontend Guide](./frontend-guide.html) | Frontend developers | Current structure, integration roadmap, and how to connect to the API |
| [Deployment](./deployment.html) | DevOps, platform engineers | Configuration steps, environment variables, release checklist |
| [Roadmap](./roadmap.html) | Project leadership | What's complete, in-progress, and planned for future phases |
| [Security and Privacy](./security-privacy.html) | Security reviewers | Authentication model, household isolation, and current controls |
| [Testing](./testing.html) | QA, platform reviewers | Test strategy, coverage, and quality approach |
| [Technical Specification](./technical-specification.html) | Technical leads | Technology choices, component responsibilities, and risks |
| [Functional Specification](./functional-specification.html) | Business analysts, QA | Detailed requirements, business rules, and acceptance criteria |
| [FAQ](./faq.html) | Everyone | Common questions about setup, status, and next steps |

## Quick Links

| Resource | What it is | Local URL or location |
|---|---|---|
| **Repository** | GitHub source code | [github.com/JYOshiro/HouseholdBudgetPlanner](https://github.com/JYOshiro/HouseholdBudgetPlanner) |
| **Frontend app** | React SPA (development) | `http://localhost:5173` |
| **Backend API** | ASP.NET Core Web API | `http://localhost:5000/api` |
| **Swagger UI** | API discovery and testing | `http://localhost:5000/swagger` |
| **Tech stack** | Technologies used | React 18, TypeScript, Vite, ASP.NET Core 9, PostgreSQL |

---

**Next step:** See [Getting Started](./getting-started.html) to set up your local environment, or [Business Overview](./business-overview.html) for product context.

Last updated: March 2026
