---
title: Roadmap
---

Clear delivery status: what's complete, what's actively being built, and what comes next.

## Delivery Status

| Workstream | Status | Next action |
|---|---|---|
| **Backend foundation** | ✓ Complete | No changes planned for baseline |
| **Frontend application** | ⏳ In progress | Complete feature API integration |
| **Quality & hardening** | 🔜 Planned | Expand test coverage before broad release |

## Complete

The baseline is implemented and tested through Swagger.

**Platform:**
- Backend API with 10 financial modules
- PostgreSQL schema and migrations
- Startup category seeding
- Global exception middleware

**Authentication:**
- User registration and login
- JWT token generation and validation
- Household scoping enforced across all modules

**Financial modules (full CRUD):**
- Categories (default and household-custom)
- Expenses and income tracking
- Monthly budgets with uniqueness constraints
- Bills with payment status tracking
- Savings goals and contributions
- Dashboard month-based summary endpoint

**Frontend foundation:**
- App shell with routing
- Landing page, login, registration pages
- Auth context and token persistence
- Protected route guards

## In Progress

Active work targets API integration completeness.

**Frontend integration work:**

| Item | Status | Purpose |
|---|---|---|
| API environment alignment | 🔜 Next | Fix `VITE_API_URL` mismatch between docs and code defaults |
| Dashboard data binding | ⏳ | Connect real `/dashboard/summary` data |
| Transactions pages | ⏳ | Bind expense and income CRUD to pages |
| Budget pages | ⏳ | Display budgets and actual spending comparison |
| Bills pages | ⏳ | Show upcoming bills and process payments |
| Savings pages | ⏳ | Display goals, track contributions |
| UX hardening | ⏳ | Loading, error, and empty states across all pages |

**Definition of done for frontend:**
- All pages functional end-to-end
- Auth session restored on page refresh
- Consistent UX states (loading, error, empty, success)
- Type-safe API consumption (DTOs aligned)
- Graceful handling of 401 and network failures

## Planned

Next priorities after frontend completeness.

**Quality and testing (high priority):**
- Backend unit tests for service layer
- Backend integration tests for auth and household isolation
- Frontend component tests for routing and auth
- End-to-end tests for critical user workflows
- All tests running in CI on pull requests

**Production readiness:**
- Health check endpoint for monitoring
- Rate limiting on auth endpoints (brute-force protection)
- Audit logging for sensitive operations
- Session refresh and expiry improvements
- Documentation for operational runbooks

**Product enhancement (lower priority):**
- Richer dashboard views (trends, summaries)
- Budget variance reporting and forecasting
- Reporting exports (PDF, CSV)
- Enhanced mobile responsiveness

## Long-Term (Future)

Not committed, but directionally possible:

- Bank account and CSV import integrations
- Mobile-native clients (iOS, Android)
- Multi-household membership for one user
- Advanced financial forecasting
- Audit history and compliance reporting

---

For details on any area, see [Frontend Guide](./frontend-guide.html) or [Architecture](./architecture.html).
