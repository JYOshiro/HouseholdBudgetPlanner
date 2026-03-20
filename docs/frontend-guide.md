---
title: Frontend Guide
---

This page describes the current frontend implementation, the target architecture, and the implementation roadmap. It's written for developers who need to understand what exists now and how to complete the app.

## Quick Links

- [Current State](#current-state)
- [Target State](#target-state)
- [Implementation Sequence](#implementation-sequence)
- [Technology and Structure](#technology-and-structure)
- [Backend Integration](#backend-integration)

## Current State

The frontend is beyond the scaffold stage but not yet feature-complete.

**What's built:**

| Component | Status | Notes |
|---|---|---|
| App shell and routing | ✓ | Main layout, route definitions, public/auth areas configured |
| Landing page | ✓ | Public homepage before login |
| Auth pages | ✓ | Login and registration UI complete |
| Auth infrastructure | ✓ | `AuthContext`, token persistence, session bootstrap |
| Protected routing | ✓ | `RequireAuth` and `RedirectIfAuthenticated` guards in place |
| Shared HTTP client | ✓ | Fetch wrapper with token injection, error handling |
| Feature pages | ⏳ | Dashboard, transactions, budget, bills, savings, household, settings scaffolds exist |
| Feature bindings | ⏳ | Page structure exists but most API integration incomplete |
| UX hardening | ⏳ | Loading, error, and empty states need broader coverage |

**Key note:** The frontend expects `VITE_API_URL` (not `VITE_API_BASE_URL`). The code defaults to `https://localhost:5001/api`, which does NOT match the documented backend at `http://localhost:5000/api`. Always set `VITE_API_URL` explicitly in local development.

## Target State

The finished frontend should:

1. **All pages functional end-to-end**
   - Dashboard shows real month-based summaries
   - Transactions page lists and creates expenses/income
   - Budget page shows limits and spending
   - Bills page displays and allows payment
   - Savings page tracks goals and contributions
   - Household page shows members
   - Settings page available

2. **Consistent UX across all pages**
   - Loading spinners while fetching data
   - Error messages on API failures
   - Empty state guidance when no data exists
   - Form validation feedback
   - Success confirmations on create/update/delete

3. **Reliable auth flow**
   - Token bootstrap at app start
   - Session restored on page refresh
   - Graceful logout
   - Expired token handling (redirect to login)
   - Token refresh mechanism (optional)

4. **Frontend DevX**
   - Type-safe API consumption (DTO alignment)
   - Reusable API client functions
   - Clear error boundaries and retry logic
   - Structured component hierarchy
   - Shared utilities for formatting and validation

## Implementation Sequence

Recommend building features in this order to unblock dependencies:

### Phase 1: API integration foundation (immediate blockers)

1. **Fix `VITE_API_URL` configuration**
   - Update `frontend/.env.local` and `.env.production`
   - Align with backend documentation
   - Prevents silent integration failures

2. **Build shared type definitions**
   - Add TypeScript interfaces for all backend DTOs under `shared/types/api.ts`
   - Mirror the backend DTOs exactly (source of truth)
   - Makes API consumption type-safe

3. **Build API service layer**
   - One service module per domain (`expenseService.ts`, `budgetService.ts`, etc.)
   - Export simple functions that wrap fetch calls
   - Handle errors consistently (401 → logout, 400 → validation feedback, 5xx → retry)

### Phase 2: Core workflows (dependencies manageable)

4. **Dashboard page**
   - Fetch `/api/dashboard/summary` for current month
   - Display totals, recent transactions, upcoming bills, savings progress
   - Add month/year selector

5. **Transactions page**
   - List expenses and income together
   - Create/edit/delete flows
   - Filter by category if time permits
   - This unblocks understanding the main financial story

6. **Bills page**
   - List bills with status
   - Support "mark as paid" action
   - Quick win: simpler than transactions

### Phase 3: Secondary features (can happen in parallel)

7. **Budget page**
   - List monthly budgets
   - Create/edit budgets
   - Show actual vs budgeted comparison

8. **Savings page**
   - List goals with progress bars
   - Create goals
   - Add contributions

9. **Household page**
   - Display household name and members
   - (Multi-user edit not in baseline)

### Phase 4: UX hardening and polish

10. **Consistent loading, error, empty states**
    - Add a shared `<Loading />` component
    - Add shared error fallback UI
    - Add "no data" guidance on each page

11. **Form validation**
    - Client-side validation before submit
    - Server error feedback in forms
    - Prevent duplicate submissions

12. **Auth hardening**
    - Graceful token expiry (redirect to login with message)
    - Automatic logout on 401
    - Retry logic for transient failures

## Module Recommendations

Organize feature modules like this:

```
features/
├── dashboard/
│   ├── pages/DashboardPage.tsx
│   ├── components/SummaryCard.tsx
│   ├── services/dashboardService.ts
│   ├── types/index.ts
│   └── hooks/useDashboard.ts
├── transactions/
│   ├── pages/TransactionsPage.tsx
│   ├── components/TransactionList.tsx
│   ├── components/TransactionForm.tsx
│   ├── services/expenseService.ts
│   ├── services/incomeService.ts
│   ├── types/index.ts
│   └── hooks/useTransactions.ts
├── [budget/, bills/, savings/, etc. — same structure]
```

Each feature module is self-contained. Services call the shared HTTP client. Components don't call fetch directly.

## Technology and Structure

### Current Stack

| Layer | Stack |
|---|---|
| Framework | React 18 + TypeScript |
| Build + Dev | Vite 5 |
| Routing | React Router 7 |
| HTTP | Fetch API (wrapped) |
| Styling | Tailwind CSS + Shadcn/UI |
| State management | Context API (auth) + React hooks (feature state) |

### Current Folder Structure

```
frontend/src/
├── app/
│   ├── App.tsx              # Root, router setup
│   ├── providers/           # Global context providers
│   └── router/              # Route definitions
├── features/
│   ├── auth/                # Login, register, auth context
│   ├── landing/             # Public home page
│   ├── dashboard/           # User dashboard
│   ├── transactions/        # Expenses and income
│   ├── budget/
│   ├── bills/
│   ├── savings/
│   ├── household/
│   └── settings/
├── shared/
│   ├── api/                 # HTTP client, endpoints
│   ├── layout/              # Main app layout wrapper
│   ├── types/               # Shared TypeScript types
│   └── utils/               # Formatting, validation helpers
├── styles/                  # Global CSS
└── main.tsx                 # Vite entry point
```

## Backend Integration

### Shared HTTP Client

Located at `shared/api/httpClient.ts`. Provides:

- API base URL (from `VITE_API_URL`)
- Request helper with automatic token injection
- Response parsing and error extraction
- API error type with status and message

**Usage pattern:**

```typescript
import { request } from "../../shared/api/httpClient";
import type { Expense } from "../../shared/types";

export const expenseService = {
  list: (token: string) =>
    request<Expense[]>("/expenses", {
      headers: { Authorization: `Bearer ${token}` },
    }),
  create: (token: string, data: CreateExpenseDto) =>
    request<Expense>("/expenses", {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: JSON.stringify(data),
    }),
};
```

### Important Rules

1. **Never send `householdId` from the frontend.** It comes from JWT claims on the backend.
2. **Don't import DTOs from backend.** Mirror them as TypeScript interfaces in `shared/types/api.ts`.
3. **All API calls go through feature services,** not directly in components.
4. **Handle 401 specially:** Log out the user and redirect to login.
5. **Mirror backend DTO changes immediately** to avoid contract drift.

### Environment Configuration

```env
# frontend/.env.local (development)
VITE_API_URL=http://localhost:5000/api

# frontend/.env.production (before build)
VITE_API_URL=https://your-api.example.com/api
```

Without this set correctly, API calls will silently fail or hit the wrong server.

## Next Steps

**Immediate (today):**
- Fix `VITE_API_URL` environment configuration
- Add type definitions for all backend DTOs

**This week:**
- Build API service modules
- Wire dashboard to real data

**Next week:**
- Complete transactions, budget, bills pages
- Add consistent UX states (loading, error, empty)

**Polish phase:**
- Auth hardening (token expiry, retry logic)
- Form validation and feedback
- Comprehensive component tests

## Related Pages

- [API Reference](./api-reference.html)
- [Architecture](./architecture.html)
- [Deployment](./deployment.html)
- [Getting Started](./getting-started.html)
