# Roadmap

<p class="page-intro">What has been implemented, what is currently in progress, and what is planned next — organised by phase.</p>

## Phase Summary

<div class="status-summary-grid">
	<article class="status-card status-done">
		<div class="status-label">Phase 1 — Backend</div>
		<div class="status-value">Complete</div>
		<p>Full backend API implemented. Database schema deployed. Auth and all financial modules operational.</p>
	</article>
	<article class="status-card status-progress">
		<div class="status-label">Phase 2 — Frontend</div>
		<div class="status-value">In Progress</div>
		<p>Frontend integration underway. Auth infrastructure is the current priority before feature screens.</p>
	</article>
	<article class="status-card status-planned">
		<div class="status-label">Phase 3 — Quality &amp; Growth</div>
		<div class="status-value">Planned</div>
		<p>Testing, analytics enhancements, production hardening, and strategic capabilities.</p>
	</article>
</div>

---

## Phase 1 — Backend Foundation

<span class="badge badge-done">Complete</span>

| Deliverable | Status |
|---|---|
| Project scaffolding — backend and frontend | <span class="badge badge-done">Done</span> |
| ASP.NET Core API structure (controllers, services, DTOs, middleware) | <span class="badge badge-done">Done</span> |
| PostgreSQL schema and Entity Framework Core migrations | <span class="badge badge-done">Done</span> |
| Auth module — register, login, JWT token issuance | <span class="badge badge-done">Done</span> |
| Household scoping enforced across all modules | <span class="badge badge-done">Done</span> |
| Categories module — system defaults and household-custom | <span class="badge badge-done">Done</span> |
| Expenses module — full CRUD | <span class="badge badge-done">Done</span> |
| Income module — full CRUD | <span class="badge badge-done">Done</span> |
| Budgets module — full CRUD with uniqueness constraint | <span class="badge badge-done">Done</span> |
| Bills module — full CRUD and payment action | <span class="badge badge-done">Done</span> |
| Savings Goals module — full CRUD | <span class="badge badge-done">Done</span> |
| Goal Contributions module — full CRUD | <span class="badge badge-done">Done</span> |
| Dashboard summary endpoint with period filtering | <span class="badge badge-done">Done</span> |
| Global exception middleware | <span class="badge badge-done">Done</span> |
| Startup default category seeding | <span class="badge badge-done">Done</span> |

---

## Phase 2 — Frontend Integration

<span class="badge badge-progress">In Progress</span>

| Deliverable | Status |
|---|---|
| API client with automatic token injection | <span class="badge badge-progress">In Progress</span> |
| Auth service — login, register, token storage | <span class="badge badge-progress">In Progress</span> |
| AuthContext + PrivateRoute route protection | <span class="badge badge-progress">In Progress</span> |
| Dashboard page bound to `/api/dashboard/summary` | <span class="badge badge-planned">Planned</span> |
| Expenses CRUD screens | <span class="badge badge-planned">Planned</span> |
| Income CRUD screens | <span class="badge badge-planned">Planned</span> |
| Budgets CRUD screens | <span class="badge badge-planned">Planned</span> |
| Bills CRUD screens with payment action | <span class="badge badge-planned">Planned</span> |
| Savings Goals screens | <span class="badge badge-planned">Planned</span> |
| Loading, error, and empty states across all screens | <span class="badge badge-planned">Planned</span> |
| Frontend TypeScript types aligned to backend DTOs | <span class="badge badge-planned">Planned</span> |

---

## Phase 3 — Quality and Enhancement

<span class="badge badge-planned">Planned</span>

| Deliverable | Status |
|---|---|
| Backend unit tests for service layer | <span class="badge badge-planned">Planned</span> |
| Frontend component and integration tests | <span class="badge badge-planned">Planned</span> |
| End-to-end test coverage for critical paths | <span class="badge badge-planned">Planned</span> |
| Health check endpoint | <span class="badge badge-planned">Planned</span> |
| Production deployment configuration hardening | <span class="badge badge-planned">Planned</span> |
| Richer dashboard analytics — trend indicators and charts | <span class="badge badge-planned">Planned</span> |
| Budget variance reporting and monthly forecasting | <span class="badge badge-planned">Planned</span> |
| Monitoring and alerting integration | <span class="badge badge-planned">Planned</span> |
| Reporting exports (PDF / CSV) | <span class="badge badge-planned">Planned</span> |

---

## Long-Term Direction

These are directional goals beyond Phase 3 — not committed deliverables:

- Integration with external financial data providers (open banking / CSV import)
- Mobile-first client — after web feature parity is complete
- Multi-household membership per user
- Audit history views and financial reporting archives
