---
title: Roadmap
---

Delivery status, timelines, and priorities from today through production launch.

The current focus is straightforward: finish frontend feature integration first, then lock quality gates and production hardening before broad release.

## Status Summary

| Phase | Status | Timeline | Next milestone |
|---|---|---|---|
| **Backend foundation** | ✓ Complete | Complete | No changes planned |
| **Frontend integration** | ⏳ In progress | 1–2 weeks | Dashboard + Transactions live |
| **Quality & hardening** | 🔜 Planned | Week 3–4 | Full test coverage |
| **Production readiness** | 🔜 Planned | Week 4+ | Feature-complete release |

Get the full picture below. See [Frontend Guide](./frontend-guide.html) for implementation priorities and [Architecture](./architecture.html) for design decisions.

## ✓ Complete

The backend is production-ready today. All financial workflows and auth are fully implemented and tested via Swagger.

**Backend API (ready for integration):**
- 10 RESTful endpoints across finance modules (auth, expenses, income, budgets, bills, savings, dashboard)
- JWT-based authentication with household isolation
- PostgreSQL persistence with migrations
- Default category seeding on startup
- Global exception middleware for consistent error handling

**Frontend shell (ready for data binding):**
- React SPA with routing and protected routes
- Login, registration, and landing pages
- Auth context with token persistence
- Feature page scaffolds awaiting API integration
- Responsive layout foundation

## ⏳ In Progress

Frontend feature integration targets completion in 1–2 weeks.

**Work priority (recommended sequence):**

| Item | Week | Blocker | Success criteria |
|---|---|---|---|
| 1. API environment config | This week | None | `VITE_API_URL` points to working backend |
| 2. Shared type definitions | This week | #1 | All DTOs mirrored in TypeScript |
| 3. API service layer | This week | #2 | Services wrap all fetch calls |
| 4. Dashboard page | Week 1 | #3 | Real `/dashboard/summary` data displays |
| 5. Transactions page | Week 1 | #3 | Create/edit/delete expenses and income |
| 6. Bills page | Week 1 | #3 | List bills and mark as paid |
| 7. Budget & Savings pages | Week 2 | #3 | All feature pages functional |
| 8. UX hardening | Week 2 | #7 | Loading states, error handling, empty states |

**Definition of done for this phase:**
- All pages functional end-to-end (dashboard, transactions, budgets, bills, savings)
- Auth session restored on page refresh
- Consistent UX patterns (loading spinners, error messages, empty states)
- Type-safe API consumption (DTOs match backend exactly)
- Graceful handling of 401 (logout), network failures (retry), and validation errors (form feedback)

## 🔜 Planned

After frontend integration is complete (weeks 3+).

**Phase 3: Quality & Testing (Week 3)**

*Priority: Required before broad release*

- Backend unit tests for all service layer logic
- Backend integration tests for auth flows and household isolation
- Frontend component tests for routing and auth context
- End-to-end tests for critical workflows (register → create expense → view dashboard)
- CI/CD integration: tests run on every pull request, block merge if failing

**Phase 4: Production Readiness (Week 4)**

*Priority: Required before deployment*

- Health check endpoint (for monitoring and load balancers)
- Rate limiting on auth endpoints (prevent brute-force attacks)
- Audit logging for sensitive operations (login, payments, goal creation)
- Session management: token refresh, expiry policies, graceful logout
- Operational runbooks (deployment, troubleshooting, incident response)
- Security review and penetration testing

**Phase 5: Product Enhancement (Post-launch)**

*Priority: Nice-to-have, can ship later*

- Dashboard trends: spending over time, category breakdowns
- Budget variance reporting: where you overspent vs actual budget
- Reporting exports: CSV and PDF statements
- Enhanced mobile responsiveness: better tablet and phone UX
- Notifications: upcoming bills, savings goals near completion

## Long-Term (Future)

Not committed, but directionally possible:

- Bank account and CSV import integrations
- Mobile-native clients (iOS, Android)
- Multi-household membership for one user
- Advanced financial forecasting
- Audit history and compliance reporting

## Related Pages

- [Frontend Guide](./frontend-guide.html)
- [Architecture](./architecture.html)
- [Deployment](./deployment.html)
- [Testing](./testing.html)
