---
title: Frontend Guide
---

This page describes the current frontend implementation, the target architecture, and the implementation roadmap. It's written for developers who need to understand what exists now and how to complete the app.

## Quick Links

- [Current State](#current-state)
- [Target State](#target-state)
- [Recommended Sequence (Short Form)](#recommended-sequence-short-form)
- [Implementation Sequence](#implementation-sequence)
- [Savings Goals UX Behavior](#savings-goals-ux-behavior)
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

## Recommended Sequence (Short Form)

If you need a compact delivery plan, use this order:

1. Configure `VITE_API_URL` correctly
2. Add shared API DTO types
3. Build feature service modules
4. Integrate dashboard and transactions first
5. Integrate budgets, bills, savings, and household pages
6. Harden loading, errors, forms, and auth expiry handling

## Implementation Sequence

**This is your delivery roadmap.** Complete phases sequentially to unblock dependencies and maintain momentum.

### Phase 1: Foundation (Day 1)

1. **Fix API configuration**
   - Set `VITE_API_URL=http://localhost:5000/api` in `frontend/.env.local`
   - The default `https://localhost:5001/api` is wrong. Silent API failures happen if this is missed.
   - This must be done before any integration work.

2. **Create shared type definitions**
   - Add TypeScript interfaces for all backend DTOs in `shared/types/api.ts`
   - Copy from backend: `User`, `Household`, `Expense`, `Income`, `Budget`, `Bill`, `SavingsGoal`, `GoalContribution`, `Category`, `DashboardSummary`
   - These become the contract. Keep them in sync with backend—mismatch causes bugs at runtime.

3. **Build API service modules**
   - One module per domain: `services/authService.ts`, `services/expenseService.ts`, `services/dashboardService.ts`, etc.
   - Each service exports simple, named functions
   - All errors are routed through consistent handlers (401 → logout, 4xx → form feedback, 5xx → retry)

**Done when:** Dashboard loads real data for the current month.

### Phase 2: Core Workflows (Days 2–4)

4. **Dashboard page** (highest priority)
   - Fetch `GET /api/dashboard/summary?year=X&month=Y`
   - Display income, expenses, net, upcoming bills, savings progress
   - Add month/year selector for browsing past months
   - Foundation for understanding the financial picture

5. **Transactions page** (complex but critical)
   - List all expenses and income in a single view
   - Create new transactions with category selector
   - Edit and delete existing transactions
   - Filter by date range and category (nice to have)
   - This is 70% of user workflows

6. **Bills page** (quick win)
   - List bills with due date and payment status
   - Single action: "Mark as paid" button
   - Optional: show upcoming (unpaid) bills only
   - Simpler than transactions; boost confidence

**Done when:** User can enter transactions, update bills, and see summaries.

### Phase 3: Secondary Features (Days 5–6)

7. **Budget page**
   - Show monthly budgets by category
   - Create new budget (month + category + limit)
   - Display actual vs budgeted spending
   - Optional: visual progress bars

8. **Savings page**
   - List goals grouped into Active and Completed sections (preferred)
   - Create and edit goals (name, amount, date, priority, details)
   - Add contributions only where state and policy allow
   - Recalculate progress and remaining immediately after edits/contributions
   - Show completion date and completed-state messaging where available

9. **Household page**
   - Display household name and members
   - (Multi-user management not required for baseline)

**Done when:** All core financial workflows are exposed in the UI.

## Savings Goals UX Behavior

Savings goals are lifecycle-based objects and must not be rendered as a single static card style.

### Required state behavior

| Goal status | Required UI signals | Primary action behavior |
|---|---|---|
| `Active` | Goal title, priority, saved/target/remaining, progress bar, target date | `Add Contribution` is primary; `Edit Goal` available as secondary |
| `Completed` | Completed badge/label, saved amount, target amount, completion date (if present), remaining shown as `$0.00` | Do not use the same primary action as active goals; prefer `Edit Goal`, `View Details`, or `Archive Goal` |
| `Archived` | Hidden from default main list (or shown only in archive view) | Contribution actions should be disabled or unavailable |

### List organization

- preferred: separate sections for `Active Goals` and `Completed Goals`
- acceptable fallback: sort completed goals below active goals with strong visual distinction

### Edit flow requirements

When users edit goal fields (name, target amount, target date, priority, notes/details):

- save changes without deleting or rewriting contribution history
- recalculate progress and remaining values
- recalculate status and UI state immediately
- update completion timestamp behavior based on status transition

Use the decision page as product source-of-truth: [Savings Goals Decision Note](./savings-goals-decision-note.html).

### Phase 4: Hardening (Day 7+)

10. **Consistent UX states**
    - Add `<LoadingSpinner />` component, use on all async operations
    - Add error boundary; show user-friendly errors
    - Add "no data" messaging on empty states

11. **Form validation and feedback**
    - Validate fields before submit
    - Show server errors inline in forms
    - Disable submit during load; prevent double-clicks

12. **Auth robustness**
    - Detect token expiry (401 response)
    - Redirect to login with "session expired" message
    - Add retry logic for transient 5xx errors
    - Handle network failures gracefully

**Done when:** Every page looks finished, loads smoothly, and recovers from errors.

## Module Architecture

This is how to organize code so it scales and stays maintainable.

### Feature Module Structure

Each feature should be self-contained:

```
features/dashboard/
├── pages/
│   └── DashboardPage.tsx          # Route target; fetches and renders
├── components/
│   ├── SummaryCard.tsx            # Reusable pieces
│   ├── TransactionList.tsx
│   └── SavingsProgress.tsx
├── services/
│   └── dashboardService.ts        # API calls only
├── hooks/
│   └── useDashboard.ts            # Fetch logic and state
├── types/
│   └── index.ts                   # Feature-specific types
└── constants.ts                   # Feature constants (labels, etc.)
```

Apply the same pattern to all features: transactions, budget, bills, savings, etc.

### Service Layer (Critical)

**Services are the API boundary:**

```typescript
// services/expenseService.ts
import { request } from "../../shared/api/httpClient";
import type { Expense, CreateExpenseDto } from "../../shared/types";

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
  
  delete: (token: string, id: number) =>
    request<void>(`/expenses/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    }),
};
```

**Never call fetch directly from components.** All API communication flows through services.

### Hook Layer (State + Logic)

Encapsulate fetch logic and state management:

```typescript
// hooks/useDashboard.ts
import { useEffect, useState } from "react";
import { dashboardService } from "../services/dashboardService";
import { useAuth } from "../../auth/hooks/useAuth";

export function useDashboard(year: number, month: number) {
  const { token } = useAuth();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    dashboardService
      .summary(token, year, month)
      .then(setData)
      .catch(setError)
      .finally(() => setLoading(false));
  }, [token, year, month]);

  return { data, loading, error };
}
```

Components use the hook and focus only on rendering:

```typescript
// pages/DashboardPage.tsx
import { useDashboard } from "../hooks/useDashboard";

export function DashboardPage() {
  const [year, setYear] = useState(2026);
  const [month, setMonth] = useState(3);
  const { data, loading, error } = useDashboard(year, month);

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorMessage error={error} />;
  return <SummaryCard data={data} />;
}
```

This keeps concerns separated and makes testing easier.

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

The service implementation pattern is shown above in **Service Layer (Critical)** and should be reused across all features.

### Integration Rules (Don't Skip These)

1. **Never send `householdId` in request bodies**
   - The backend derives household from JWT claims
   - If you send it, it's ignored (wasted bandwidth)
   - If you try to override it, the request fails silently
   - **Result:** Always comes from the token

2. **Don't import backend DTOs; mirror them in TypeScript**
   - Create types in `frontend/shared/types/api.ts`
   - Keep them in sync with backend responses
   - Use `diff` tools or API test to catch drift
   - **Result:** Type-safe async calls

3. **Route all API calls through feature services**
   - No `fetch` in components
   - No `fetch` in hooks (call services instead)
   - One service per domain (auth, dashboard, expenses, etc.)
   - **Result:** Centralized, testable API layer

4. **Handle 401 responses specially**
   - 401 = token expired or invalid
   - Log out the user immediately
   - Redirect to login page
   - Show "session expired" message
   - **Result:** Users know why they're logged out

5. **Align `VITE_API_URL` with actual backend location**
   - Local: `http://localhost:5000/api`
   - Staging: `https://staging-api.example.com/api`
   - Production: `https://api.example.com/api`
   - Mismatch causes silent failures; use Swagger to debug
   - **Result:** API calls reach the right server

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
