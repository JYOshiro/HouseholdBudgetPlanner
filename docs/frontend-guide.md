---
title: Frontend Guide
---

This page describes the current frontend implementation, the intended direction for the app structure, and the main integration priorities. It is written for developers who need to understand what already exists and what still needs work.

## Quick Links

- [Current Status](#current-status)
- [Technology Stack](#technology-stack)
- [Current Structure](#current-structure)
- [Routing](#routing)
- [Authentication and Token Handling](#authentication-and-token-handling)
- [Backend Integration Approach](#backend-integration-approach)
- [Known Gaps and Priorities](#known-gaps-and-priorities)

## Current Status

Current status: Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned.

The frontend is beyond the scaffold stage. It already includes a working application shell and authentication foundation.

| Area | Status | Notes |
|---|---|---|
| App shell | Implemented | Main app layout, router, and feature pages exist |
| Landing page | Implemented | Separate public entry page exists at `/` |
| Authentication UI | Implemented | Login and registration pages are present |
| Auth state management | Implemented | `AuthContext` stores the token and current user |
| Protected routing | Implemented | Auth-only routes are guarded with `RequireAuth` |
| Shared HTTP client | Implemented | `shared/api/httpClient.ts` handles API requests and error wrapping |
| Feature data integration | Partial | Some pages exist before full API wiring is complete |
| UX hardening | In progress | Loading, error, and empty states need broader coverage |

> Important: the current frontend expects `VITE_API_URL`, not `VITE_API_BASE_URL`. The local default in code is `https://localhost:5001/api`, which does not match the main docs default of `http://localhost:5000/api` unless you override it.

## Technology Stack

| Technology | Purpose |
|---|---|
| React 18 | Component-based UI |
| TypeScript | Shared type safety across app state and API data |
| Vite | Dev server and build pipeline |
| React Router 7 | Route definitions and guarded navigation |
| Fetch API | HTTP communication |
| Tailwind CSS and UI libraries | Styling and component primitives |

## Current Structure

The frontend follows a mostly feature-based structure with app-level routing and shared infrastructure.

```text
frontend/src/
├── app/
│   ├── App.tsx
│   ├── providers/
│   └── router/
├── features/
│   ├── auth/
│   ├── bills/
│   ├── budget/
│   ├── dashboard/
│   ├── finance/
│   ├── household/
│   ├── landing/
│   ├── savings/
│   ├── settings/
│   └── transactions/
├── shared/
│   ├── api/
│   ├── layout/
│   └── types/
├── styles/
└── main.tsx
```

### What each area is for

| Folder | Purpose |
|---|---|
| `app/` | App bootstrapping, router setup, and global providers |
| `features/auth/` | Auth pages, auth API calls, route guards, and auth context |
| `features/landing/` | Public landing page |
| `features/dashboard/` | Dashboard page and summary views |
| `features/transactions/` | Transaction-focused screens, likely combining expense and income workflows |
| `features/budget/` | Budget pages |
| `features/bills/` | Bills pages |
| `features/savings/` | Savings goal pages |
| `features/household/` | Household-related pages |
| `features/settings/` | Settings area |
| `shared/api/` | Shared HTTP client and request helpers |
| `shared/layout/` | Main authenticated layout |
| `shared/types/` | Shared API and view-model types |

## Routing

The current router uses a public area and an authenticated app area.

| Route | Purpose | Access |
|---|---|---|
| `/` | Landing page | Public |
| `/login` | Login page | Public |
| `/register` | Registration page | Public |
| `/app` | App shell with dashboard by default | Auth required |
| `/app/transactions` | Transactions page | Auth required |
| `/app/budget` | Budget page | Auth required |
| `/app/bills` | Bills page | Auth required |
| `/app/savings` | Savings page | Auth required |
| `/app/household` | Household page | Auth required |
| `/app/settings` | Settings page | Auth required |

`RequireAuth` protects the `/app` route tree. `RedirectIfAuthenticated` keeps signed-in users away from `/login` and `/register`.

## Authentication and Token Handling

The current auth flow is already implemented in the app.

1. The user logs in or registers.
2. The frontend receives an auth response with `token`, `expiresIn`, and `user`.
3. The token is stored in local storage under `hb_auth_token`.
4. `AuthContext` keeps the active user and token in app state.
5. On app startup, the provider calls `GET /api/auth/me` when a token exists.
6. If the token is invalid, the app clears local auth state.

### Current auth responsibilities

| Part | Responsibility |
|---|---|
| `AuthContext` | Stores token, user, loading state, login, register, logout, and refresh behavior |
| `authApi` | Wraps `/auth/register`, `/auth/login`, and `/auth/me` |
| `RequireAuth` | Blocks unauthenticated access to app routes |
| `RedirectIfAuthenticated` | Redirects signed-in users away from auth pages |

## Backend Integration Approach

The current integration pattern is service-oriented and should stay that way.

### Shared HTTP client

`shared/api/httpClient.ts` provides:

- the API base URL
- a common request helper
- an `ApiError` type with status and payload
- basic response parsing and validation error extraction

### Current environment variable

```env
VITE_API_URL=http://localhost:5000/api
```

This should be set explicitly in local development to avoid the current fallback mismatch.

### Recommended integration rules

1. Keep raw `fetch` calls inside shared API helpers or feature API modules.
2. Keep route components focused on state and rendering, not request construction.
3. Mirror backend DTOs in shared TypeScript types.
4. Do not send `householdId` from the frontend. Household scope is derived from JWT claims.
5. Handle `401`, `400`, and network failures explicitly in the UI.

### Example pattern

```typescript
import { apiRequest } from "../../shared/api/httpClient";
import type { Expense } from "../../shared/types/api";

export function getExpenses(token: string) {
  return apiRequest<Expense[]>("/expenses", {
    method: "GET",
    token,
  });
}
```

## DTO and Contract Alignment

The backend DTOs are the source of truth for API shapes.

| Contract source | Location |
|---|---|
| Backend DTOs | `backend/DTOs/` |
| Frontend shared types | `frontend/src/shared/types/api.ts` |
| Interactive reference | Swagger UI at `http://localhost:5000/swagger` |

When a backend DTO changes, the matching frontend type should be updated at the same time.

## Known Gaps and Priorities

| Priority | Item | Why it matters |
|---|---|---|
| High | Standardize local API URL configuration | Prevents avoidable local integration failures |
| High | Complete feature-level API wiring | Makes the existing screens functional end to end |
| High | Add consistent loading and error states | Prevents fragile or confusing UX |
| Medium | Review naming consistency across features | `transactions`, `budget`, and `finance` should map clearly to backend domains |
| Medium | Expand type coverage | Reduces drift between frontend and backend contracts |
| Medium | Improve session expiry handling | Better redirects and messaging on token expiry |
| Lower | Add more reusable view components | Helps keep page implementations consistent |

## Suggested Next Frontend Milestones

1. align `VITE_API_URL` across local, production, and documentation
2. wire dashboard and transaction pages to live endpoints
3. complete CRUD flows for budgets, bills, and savings goals
4. add loading, empty, and error states across all authenticated pages
5. add frontend tests around auth bootstrap and protected routing

## Related Pages

- [Getting Started](./getting-started.html)
- [API Reference](./api-reference.html)
- [Architecture](./architecture.html)
- [Roadmap](./roadmap.html)
