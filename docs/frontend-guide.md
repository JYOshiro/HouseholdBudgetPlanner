# Frontend Guide

<p class="page-intro">Frontend stack, current delivery status, intended architecture, folder structure, auth/token handling, backend integration approach, and current gaps — for developers working on the frontend.</p>

## Current Status

<div class="status-summary-grid">
	<article class="status-card status-done">
		<div class="status-label">Foundation</div>
		<div class="status-value">In Place</div>
		<p>Project scaffolded with Vite, React, TypeScript, Tailwind CSS, and Shadcn/UI configured.</p>
	</article>
	<article class="status-card status-progress">
		<div class="status-label">API Integration</div>
		<div class="status-value">In Progress</div>
		<p>Auth service and API client are the active development track. Feature screens are queued.</p>
	</article>
	<article class="status-card status-planned">
		<div class="status-label">Full Feature Parity</div>
		<div class="status-value">Planned</div>
		<p>All CRUD screens, empty states, loading states, and error handling to follow integration.</p>
	</article>
</div>

## Technology Stack

| Technology | Purpose |
|---|---|
| React 18 | UI component framework |
| TypeScript | Static typing across the codebase |
| Vite | Build tool and development server |
| Tailwind CSS | Utility-first styling |
| Shadcn/UI | Pre-built accessible UI components |
| React Router v7 | Client-side navigation and routing |
| Fetch API | HTTP client for backend communication |

## Intended Architecture

The frontend uses a **feature-based folder structure**. Each financial domain has its own self-contained module with pages, components, services, hooks, and types. Shared utilities and infrastructure live in a top-level `shared/` directory.

### Folder Structure

<div class="folder-tree">frontend/src/
├── App.tsx                        # Root — providers, router setup
├── main.tsx                       # Entry point
│
├── features/
│   ├── auth/
│   │   ├── pages/                 # LoginPage, RegisterPage
│   │   ├── components/            # LoginForm, RegisterForm
│   │   ├── services/              # authService.ts
│   │   ├── hooks/                 # useAuth.ts
│   │   └── types/                 # AuthDto, LoginDto, etc.
│   │
│   ├── dashboard/
│   │   ├── pages/                 # DashboardPage
│   │   ├── components/            # SummaryCard, BudgetBar, etc.
│   │   ├── services/              # dashboardService.ts
│   │   └── types/
│   │
│   ├── expenses/
│   ├── income/
│   ├── budgets/
│   ├── bills/
│   └── savings/
│
├── shared/
│   ├── api/
│   │   └── apiClient.ts           # Base fetch wrapper with token injection
│   ├── auth/
│   │   └── AuthContext.tsx        # Auth context — user, token, login, logout
│   ├── components/
│   │   ├── PrivateRoute.tsx       # Route guard for authenticated pages
│   │   ├── Layout.tsx             # App shell — nav, sidebar, outlet
│   │   └── ui/                    # Shared presentational components
│   └── utils/
│       ├── formatCurrency.ts      # Consistent currency formatting
│       └── formatDate.ts          # Consistent date formatting
│
└── types/
		└── global.d.ts</div>

## Authentication and Token Handling

The auth flow:

1. User submits the login form — frontend POSTs to `POST /api/auth/login`
2. On success, the API returns a JWT token
3. The token is stored in `localStorage`
4. All subsequent API requests include `Authorization: Bearer <token>` in the header
5. On logout, the token is removed from storage and the user is redirected to `/login`

The `AuthContext` provides the current user object and token to the entire component tree. The `PrivateRoute` component reads from this context and redirects unauthenticated users to `/login`.

### API Client Pattern

All requests go through a shared `apiClient.ts` wrapper that injects the auth token automatically:

```typescript
// shared/api/apiClient.ts
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000/api';

function getToken(): string | null {
	return localStorage.getItem('token');
}

export async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
	const token = getToken();
	const response = await fetch(`${API_BASE}${path}`, {
		...options,
		headers: {
			'Content-Type': 'application/json',
			...(token ? { Authorization: `Bearer ${token}` } : {}),
			...options.headers,
		},
	});

	if (!response.ok) {
		throw new Error(`Request failed: ${response.status} ${response.statusText}`);
	}

	if (response.status === 204) return undefined as T;
	return response.json() as Promise<T>;
}
```

### Service Layer Pattern

Each feature module wraps API calls in a dedicated service file. Components call services — never `fetch` directly:

```typescript
// features/expenses/services/expenseService.ts
import { request } from '../../../shared/api/apiClient';
import type { Expense, CreateExpenseDto } from '../types';

export const expenseService = {
	getAll: () =>
		request<Expense[]>('/expenses'),
	getById: (id: number) =>
		request<Expense>(`/expenses/${id}`),
	create: (data: CreateExpenseDto) =>
		request<Expense>('/expenses', { method: 'POST', body: JSON.stringify(data) }),
	update: (id: number, data: Partial<CreateExpenseDto>) =>
		request<Expense>(`/expenses/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
	remove: (id: number) =>
		request<void>(`/expenses/${id}`, { method: 'DELETE' }),
};
```

## DTO Alignment

Frontend TypeScript types should mirror the backend DTO contracts exactly. Backend DTOs are the source of truth — they are located in `backend/DTOs/`. For example, the C# `ExpenseDto` defines what `GET /api/expenses` returns, and the frontend `Expense` interface should match field-for-field.

<div class="callout-tip">
<strong>Tip:</strong> Use the Swagger UI at <code>http://localhost:5000/swagger</code> to inspect the exact shape of every request and response while developing.
</div>

## Routing

| Route | Component | Auth Required |
|---|---|---|
| `/` | Redirect → `/dashboard` | Yes |
| `/login` | LoginPage | No |
| `/register` | RegisterPage | No |
| `/dashboard` | DashboardPage | Yes |
| `/expenses` | ExpensesPage | Yes |
| `/income` | IncomePage | Yes |
| `/budgets` | BudgetsPage | Yes |
| `/bills` | BillsPage | Yes |
| `/savings` | SavingsPage | Yes |

Protected routes render a `PrivateRoute` wrapper that checks `AuthContext` — unauthenticated users are redirected to `/login`.

## Integration Backlog

<div class="callout-warning">
<strong>Active work:</strong> The items below are the current delivery priorities, ordered by dependency. Auth infrastructure must be in place before any feature screen can work.
</div>

| Priority | Item | Status |
|---|---|---|
| 1 | API client with token injection (`apiClient.ts`) | <span class="badge badge-progress">In Progress</span> |
| 2 | Auth service — login, register, token storage | <span class="badge badge-progress">In Progress</span> |
| 3 | `AuthContext` + `PrivateRoute` setup | <span class="badge badge-progress">In Progress</span> |
| 4 | Dashboard page — bound to `/api/dashboard/summary` | <span class="badge badge-planned">Planned</span> |
| 5 | Expenses CRUD screens | <span class="badge badge-planned">Planned</span> |
| 6 | Income CRUD screens | <span class="badge badge-planned">Planned</span> |
| 7 | Budgets CRUD screens | <span class="badge badge-planned">Planned</span> |
| 8 | Bills CRUD screens + pay action | <span class="badge badge-planned">Planned</span> |
| 9 | Savings Goals screens | <span class="badge badge-planned">Planned</span> |
| 10 | Loading, error, and empty states across all screens | <span class="badge badge-planned">Planned</span> |

## Developer Notes

- **Never pass `householdId` in request bodies.** The backend derives it from the JWT token — passing it in the body will be ignored or could cause errors.
- **API returns `404`, not `403`, for cross-household access.** If a resource exists but belongs to a different household, the API returns 404 — not a permissions error. This is intentional.
- **Use shared formatters.** The `formatCurrency` and `formatDate` utilities in `shared/utils/` ensure consistent output across all screens.
- **Keep API calls in service files.** Components should call services and handle the result — not call `fetch` directly. This keeps components clean and makes services independently testable.
- **API base URL is configurable.** Set `VITE_API_BASE_URL` in `.env.local` for development or `.env.production` for production builds.
