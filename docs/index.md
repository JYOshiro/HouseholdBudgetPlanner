<div class="top-banner">
	<span><strong>Household Budget Planner</strong> Documentation</span>
	<span class="banner-links">
		<a href="https://github.com/JYOshiro/HouseholdBudgetPlanner" target="_blank" rel="noopener">View Repository</a>
		<a href="https://jyoshiro.github.io/HouseholdBudgetPlanner/" target="_blank" rel="noopener">Project Site</a>
	</span>
</div>

<section class="hero">
	<h1>Household Budget Planner Documentation</h1>
	<p>Household Budget Planner is a shared-finance platform that enables households to manage income, expenses, budgets, bills, and savings goals through a unified operating model and API foundation.</p>
</section>

<div class="kpi-grid">
	<article class="kpi-card">
		<h3>Core Business Scope</h3>
		<p>End-to-end household finance operations across planning, tracking, and review.</p>
	</article>
	<article class="kpi-card">
		<h3>Architecture Model</h3>
		<p>Frontend client plus layered ASP.NET Core API with PostgreSQL as system of record.</p>
	</article>
	<article class="kpi-card">
		<h3>Documentation Focus</h3>
		<p>Executive clarity, implementation traceability, and release readiness.</p>
	</article>
</div>

## Executive Summary

This documentation set establishes a single source of truth for business stakeholders, delivery teams, and technical assessors. It outlines the product's business rationale, architecture decisions, delivery status, and release considerations in a format suitable for governance reviews and implementation handoff.

## Product Summary

The solution is designed around household-scoped financial management:

- Users authenticate and operate inside a single household context.
- Financial records are separated by domain modules (expenses, income, budgets, bills, goals).
- Dashboard reporting provides period-based summaries for financial oversight.
- A layered backend architecture keeps controllers, business logic, and data access distinct.

## Business Value

- Improves household planning discipline through centralized budget and category controls.
- Strengthens monthly visibility through consistent transaction and billing workflows.
- Reduces delivery risk through modular API domains and clear ownership boundaries.
- Provides a stable baseline for analytics expansion, automation, and production hardening.

## High-Level Architecture

1. Frontend client sends authenticated requests to backend API routes.
2. Controllers validate request context and delegate to domain services.
3. Services enforce household isolation and business rules.
4. Entity Framework persists and retrieves domain entities in PostgreSQL.
5. Dashboard endpoints aggregate data for monthly operational review.

See [Architecture](./architecture.html) and [Security and Privacy](./security-privacy.html) for implementation detail.

<div class="callout">
This documentation is maintained as a formal baseline for executive communication, delivery alignment, and technical assurance.
</div>

## Environment Reference

| Item | Value |
|---|---|
| Frontend URL (development) | http://localhost:5173 |
| Backend API (development) | http://localhost:5000/api |
| Swagger UI (development) | http://localhost:5000/swagger |
| Database | PostgreSQL |
| Auth Model | JWT Bearer token |
| Backend Framework | ASP.NET Core (.NET 8 target) |

## Audience Paths

- Product and non-technical stakeholders: [Business Overview](./business-overview.html), [Roadmap](./roadmap.html), [FAQ](./faq.html)
- Technical assessors: [Architecture](./architecture.html), [Security and Privacy](./security-privacy.html), [Testing](./testing.html)
- Developers: [Getting Started](./getting-started.html), [API Reference](./api-reference.html), [Frontend Guide](./frontend-guide.html), [Deployment](./deployment.html)

## Documentation Map

| Document | Purpose |
|---|---|
| [Business Overview](./business-overview.html) | Scope, users, business goals, and delivery status |
| [Getting Started](./getting-started.html) | Local setup and first run instructions |
| [Architecture](./architecture.html) | System components, data flow, and technical model |
| [Security and Privacy](./security-privacy.html) | Authentication, authorization, and data isolation controls |
| [API Reference](./api-reference.html) | Implemented endpoint surface and module routes |
| [Frontend Guide](./frontend-guide.html) | Frontend status, structure, and integration guidance |
| [Testing](./testing.html) | Current test posture and recommended quality gates |
| [Deployment](./deployment.html) | Runtime configuration and release considerations |
| [Roadmap](./roadmap.html) | Delivery priorities and near-term milestones |
| [FAQ](./faq.html) | Common implementation and operations questions |
| [Functional Specification](./functional-specification.html) | Detailed functional requirements and user journeys |
| [Technical Specification](./technical-specification.html) | Detailed technical baseline and architecture constraints |

Last updated: March 2026
