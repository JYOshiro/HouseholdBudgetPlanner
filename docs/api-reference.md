---
title: API Reference
---

Complete reference for the backend API. Organized for both first-time integrators and ongoing development.

For exact request/response schemas, use [Swagger UI](http://localhost:5000/swagger) alongside this page.

## Quick Links

- [Base URLs](#base-urls)
- [Typical Integration Flow](#typical-integration-flow)
- [Authentication](#authentication)
- [Status Codes](#status-codes)
- [Endpoint Reference](#endpoint-reference)
- [Examples](#examples)
- [Developer Notes](#developer-notes)

## Base URLs

| Environment | URL |
|---|---|
| Local API | `http://localhost:5000/api` |
| Local Swagger | `http://localhost:5000/swagger` |

> **Frontend integration note:** The frontend reads `VITE_API_URL` and falls back to `https://localhost:5001/api`. For local development against the backend at `http://localhost:5000`, set `VITE_API_URL=http://localhost:5000/api` in **frontend/.env.local**.

## Typical Integration Flow

**Start here if you're new to the API.** Follow these steps to verify your setup and understand how the system works together.

All requests assume you've successfully run the backend and database migrations (see [Getting Started](./getting-started.html)).

### Steps 1–6: Complete a basic workflow

**1. Register a household:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "Jane",
  "lastName": "Doe",
  "householdName": "My Home"
}
```

Response includes a `token`. Store it in your application or browser localStorage.

**2. Log in (for future sessions):**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

Response includes a new `token`.

**3. Use the token on all protected requests:**
```http
GET /api/auth/me
Authorization: Bearer <token>
```

This returns your user profile and confirms the token works. All subsequent requests must include this header.

**4. Fetch categories (provided by default):**
```http
GET /api/categories
Authorization: Bearer <token>
```

You'll receive system categories like "Groceries", "Transportation", "Utilities", etc. Use their IDs when creating expenses.

**5. Create an expense:**
```http
POST /api/expenses
Authorization: Bearer <token>
Content-Type: application/json

{
  "amount": 25.50,
  "description": "Coffee",
  "categoryId": 1,
  "date": "2026-03-20T14:30:00Z",
  "isShared": false
}
```

**6. Get a monthly summary:**
```http
GET /api/dashboard/summary?year=2026&month=3
Authorization: Bearer <token>
```

This returns aggregated totals: income, expenses, savings progress, and budget summaries for the month.

### What you've just learned

- Auth tokens are **required** for all financial operations
- Household scope is **automatic** from the token (no `householdId` in request bodies)
- Request/response are **always JSON**
- Status codes indicate outcomes: `200/201` for success, `400` for validation errors, `401` for auth issues, `404` for not found

This foundation applies to all endpoints. Browse the [Endpoint Reference](#endpoint-reference) for the full API surface.

## Authentication

Two endpoints are unprotected:
- `POST /api/auth/register`
- `POST /api/auth/login`

All other endpoints require a bearer token:

```http
Authorization: Bearer <your-jwt-token>
```

**How it works:**

1. Token is issued at registration or login
2. Token includes `userId`, `email`, and `householdId` claims
3. Backend uses `householdId` from the token to determine what data you can access
4. You cannot override the household by sending `householdId` in the request body — it's ignored

**Token expiration:**

Tokens expire after a fixed duration (default: 60 minutes). When expired, log in again to get a new token.

**Swagger testing:**

Click the **Authorize** button in Swagger and enter `Bearer <token>`. Swagger will send it on all subsequent requests.

## Status Codes

| Code | Meaning | Typical causes |
|---|---|---|
| `200 OK` | Request succeeded | Any successful read, update, or action |
| `201 Created` | Resource created | Not commonly returned from current endpoints; most use `200` |
| `204 No Content` | Delete succeeded | Successful DELETE requests |
| `400 Bad Request` | Validation failed | Missing/invalid fields, duplicate budget, invalid month |
| `401 Unauthorized` | Token missing or invalid | No token, expired token, bad credentials |
| `404 Not Found` | Resource not found | Wrong ID, or resource belongs to a different household |
| `500 Error` | Server error | Unhandled exception (rare in normal use) |

## Endpoint Reference

### Auth

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `POST` | `/api/auth/register` | No | Create a user and initial household |
| `POST` | `/api/auth/login` | No | Log in and receive a JWT token |
| `GET` | `/api/auth/me` | Yes | Get the authenticated user |

### Household

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/households` | Yes | Get your household details |
| `GET` | `/api/households/members` | Yes | Get household members (all users in your household) |

### Categories

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/categories` | Yes | List system and household categories |
| `GET` | `/api/categories/{id}` | Yes | Get a specific category |
| `POST` | `/api/categories` | Yes | Create a household-specific category |
| `PUT` | `/api/categories/{id}` | Yes | Update a category |
| `DELETE` | `/api/categories/{id}` | Yes | Delete a category |

### Expenses

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/expenses` | Yes | List expenses |
| `GET` | `/api/expenses/{id}` | Yes | Get a specific expense |
| `POST` | `/api/expenses` | Yes | Create an expense |
| `PUT` | `/api/expenses/{id}` | Yes | Update an expense |
| `DELETE` | `/api/expenses/{id}` | Yes | Delete an expense |

### Income

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/income` | Yes | List income entries |
| `GET` | `/api/income/{id}` | Yes | Get a specific income entry |
| `POST` | `/api/income` | Yes | Create an income entry |
| `PUT` | `/api/income/{id}` | Yes | Update an income entry |
| `DELETE` | `/api/income/{id}` | Yes | Delete an income entry |

### Budgets

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/budgets` | Yes | List budgets |
| `GET` | `/api/budgets/{id}` | Yes | Get a specific budget |
| `POST` | `/api/budgets` | Yes | Create a monthly budget for a category |
| `PUT` | `/api/budgets/{id}` | Yes | Update a budget |
| `DELETE` | `/api/budgets/{id}` | Yes | Delete a budget |

Note: One budget per household + category + month. Duplicate attempts return `400`.

### Bills

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/bills` | Yes | List bills |
| `GET` | `/api/bills/upcoming` | Yes | List unpaid bills with upcoming due dates |
| `GET` | `/api/bills/{id}` | Yes | Get a specific bill |
| `POST` | `/api/bills` | Yes | Create a bill |
| `PUT` | `/api/bills/{id}` | Yes | Update a bill |
| `DELETE` | `/api/bills/{id}` | Yes | Delete a bill |
| `POST` | `/api/bills/{id}/pay` | Yes | Mark a bill as paid |

### Savings Goals

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/savings-goals` | Yes | List savings goals |
| `GET` | `/api/savings-goals/{id}` | Yes | Get a specific goal |
| `POST` | `/api/savings-goals` | Yes | Create a goal |
| `PUT` | `/api/savings-goals/{id}` | Yes | Update a goal |
| `DELETE` | `/api/savings-goals/{id}` | Yes | Delete a goal |

### Goal Contributions

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/goals/{goalId}/contributions` | Yes | List contributions for a goal |
| `GET` | `/api/goals/{goalId}/contributions/{id}` | Yes | Get a specific contribution |
| `POST` | `/api/goals/{goalId}/contributions` | Yes | Create a contribution |
| `PUT` | `/api/goals/{goalId}/contributions/{id}` | Yes | Update a contribution |
| `DELETE` | `/api/goals/{goalId}/contributions/{id}` | Yes | Delete a contribution |

### Dashboard

| Method | Route | Auth | Parameters |
|---|---|---|---|
| `GET` | `/api/dashboard/summary` | Yes | `year` (int), `month` (int, 1-12) |

Returns summary data for the selected period.

## Examples

### Register and log in

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "alice@example.com",
  "password": "SecurePass123!",
  "firstName": "Alice",
  "lastName": "Smith",
  "householdName": "Smith Household"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "email": "alice@example.com",
    "firstName": "Alice",
    "lastName": "Smith",
    "householdId": 1
  }
}
```

### Create an expense

```http
POST /api/expenses
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "amount": 45.99,
  "description": "Grocery shopping",
  "categoryId": 2,
  "date": "2026-03-20T18:00:00Z",
  "isShared": true
}
```

Response:
```json
{
  "id": 42,
  "amount": 45.99,
  "description": "Grocery shopping",
  "categoryId": 2,
  "categoryName": "Groceries",
  "date": "2026-03-20T18:00:00Z",
  "isShared": true,
  "paidByUserId": 1,
  "paidByUserName": "Alice Smith"
}
```

### Get dashboard summary

```http
GET /api/dashboard/summary?year=2026&month=3
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Response:
```json
{
  "totalIncome": 5000.00,
  "totalExpenses": 2150.75,
  "netAmount": 2849.25,
  "budgetUsage": [
    { "categoryId": 2, "categoryName": "Groceries", "budgeted": 600, "spent": 410 }
  ],
  "upcomingBills": [
    { "id": 1, "description": "Internet", "amount": 79.99, "dueDate": "2026-03-25" }
  ],
  "recentTransactions": [],
  "savingsProgress": []
}
```

## Developer Notes

- **Household scope:** Never send `householdId` in request bodies. The backend derives it from your JWT token. If you try to send it, it will be ignored.
- **404 vs 403:** Cross-household access attempts return `404`, not `403`. This prevents endpoint enumeration attacks.
- **Token storage:** The frontend stores tokens in local storage. In production, consider more secure alternatives.
- **Swagger:** Best way to explore and test endpoints interactively. Available at `http://localhost:5000/swagger` when the backend is running in Development mode.
- **DTO source files:** `backend/DTOs/` contains C# classes that define request and response shapes. These are the source of truth.
- **Validation errors:** `400 Bad Request` responses include details about which fields failed and why.

## Related Pages

- [Getting Started](./getting-started.html)
- [Architecture](./architecture.html)
- [Frontend Guide](./frontend-guide.html)
- [Deployment](./deployment.html)
